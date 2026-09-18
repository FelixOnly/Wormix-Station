using Content.Client.Eui;
using Content.Shared._Wormix.PlayerCharactersList;
using Content.Shared.Eui;
using JetBrains.Annotations;


namespace Content.Client._Wormix.Administration.CharacterWhitelist.UI.Character;

[UsedImplicitly]
public sealed class CharacterWhitelistEui: BaseEui
{
    private CharacterWhitelistWindow CharacterWindow { get; }

    public CharacterWhitelistEui()
    {
        CharacterWindow = new CharacterWhitelistWindow();
        CharacterWindow.OnClose += OnClosed;
    }

    private void OnClosed()
    {
        SendMessage(new CloseEuiMessage());
    }

    public override void HandleState(EuiStateBase state)
    {
        if (state is not CharacterRoleWhitelistEuiState s)
            return;

        CharacterWindow.SetCharacter(s.Character.Id, s.Character.Name);
        CharacterWindow.SetWhitelists(s.WhitelistRoles);
    }

    public override void Closed()
    {
        base.Closed();
        CharacterWindow.Close();
    }

    public override void Opened()
    {
        CharacterWindow.OpenCentered();
    }

}
