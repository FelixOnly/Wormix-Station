using Content.Client.Eui;
using Content.Shared._Wormix;
using Content.Shared._Wormix.PlayerCharactersList;
using Content.Shared.Eui;
using JetBrains.Annotations;

namespace Content.Client._Wormix.Administration.CharacterWhitelist;

[UsedImplicitly]
public sealed class PlayerCharacterListEui : BaseEui
{

    public PlayerCharacterListEui()
    {
        CharactersWindow = new UI.PlayersCharactersListWindow();
        CharactersWindow.OnClose += OnClosed;
    }

    private UI.PlayersCharactersListWindow CharactersWindow { get; }

    private void OnClosed()
    {
        SendMessage(new CloseEuiMessage());
    }

    public override void HandleState(EuiStateBase state)
    {
        if (state is not PlayerCharactersListEuiState s)
            return;

        CharactersWindow.SetTitlePlayer(s.PlayerNameCharacterList);
        CharactersWindow.SetCharacters(s.PlayerCharacters);
    }

    public static void SetData<T>(ICharacterListLine<T> line, SharedCharacter character) where T : SharedCharacter
    {
        line.CharId.Text = character.Id.ToString();
        line.CharName.Text = character.Name;
    }


    public override void Closed()
    {
        base.Closed();
        CharactersWindow.Close();
    }

    public override void Opened()
    {
        CharactersWindow.OpenCentered();
    }
}
