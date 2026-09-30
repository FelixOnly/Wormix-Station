using Content.Server.Administration;
using Content.Shared.Administration;
using Robust.Shared.Console;
using Content.Server.EUI;
using System.Linq;
using Content.Server._Wormix.PlayerCharacterList;
using Content.Server._Wormix.Players;
using Content.Server.Players;
using Content.Shared._Wormix;
using Robust.Server.Player;

namespace Content.Server._Wormix.Commands;

[AdminCommand(AdminFlags.Ban)]
public sealed class PlayerCharactersPanelCommand : LocalizedCommands
{
    [Dependency] private readonly IPlayerLocator _locator = default!;
    [Dependency] private readonly EuiManager _euis = default!;
    [Dependency] private readonly IPlayerManager _players = default!;

    public override string Command => "playercharacterspanel";

    public override async void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        if (args.Length != 1)
        {
            shell.WriteError(Loc.GetString("shell-wrong-arguments-number-need-specific",
                ("properAmount", 1),
                ("currentAmount", args.Length)));
            shell.WriteLine(Help);
            return;
        }


        var player = args[0].Trim();

        var data = await _locator.LookupIdByNameAsync(player);
        if (data != null)
        {

            var ui  = new PlayerCharacterListEui();

            if (shell.Player != null)
                _euis.OpenEui(ui, shell.Player);

            ui.SetPlayer(data.UserId, data.Username);
            return;
        }

        shell.WriteError(Loc.GetString("cmd-jobwhitelist-player-not-found", ("player", player)));
    }

    public override CompletionResult GetCompletion(IConsoleShell shell, string[] args)
    {
        if (args.Length == 1)
        {
            return CompletionResult.FromHintOptions(
                _players.Sessions.Select(s => s.Name),
                Loc.GetString("cmd-jobwhitelist-hint-player"));
        }

        return CompletionResult.Empty;
    }
}

[AdminCommand(AdminFlags.Ban)]
public sealed class CharacterWhitelistPanelCommand : LocalizedCommands
{
    [Dependency] private readonly JobCharacterWhitelistManager _manager = default!;
    [Dependency] private readonly IPlayerManager _players = default!;
    [Dependency] private readonly EuiManager _euis = default!;

    public override string Command => "characterwhitelistpanel";

    public override async void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        if (args.Length != 1)
        {
            shell.WriteError(Loc.GetString("shell-wrong-arguments-number-need-specific",
                ("properAmount", 1),
                ("currentAmount", args.Length)));
            shell.WriteLine(Help);
            return;
        }

        var id = Int32.Parse(args[0].Trim());

        var restrictions = _manager.GetAllCharacterRestrictions(id);

        var charName = await _manager.GetCharacterName(id);

        var ui  = new CharacterWhitelistEui(new SharedCharacter(id, charName), restrictions);

        if (shell.Player != null)
            _euis.OpenEui(ui, shell.Player);

    }

    public override CompletionResult GetCompletion(IConsoleShell shell, string[] args)
    {

        return CompletionResult.Empty;
    }
}
