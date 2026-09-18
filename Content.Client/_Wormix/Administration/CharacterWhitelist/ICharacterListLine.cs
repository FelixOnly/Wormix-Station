using Content.Shared._Wormix;
using Robust.Client.UserInterface.Controls;

namespace Content.Client._Wormix.Administration.CharacterWhitelist;

public interface ICharacterListLine<T> where T : SharedCharacter
{
    T Character { get; }
    Label CharId { get; }
    Label CharName { get; }
    Button CharButton { get; }
}
