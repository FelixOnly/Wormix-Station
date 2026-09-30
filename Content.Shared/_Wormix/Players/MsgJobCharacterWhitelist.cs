using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared._Wormix.Players;

public sealed class MsgJobCharacterWhitelist: NetMessage
{
    public override MsgGroups MsgGroup => MsgGroups.EntityEvent;

    public List<CharacterWhitelistRole> CharactersWhitelist = new();


    public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
    {
        var restrictionCount = buffer.ReadVariableInt32();
        CharactersWhitelist.EnsureCapacity(restrictionCount);

        for (var i = 0; i < restrictionCount; i++)
        {
            var newId = buffer.ReadInt32();
            var newJob = buffer.ReadString();
            var newRestriction = buffer.ReadBoolean();

            CharactersWhitelist.Add(new CharacterWhitelistRole(newId,newJob, newRestriction));
        }
    }

    public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
    {
        buffer.WriteVariableInt32(CharactersWhitelist.Count);

        foreach (var character in CharactersWhitelist)
        {
            buffer.Write(character.characterId);
            buffer.Write(character.job);
            buffer.Write(character.isRestricted);
        }
    }
}
