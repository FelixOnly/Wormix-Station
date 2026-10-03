using System.Linq;
using Content.Server.Administration;
using Content.Server.Administration.Logs;
using Content.Shared._Wormix.MessageOverlay;
using Content.Shared.Administration;
using Robust.Shared.Console;
using Robust.Shared.Player;

// Goobstation - Admin Log
using Content.Shared.Database;
using Content.Server.Administration.Logs;


namespace Content.Server._Wormix.MessageOverlay;

[AdminCommand(AdminFlags.Fun)]
public sealed class MessageOverlayCommands : IConsoleCommand
{
    [Dependency] private readonly IEntityManager _entityManager = default!;
    [Dependency] private readonly Robust.Server.Player.IPlayerManager _playerManager = default!;

    // Goobstation - Admin Log
    [Dependency] private readonly IAdminLogManager _adminLog = default!;

    public string Command => "overlaymessage";
    public string Description => "Большой-текст Маленький-текст (Желательно пишите в кавычках)";
    public string Help => "[Большой текст] [Маленький текст] Оставьте пусто если для всех, либо перечислите игроков как playglobalsound";
    public void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        NewOverlayMessage newMessage = new NewOverlayMessage(args[0], args[1]);
        Filter filter;

        var playerName = shell.Player as ICommonSession;


        if (args.Length == 2)
        {
            filter = Filter.Empty().AddAllPlayers(_playerManager);

            _adminLog.Add(LogType.AdminCommands,
                LogImpact.High,
                $"{playerName} made overlay message {args[0]} {args[1]} to all players.");

            _entityManager.System<MessageOverlaySystem>().CallMessage(newMessage, filter);
        }
        else
        {
            filter = Filter.Empty();

            for (var i = 2; i < args.Length; i++)
            {
                var username = args[i];

                if (!_playerManager.TryGetSessionByUsername(username, out var session))
                {
                    shell.WriteError(Loc.GetString("play-global-sound-command-player-not-found", ("username", username)));
                    continue;
                }

                filter.AddPlayer(session);

                _adminLog.Add(LogType.AdminCommands,
                    LogImpact.High,
                    $"{playerName} made overlay message {args[0]} {args[1]} for {session.Name}.");
            }
        }

        _entityManager.System<MessageOverlaySystem>().CallMessage(newMessage, filter);



    }

    public CompletionResult GetCompletion(IConsoleShell shell, string[] args)
    {

        if (args.Length > 2)
        {
            var options = _playerManager.Sessions.Select<ICommonSession, string>(c => c.Name);
            return CompletionResult.FromHintOptions(
                options,
                Loc.GetString("play-global-sound-command-arg-usern", ("user", args.Length - 2)));
        }

        return CompletionResult.Empty;
    }
}


