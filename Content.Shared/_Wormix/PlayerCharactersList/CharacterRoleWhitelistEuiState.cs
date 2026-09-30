using Content.Shared.Eui;
using Robust.Shared.Serialization;

namespace Content.Shared._Wormix.PlayerCharactersList;

[Serializable, NetSerializable]
public sealed class CharacterRoleWhitelistEuiState: EuiStateBase
{
    public CharacterRoleWhitelistEuiState(SharedCharacter character, List<(string, bool)> whitelistRoles)
    {
        Character = character;
        WhitelistRoles = whitelistRoles;
    }

    public SharedCharacter Character { get; }
    public List<(string, bool)> WhitelistRoles { get; }

}
