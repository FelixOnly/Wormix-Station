using Content.Shared.Roles;
using Robust.Shared.Network;
using Robust.Shared.Player;

namespace Content.Shared._Wormix.Players;

public sealed record CharacterWhitelistRole(int characterId, string job);

public sealed record CharacterWhitelistRoleWithUser(NetUserId player, string job);
