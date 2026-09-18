using System.Threading;
using System.Threading.Tasks;
using Content.Server.Administration;
using Content.Server.Administration.Managers;
using Content.Server.Database;
using Content.Server.EUI;
using Content.Shared._Wormix;
using Content.Shared._Wormix.PlayerCharactersList;
using Content.Shared.Eui;
using Robust.Shared.Network;

namespace Content.Server._Wormix.PlayerCharacterList;

public sealed class PlayerCharacterListEui : BaseEui
{
    [Dependency] private readonly ILogManager _log = default!;
    [Dependency] private readonly IAdminManager _admins = default!;
    [Dependency] private readonly IServerDbManager _db = default!;


    private readonly ISawmill _sawmill;

    private NetUserId PlayerId { get; set; }
    private string PlayerName { get; set; } = string.Empty;

    private List<SharedCharacter> playerCharacters { get; set; } = new List<SharedCharacter>();


    public PlayerCharacterListEui()
    {
        IoCManager.InjectDependencies(this);

        _sawmill = _log.GetSawmill("admin.player_characters_eui");
    }

    public override EuiStateBase GetNewState()
    {
        return new PlayerCharactersListEuiState(PlayerName, playerCharacters);
    }

    private async Task LoadFromDb()
    {
        playerCharacters.Clear();

        var dbChars = await _db.GetPlayerCharacters(PlayerId, CancellationToken.None);

        foreach (var character in dbChars)
        {
            playerCharacters.Add(new SharedCharacter(character.Id, character.CharacterName));
        }

        StateDirty();
    }


    public async void SetPlayer(NetUserId playerId, string playerName)
    {
        PlayerId = playerId;
        PlayerName = playerName;

        await LoadFromDb();
    }

    public override async void Opened()
    {
        base.Opened();
        _admins.OnPermsChanged += OnPermsChanged;
    }

    public override void Closed()
    {
        base.Closed();
        _admins.OnPermsChanged -= OnPermsChanged;
    }

    private void OnPermsChanged(AdminPermsChangedEventArgs args)
    {
        if (args.Player != Player)
        {
            return;
        }

        StateDirty();
    }

}
