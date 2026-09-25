using Robust.Shared.Configuration;

namespace Content.Shared._AS.CCVar;

[CVarDefs]
public sealed class ASCCVars
{
    /// <summary>
    /// Whether players may make the result of /roll visible to nearby players with the "show" argument.
    /// Private rolls are always available.
    /// </summary>
    public static readonly CVarDef<bool> ChatRollShowEnabled = CVarDef.Create("as.chat.roll_show_enabled", true, CVar.SERVERONLY);
}
