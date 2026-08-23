// SPDX-FileCopyrightText: 2026 FelixOnly <62942680+felixonly@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Robust.Shared.Configuration;

namespace Content.Shared._Wormix.CCVar;

[CVarDefs]
public sealed partial class CCVar
{
    public static readonly CVarDef<string> BanDiscordWebhook =
        CVarDef.Create("discord.ban_webhook", "", CVar.SERVERONLY | CVar.CONFIDENTIAL);

    public static readonly CVarDef<string> FaxDiscordWebhook =
        CVarDef.Create("discord.fax_webhook", "", CVar.SERVERONLY | CVar.CONFIDENTIAL);
}
