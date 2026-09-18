using Robust.Shared.Network;

namespace Content.Shared._Wormix.Players;

public sealed record CharacterWhitelistRole(int characterId, string job, bool isRestricted);

public sealed record CharacterWhitelistRoleWithUser(NetUserId player, string job, bool isRestricted);
