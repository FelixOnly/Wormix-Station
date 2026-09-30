using Robust.Shared.Serialization;

namespace Content.Shared._Wormix;

[Serializable, NetSerializable]
public record SharedCharacter(int ? Id, string Name);
