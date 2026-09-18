using Content.Shared.Eui;
using Robust.Shared.Serialization;

namespace Content.Shared._Wormix.PlayerCharactersList;

[Serializable, NetSerializable]
public sealed class PlayerCharactersListEuiState: EuiStateBase
{
    public PlayerCharactersListEuiState(string playerNameCharacterList,  List<SharedCharacter> playerCharacters)
    {
        PlayerNameCharacterList = playerNameCharacterList;
        PlayerCharacters = playerCharacters;
    }

    public string PlayerNameCharacterList { get; }

    public List<SharedCharacter> PlayerCharacters { get; }


}
