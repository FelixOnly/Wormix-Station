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





        //
        //
        //
        // var allowCharacterIdCount = buffer.ReadVariableInt32();
        // var allowCharacterIDs = new List<int>(allowCharacterIdCount);
        //
        // for (var i = 0; i < allowCharacterIdCount; i++)
        //     allowCharacterIDs.Add(buffer.ReadInt32());
        //
        // var allowCharacterRoleCount = buffer.ReadVariableInt32();
        // var allowCharacterRoles = new List<string>(allowCharacterRoleCount);
        //
        // for (var i = 0; i < allowCharacterRoleCount; i++)
        //     allowCharacterRoles.Add(buffer.ReadString());
        //
        // Allow.Clear();
        // Allow.EnsureCapacity(allowCharacterIdCount);
        //
        // for (var i = 0; i < allowCharacterIdCount; i++)
        //     Allow.Add(new CharacterWhitelistRole(allowCharacterIDs[i], allowCharacterRoles[i]));
        //
        //
        // var denyCharacterIdCount = buffer.ReadVariableInt32();
        // var denyCharacterIDs = new List<int>(denyCharacterIdCount);
        //
        // for (var i = 0; i < denyCharacterIdCount; i++)
        //     denyCharacterIDs.Add(buffer.ReadInt32());
        //
        // var denyCharacterRoleCount = buffer.ReadVariableInt32();
        // var denyCharacterRoles = new List<string>(denyCharacterRoleCount);
        //
        // for (var i = 0; i < denyCharacterRoleCount; i++)
        //     denyCharacterRoles.Add(buffer.ReadString());
        //
        // Deny.Clear();
        // Deny.EnsureCapacity(denyCharacterIdCount);
        //
        // for (var i = 0; i < denyCharacterIdCount; i++)
        //     Deny.Add(new CharacterWhitelistRole(denyCharacterIDs[i], denyCharacterRoles[i]));
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



        // // Упаковка
        //
        // List<int> AllowCharacterIDs = new List<int>();
        // List<string> AllowCharacterRoles = new List<string>();
        //
        // foreach (var allow in Allow)
        // {
        //     AllowCharacterIDs.Add(allow.characterId);
        //     AllowCharacterRoles.Add(allow.job);
        // }
        //
        // List<int> DenyCharacterIDs = new List<int>();
        // List<string> DenyCharacterRoles = new List<string>();
        //
        // foreach (var deny in Deny)
        // {
        //     DenyCharacterIDs.Add(deny.characterId);
        //     DenyCharacterRoles.Add(deny.job);
        // }
        //
        // // Упаковка
        //
        //
        // // ALLOW
        // buffer.WriteVariableInt32(AllowCharacterIDs.Count);
        //
        // foreach (var ids in AllowCharacterIDs)
        // {
        //     buffer.Write(ids);
        // }
        //
        // buffer.WriteVariableInt32(AllowCharacterRoles.Count);
        //
        // foreach (var roles in AllowCharacterRoles)
        // {
        //     buffer.Write(roles);
        // }
        //
        //
        // //DENY
        //
        // buffer.WriteVariableInt32(DenyCharacterIDs.Count);
        //
        // foreach (var ids in DenyCharacterIDs)
        // {
        //     buffer.Write(ids);
        // }
        //
        // buffer.WriteVariableInt32(DenyCharacterRoles.Count);
        //
        // foreach (var roles in DenyCharacterRoles)
        // {
        //     buffer.Write(roles);
        // }
    }


}
