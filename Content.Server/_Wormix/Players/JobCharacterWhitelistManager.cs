// SPDX-FileCopyrightText: 2026 FelixOnly <62942680+felixonly@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Content.Server.Database;
using Content.Shared._Wormix.Players;
using Content.Shared.Preferences;
using Content.Shared.Roles;
using Robust.Server.Player;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Server._Wormix.Players;

public sealed class JobCharacterWhitelistManager: IPostInjectInit
{
    [Dependency] private readonly IServerDbManager _db = default!;
    [Dependency] private readonly INetManager _net = default!;
    [Dependency] private readonly IPlayerManager _player = default!;
    [Dependency] private readonly UserDbDataManager _userDb = default!;

    private readonly List<CharacterWhitelistRole> _charactersWhitelist = new();


    public void Initialize()
    {
        _net.RegisterNetMessage<MsgJobCharacterWhitelist>();
    }

    private async Task LoadData(ICommonSession session, CancellationToken cancel)
    {
        var playerCharacters = await _db.GetPlayerCharacters(session.UserId, CancellationToken.None);

        if (playerCharacters is null)
        {
            cancel.ThrowIfCancellationRequested();
            return;
        }

        foreach (var character in playerCharacters)
        {
            var charactersRestrictions = await _db.GetJobCharacterWhitelistAll(character.Id, CancellationToken.None);

            foreach (var restriction in charactersRestrictions)
            {
                _charactersWhitelist.Add(new CharacterWhitelistRole(character.Id, restriction.RoleId, restriction.IsRestricted));
            }
        }

        cancel.ThrowIfCancellationRequested();
    }

    private void FinishLoad(ICommonSession session)
    {
        SendJobCharacterWhitelist(session);
    }

    private async void ClientDisconnected(ICommonSession session)
    {
        var playerCharacters = await _db.GetPlayerCharacters(session.UserId, CancellationToken.None);

        foreach (var character in playerCharacters)
        {
            foreach (var restriction in _charactersWhitelist)
            {
                if (restriction.characterId == character.Id)
                {
                    _charactersWhitelist.Remove(restriction);
                }
            }
        }
    }

    public async Task<string> GetCharacterName(int characterId)
    {

        var username = await _db.FindPlayerByCharacter(characterId);

        _player.TryGetPlayerDataByUsername(username, out var data);

        if (data != null)
        {
            var playerCharacters = await _db.GetPlayerCharacters(data.UserId, CancellationToken.None);

            return playerCharacters.Find(x => x.Id == characterId)!.CharacterName;
        }

        return string.Empty;
    }


    public async void AddCharacterWhitelist(NetUserId player, int character, ProtoId<JobPrototype> jobId, bool isRestricted)
    {

        var lastRestriction = _charactersWhitelist.Find(x => x.characterId == character && x.job == jobId.Id);

        if (lastRestriction is not null)
        {
            RemoveWhitelist(player, character, jobId);
        }

        _charactersWhitelist.Add(new CharacterWhitelistRole(character, jobId, isRestricted));

        // Добавить сообщение в логах

        await _db.AddJobCharacterWhitelist(character, jobId, isRestricted);

        if (_player.TryGetSessionById(player, out var session))
            SendJobCharacterWhitelist(session);
    }

    public async Task<string> FindPlayerByCharacter(int character)
    {
        return await _db.FindPlayerByCharacter(character, CancellationToken.None);
    }

    public async Task<int> FindIdCharacterByName(ICommonSession player, string name)
    {
        var characters = await _db.GetPlayerCharacters(player.UserId, CancellationToken.None);

        foreach (var character in characters)
        {
            if (character.CharacterName == name)
            {
                return character.Id;
            }
        }

        return -1;
    }

    public async void RemoveWhitelist(NetUserId player, int characterId, ProtoId<JobPrototype> jobId)
    {
        _charactersWhitelist.RemoveAll(x => x.characterId == characterId && x.job == jobId);

        // Добавить сообщение в логах

        await _db.RemoveJobCharacterWhitelist(characterId, jobId);

        if (_player.TryGetSessionById(new NetUserId(player), out var session))
            SendJobCharacterWhitelist(session);
    }

    public void RemoveAllCharacterWhitelist(NetUserId player, int characterId)
    {
        foreach (var character in _charactersWhitelist)
        {
            RemoveWhitelist(player, character.characterId, character.job);
        }

        if (_player.TryGetSessionById(new NetUserId(player), out var session))
            SendJobCharacterWhitelist(session);
    }

    public List<CharacterWhitelistRole> GetAllCharacterRestrictions(int characterId)
    {
        return _charactersWhitelist.Where(x => x.characterId == characterId).ToList();
    }

    private int GetCharacterIndexOfName(string name, IReadOnlyDictionary<int, ICharacterProfile> characterProfiles)
    {
        return characterProfiles.FirstOrDefault(pair => pair.Value.Name == name).Key;
    }

    public async void SendJobCharacterWhitelist(ICommonSession player)
    {

        var dbCharacters = await  _db.GetPlayerCharacters(player.UserId, CancellationToken.None);
        var playerPref = await _db.GetPlayerPreferencesAsync(player.UserId, CancellationToken.None);

        if(playerPref is null)
            return;

        var tempRestrictions = new List<CharacterWhitelistRole>();


        for (int localCharacter = 0; localCharacter < playerPref.Characters.Count; localCharacter++)
        {

            foreach (var dbRestrictions in _charactersWhitelist)
            {
                if (dbCharacters[localCharacter].Id == dbRestrictions.characterId)
                {
                    tempRestrictions.Add(new CharacterWhitelistRole(
                        GetCharacterIndexOfName(dbCharacters[localCharacter].CharacterName,playerPref.Characters),
                        dbRestrictions.job,
                        dbRestrictions.isRestricted));
                }

            }
        }

        var msg = new MsgJobCharacterWhitelist
        {
            CharactersWhitelist = tempRestrictions,
        };

        // Отправляем игроку список, но айди относительно его списка персонажей
        _net.ServerSendMessage(msg, player.Channel);
    }

    void IPostInjectInit.PostInject()
    {
        _userDb.AddOnLoadPlayer(LoadData);
        _userDb.AddOnFinishLoad(FinishLoad);
        _userDb.AddOnPlayerDisconnect(ClientDisconnected);
    }



}
