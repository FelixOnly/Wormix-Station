using Content.Shared._Wormix.Players;
using Content.Shared.Roles;
using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared.Players;

public sealed class MsgJobCharacterWhitelist: NetMessage
{
    public override MsgGroups MsgGroup => MsgGroups.EntityEvent;

    public List<CharacterWhitelistRole>  Allow = new();
    public List<CharacterWhitelistRole>  Deny = new();

    public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
    {

        // Allow.Clear();
        var allowCount = buffer.ReadVariableInt32();
        Allow.EnsureCapacity(allowCount);

        for (var i = 0; i < allowCount; i++)
        {
            var newId = buffer.ReadInt32();
            var newJob = buffer.ReadString();

            Allow.Add(new CharacterWhitelistRole(newId,newJob));
        }

        // Deny.Clear();
        var denyCount = buffer.ReadVariableInt32();
        Deny.EnsureCapacity(denyCount);

        for (var i = 0; i < denyCount; i++)
        {
            var newId = buffer.ReadInt32();
            var newJob = buffer.ReadString();

            Deny.Add(new CharacterWhitelistRole(newId,newJob));
        }
    }

    public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
    {
        buffer.WriteVariableInt32(Allow.Count);

        foreach (var character in Allow)
        {
            buffer.Write(character.characterId);
            buffer.Write(character.job);
        }

        buffer.WriteVariableInt32(Deny.Count);

        foreach (var character in Deny)
        {
            buffer.Write(character.characterId);
            buffer.Write(character.job);
        }
    }


}
