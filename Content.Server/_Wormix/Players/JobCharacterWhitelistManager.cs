using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Content.Server.Database;
using Content.Shared._Wormix.Players;
using Content.Shared.Players;
using Content.Shared.Preferences;
using Content.Shared.Roles;
using Robust.Server.Player;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Server.Players;

public sealed class JobCharacterWhitelistManager: IPostInjectInit
{
    [Dependency] private readonly IServerDbManager _db = default!;
    [Dependency] private readonly INetManager _net = default!;
    [Dependency] private readonly IPlayerManager _player = default!;
    [Dependency] private readonly UserDbDataManager _userDb = default!;

    private readonly List<CharacterWhitelistRole> _allow = new();
    private readonly List<CharacterWhitelistRole> _deny = new();

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
            var characterAllowed = await _db.GetJobCharacterWhitelistAllowed(character.Id);
            var characterDenied = await _db.GetJobCharacterWhitelistDenied(character.Id);

            foreach (var job in characterAllowed)
            {
                _allow.Add(new CharacterWhitelistRole(character.Id, job));
            }

            foreach (var job in characterDenied)
            {
                _deny.Add(new CharacterWhitelistRole(character.Id, job));
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
            foreach (var allowCharacter in _allow.ToList())
            {
                if (allowCharacter.characterId == character.Id)
                {
                    _allow.Remove(allowCharacter);
                }
            }

            foreach (var denyCharacter in _deny.ToList())
            {
                if (denyCharacter.characterId == character.Id)
                {
                    _deny.Remove(denyCharacter);
                }
            }
        }
    }


    public async void AddCharacterWhitelist(NetUserId player, int character, ProtoId<JobPrototype> allow, ProtoId<JobPrototype> deny)
    {
        _allow.Add(new CharacterWhitelistRole(character, allow.Id));
        _deny.Add(new CharacterWhitelistRole(character, deny.Id));

        // Добавить сообщение в логах

        await _db.AddJobCharacterWhitelist(character, allow, deny);

        if (_player.TryGetSessionById(player, out var session))
            SendJobCharacterWhitelist(session);
    }

    public async void RemoveWhitelist(NetUserId player, int characterId, ProtoId<JobPrototype> allow, ProtoId<JobPrototype> deny)
    {
        _allow.Remove(new CharacterWhitelistRole(characterId, allow.Id));
        _deny.Remove(new CharacterWhitelistRole(characterId, deny.Id));

        // Добавить сообщение в логах

        await _db.RemoveJobCharacterWhitelist(characterId, allow, deny);

        if (_player.TryGetSessionById(new NetUserId(player), out var session))
            SendJobCharacterWhitelist(session);
    }

    public void RemoveAllCharacterWhitelist(NetUserId player, int characterId)
    {
        foreach (var allow in _allow)
        {
            RemoveWhitelist(player, characterId, allow.job, allow.job);
        }

        foreach (var deny in _deny)
        {
            RemoveWhitelist(player, characterId, deny.job, deny.job);
        }

        if (_player.TryGetSessionById(new NetUserId(player), out var session))
            SendJobCharacterWhitelist(session);
    }

    public async Task<List<string>> GetAllCharacterDenies(int characterId)
    {
        return await _db.GetJobCharacterWhitelistDenied(characterId);
    }

    public async Task<List<string>> GetAllCharacterAllowed(int characterId)
    {
        return await _db.GetJobCharacterWhitelistAllowed(characterId);
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

        var tempAllow = new List<CharacterWhitelistRole>();
        var tempDeny = new List<CharacterWhitelistRole>();


        for (int localCharacter = 0; localCharacter < playerPref.Characters.Count; localCharacter++)
        {
            foreach (var dbAllow in _allow)
            {
                if (dbCharacters[localCharacter].Id == dbAllow.characterId)
                {
                    tempAllow.Add(new CharacterWhitelistRole(
                        GetCharacterIndexOfName(dbCharacters[localCharacter].CharacterName,playerPref.Characters),
                        dbAllow.job));
                }

            }

            foreach (var dbDeny in _deny)
            {
                if (dbCharacters[localCharacter].Id == dbDeny.characterId)
                {

                    tempDeny.Add(new CharacterWhitelistRole(
                        GetCharacterIndexOfName(dbCharacters[localCharacter].CharacterName,playerPref.Characters),
                        dbDeny.job));
                }
            }

        }

        var msg = new MsgJobCharacterWhitelist
        {
            Allow = tempAllow,
            Deny = tempDeny
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
