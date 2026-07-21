using Robust.Shared.Configuration;

namespace Content.Shared._Wormix.CCVar;

[CVarDefs]
public sealed partial class CCVar
{
    public static readonly CVarDef<string> BanDiscordWebhook =
        CVarDef.Create("discord.ban_webhook", "", CVar.SERVERONLY | CVar.CONFIDENTIAL);
}
