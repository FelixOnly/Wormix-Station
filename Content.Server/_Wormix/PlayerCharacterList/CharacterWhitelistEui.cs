using Content.Server.Administration;
using Content.Server.Administration.Managers;
using Content.Server.EUI;
using Content.Shared._Wormix;
using Content.Shared._Wormix.PlayerCharactersList;
using Content.Shared.Eui;
using Content.Shared._Wormix.Players;

namespace Content.Server._Wormix.PlayerCharacterList;

public sealed class CharacterWhitelistEui : BaseEui
{
    [Dependency] private readonly IAdminManager _admins = default!;


    public CharacterWhitelistEui(SharedCharacter character, List<CharacterWhitelistRole> whitelistRoles)
    {
        IoCManager.InjectDependencies(this);

        Character = character;
        WhitelistRoles = whitelistRoles;
    }

    private SharedCharacter Character { get; }
    private List<CharacterWhitelistRole> WhitelistRoles { get; }

    public override EuiStateBase GetNewState()
    {
        var tempRestrictions = new List<(string, bool)>();

        foreach (var role in WhitelistRoles)
        {
            tempRestrictions.Add(new ValueTuple<string, bool>()
            {
                Item1 = role.job,
                Item2 = role.isRestricted,
            });
        }

        return new CharacterRoleWhitelistEuiState(Character, tempRestrictions);
    }

    public override async void Opened()
    {
        base.Opened();
        StateDirty();
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
