using System.Globalization;
using Content.Server.Chat.Managers;
using Content.Server.Chat.Systems;
using Content.Shared.Administration;
using Content.Shared._AS.CCVar;
using Content.Shared.Chat;
using Robust.Shared.Configuration;
using Robust.Shared.Console;
using Robust.Shared.Enums;
using Robust.Shared.Random;

namespace Content.Server._AS.Chat.Commands;

[AnyCommand]
internal sealed partial class RollCommand : IConsoleCommand
{
    // If you need any of these values higher than this, no you fucking don't?
    // What kind of rolls are you making that you need more than 1,000 sides? Some 5th dimensional eldritch bullshit?
    // Don't answer that question.
    private const int MaxDice = 50;
    private const int MaxDisplayedDice = 5;
    private const int MaxSides = 1_000;
    private const int MaxModifier = 10_000;

    [Dependency] private IChatManager _chatManager = null!;
    [Dependency] private IConfigurationManager _configuration = null!;
    [Dependency] private IEntityManager _entityManager = null!;
    [Dependency] private IRobustRandom _random = null!;

    public string Command => "roll";
    public string Description => Loc.GetString("roll-command-description");
    public string Help => Loc.GetString("roll-command-help");

    public void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        // DON'T EXPLODE THE SERVER
        if (shell.Player is not { } player)
        {
            shell.WriteError(Loc.GetString("shell-cannot-run-command-from-server"));
            return;
        }

        if (player.Status != SessionStatus.InGame || player.AttachedEntity is not { Valid: true } playerEntity)
            return;

        if (!TryReadArguments(args, out var expression, out var show, out var argumentError))
        {
            var message = Loc.GetString(argumentError);
            shell.WriteError(message);
            _chatManager.DispatchServerMessage(player, message, suppressLog: true);
            return;
        }

        if (!TryParseExpression(expression, out var count, out var sides, out var modifier, out var parseError))
        {
            var message = GetParseError(parseError);
            shell.WriteError(message);
            _chatManager.DispatchServerMessage(player, message, suppressLog: true);
            return;
        }

        if (show && !_configuration.GetCVar(ASCCVars.ChatRollShowEnabled))
        {
            var message = Loc.GetString("roll-command-show-disabled");
            shell.WriteLine(message);
            _chatManager.DispatchServerMessage(player, message, suppressLog: true);
            show = false;
        }

        var rolls = new int[count];
        var total = modifier;
        for (var i = 0; i < count; i++)
        {
            rolls[i] = _random.Next(1, sides + 1);
            total += rolls[i];
        }

        var calculation = FormatCalculation(rolls, sides, modifier);

        if (show)
        {
            var message = Loc.GetString("roll-command-result-public", ("total", total), ("calculation", calculation));
            _entityManager.System<ChatSystem>().TrySendInGameICMessage(playerEntity, message, InGameICChatType.Emote, ChatTransmitRange.Normal, false, shell, player);
            return;
        }

        _chatManager.DispatchServerMessage(player, Loc.GetString("roll-command-result-private", ("total", total), ("calculation", calculation)));
    }

    private static bool TryReadArguments(string[] args, out string expression, out bool show, out string error)
    {
        expression = string.Empty;
        show = false;
        error = string.Empty;

        switch (args.Length)
        {
            case < 1:
                error = "roll-command-missing-expression";
                return false;
            case > 2:
                error = "roll-command-too-many-arguments";
                return false;
        }

        expression = args[0];
        if (args.Length == 1)
            return true;

        show = args[1].Equals("show", StringComparison.OrdinalIgnoreCase);
        if (show)
            return true;

        error = "roll-command-invalid-option";
        return false;
    }

    internal static bool TryParseExpression(string expression, out int count, out int sides, out int modifier, out RollParseError error)
    {
        count = 1;
        sides = 0;
        modifier = 0;
        error = RollParseError.None;

        var span = expression.AsSpan().Trim();
        if (span.IsEmpty)
        {
            error = RollParseError.EmptyExpression;
            return false;
        }

        var dIndex = span.IndexOfAny('d', 'D');
        ReadOnlySpan<char> remainder;
        if (dIndex < 0)
            remainder = span;
        else
        {
            var countSpan = span[..dIndex];
            if (!countSpan.IsEmpty)
            {
                if (!int.TryParse(countSpan, NumberStyles.None, CultureInfo.InvariantCulture, out count))
                {
                    error = RollParseError.InvalidDiceCount;
                    return false;
                }

                if (count is < 1 or > MaxDice)
                {
                    error = RollParseError.DiceCountOutOfRange;
                    return false;
                }
            }

            remainder = span[(dIndex + 1)..];
        }

        var modifierIndex = IndexOfModifier(remainder);
        var sidesSpan = modifierIndex < 0 ? remainder : remainder[..modifierIndex];
        if (!int.TryParse(sidesSpan, NumberStyles.None, CultureInfo.InvariantCulture, out sides))
        {
            error = RollParseError.InvalidSides;
            return false;
        }

        if (sides is < 1 or > MaxSides)
        {
            error = RollParseError.SidesOutOfRange;
            return false;
        }

        if (modifierIndex < 0)
            return true;

        if (!int.TryParse(remainder[modifierIndex..], NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out modifier))
        {
            error = RollParseError.InvalidModifier;
            return false;
        }

        if (modifier is >= -MaxModifier and <= MaxModifier)
            return true;

        error = RollParseError.ModifierOutOfRange;
        return false;
    }

    private static string GetParseError(RollParseError error)
    {
        return error switch
        {
            RollParseError.EmptyExpression => Loc.GetString("roll-command-missing-expression"),
            RollParseError.InvalidDiceCount => Loc.GetString("roll-command-invalid-count"),
            RollParseError.DiceCountOutOfRange => Loc.GetString("roll-command-count-out-of-range", ("maximum", MaxDice)),
            RollParseError.InvalidSides => Loc.GetString("roll-command-invalid-sides"),
            RollParseError.SidesOutOfRange => Loc.GetString("roll-command-sides-out-of-range", ("maximum", MaxSides)),
            RollParseError.InvalidModifier => Loc.GetString("roll-command-invalid-modifier"),
            RollParseError.ModifierOutOfRange => Loc.GetString("roll-command-modifier-out-of-range", ("maximum", MaxModifier)),
            _ => Loc.GetString("roll-command-invalid"),
        };
    }

    private static int IndexOfModifier(ReadOnlySpan<char> value)
    {
        for (var i = 1; i < value.Length; i++)
        {
            if (value[i] is '+' or '-')
                return i;
        }
        return -1;
    }

    internal static string FormatCalculation(int[] rolls, int sides, int modifier)
    {
        if (rolls.Length is 1 or > MaxDisplayedDice)
            return FormatExpression(rolls.Length, sides, modifier);

        var calculation = string.Join(" + ", rolls);
        return modifier switch
        {
            > 0 => $"{calculation} + {modifier}", < 0 => $"{calculation} - {-modifier}", _ => calculation,
        };
    }

    private static string FormatExpression(int count, int sides, int modifier)
    {
        return modifier switch
        {
            > 0 => $"{count}d{sides}+{modifier}", < 0 => $"{count}d{sides}{modifier}", _ => $"{count}d{sides}",
        };
    }

    internal enum RollParseError : byte
    {
        None,
        EmptyExpression,
        InvalidDiceCount,
        DiceCountOutOfRange,
        InvalidSides,
        SidesOutOfRange,
        InvalidModifier,
        ModifierOutOfRange,
    }
}
