// SPDX-FileCopyrightText: 2026 FelixOnly <62942680+felixonly@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Linq;
using Content.Server.Administration;
using Content.Server.Database;
using Content.Server.Players;
using Content.Shared.Administration;
using Content.Shared.Roles;
using Robust.Server.Player;
using Robust.Shared.Console;
using Robust.Shared.Prototypes;

namespace Content.Server._Wormix.Commands;


// Добавление

[AdminCommand(AdminFlags.Ban)]
public sealed class CharacterAddJobAllowCommand : LocalizedCommands
{
    [Dependency] private readonly JobCharacterWhitelistManager _manager = default!;
    [Dependency] private readonly IPrototypeManager _prototypes = default!;
    [Dependency] private readonly IPlayerLocator _playerLocator = default!;

    public override string Command => "addcharacterjoballow";
    public override string Description => Loc.GetString("cmd-addcharacterallow-desc");

    public override string Help => Loc.GetString("cmd-addcharacterallow-help");

    //character
    //job

    public override async void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        if (args.Length != 2)
        {
            shell.WriteError(Loc.GetString("shell-wrong-arguments-number-need-specific",
                ("properAmount", 2),
                ("currentAmount", args.Length)));
            shell.WriteLine(Help);
            return;
        }

        var characterId = int.Parse(args[0]);
        var job = new ProtoId<JobPrototype>(args[1].Trim());
        if (!_prototypes.TryIndex(job, out var jobPrototype))
        {
            shell.WriteError(Loc.GetString("cmd-job-does-not-exist", ("job", job.Id)));
            shell.WriteLine(Help);
            return;
        }

        var getPlayer = await _manager.FindPlayerByCharacter(characterId);

        var data = await _playerLocator.LookupIdByNameAsync(getPlayer);

        if (data != null)
        {
            var guid = data.UserId;

            _manager.AddCharacterWhitelist(guid, characterId, job, new ProtoId<JobPrototype>("TAssistant"));

            shell.WriteLine(Loc.GetString("cmd-characterjobadded-success"));
            return;
        }

        shell.WriteError(Loc.GetString("cmd-characterjob-player-not-found", ("player", getPlayer)));
    }

    public override CompletionResult GetCompletion(IConsoleShell shell, string[] args)
    {

        if (args.Length == 2)
        {
            return CompletionResult.FromHintOptions(
                _prototypes.EnumeratePrototypes<JobPrototype>().Select(p => p.ID),
                Loc.GetString("cmd-jobwhitelist-hint-job"));
        }

        return CompletionResult.Empty;
    }

}

[AdminCommand(AdminFlags.Ban)]
public sealed class CharacterAddJobDenyCommand : LocalizedCommands
{
    [Dependency] private readonly JobCharacterWhitelistManager _manager = default!;
    [Dependency] private readonly IPrototypeManager _prototypes = default!;
    [Dependency] private readonly IPlayerLocator _playerLocator = default!;


    public override string Command => "addcharacterjobdeny";
    public override string Description => Loc.GetString("cmd-addcharacterdeny-desc");

    public override string Help => Loc.GetString("cmd-addcharacterdeny-help");

    //character
    //job

    public override async void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        if (args.Length != 2)
        {
            shell.WriteError(Loc.GetString("shell-wrong-arguments-number-need-specific",
                ("properAmount", 2),
                ("currentAmount", args.Length)));
            shell.WriteLine(Help);
            return;
        }

        var characterId = int.Parse(args[0]);
        var job = new ProtoId<JobPrototype>(args[1].Trim());
        if (!_prototypes.TryIndex(job, out var jobPrototype))
        {
            shell.WriteError(Loc.GetString("cmd-jobwhitelist-job-does-not-exist", ("job", job.Id)));
            shell.WriteLine(Help);
            return;
        }

        var getPlayer = await _manager.FindPlayerByCharacter(characterId);

        var data = await _playerLocator.LookupIdByNameAsync(getPlayer);
        if (data != null)
        {
            var guid = data.UserId;

            _manager.AddCharacterWhitelist(guid, characterId, new ProtoId<JobPrototype>("TAssistant"), job);

            shell.WriteLine(Loc.GetString("cmd-characterjobadded-success"));
            return;
        }

        shell.WriteError(Loc.GetString("cmd-jobwhitelist-player-not-found", ("player", getPlayer)));
    }

    public override CompletionResult GetCompletion(IConsoleShell shell, string[] args)
    {
        if (args.Length == 2)
        {
            return CompletionResult.FromHintOptions(
                _prototypes.EnumeratePrototypes<JobPrototype>().Select(p => p.ID),
                Loc.GetString("cmd-jobwhitelist-hint-job"));
        }

        return CompletionResult.Empty;
    }

}

// Удаление

[AdminCommand(AdminFlags.Ban)]
public sealed class CharacterRemoveJobAllowCommand : LocalizedCommands
{
    [Dependency] private readonly JobCharacterWhitelistManager _manager = default!;
    [Dependency] private readonly IPrototypeManager _prototypes = default!;
    [Dependency] private readonly IPlayerLocator _playerLocator = default!;


    public override string Command => "remcharacterjoballow";
    public override string Description => Loc.GetString("cmd-remcharacterjoballow-desc");

    public override string Help => Loc.GetString("cmd-remcharacterjoballow-help");

    //character
    //job

    public override async void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        if (args.Length != 2)
        {
            shell.WriteError(Loc.GetString("shell-wrong-arguments-number-need-specific",
                ("properAmount", 2),
                ("currentAmount", args.Length)));
            shell.WriteLine(Help);
            return;
        }

        var characterId = int.Parse(args[0]);
        var job = new ProtoId<JobPrototype>(args[1].Trim());
        if (!_prototypes.TryIndex(job, out var jobPrototype))
        {
            shell.WriteError(Loc.GetString("cmd-jobwhitelist-job-does-not-exist", ("job", job.Id)));
            shell.WriteLine(Help);
            return;
        }

        var getPlayer = await _manager.FindPlayerByCharacter(characterId);

        var data = await _playerLocator.LookupIdByNameAsync(getPlayer);
        if (data != null)
        {
            var guid = data.UserId;

            _manager.RemoveWhitelist(guid, characterId, job, new ProtoId<JobPrototype>("TAssistant"));

            shell.WriteLine(Loc.GetString("cmd-characterjobremoved-success"));

            return;
        }

        shell.WriteError(Loc.GetString("cmd-jobwhitelist-player-not-found", ("player", getPlayer)));
    }

    public override CompletionResult GetCompletion(IConsoleShell shell, string[] args)
    {
        if (args.Length == 2)
        {
            return CompletionResult.FromHintOptions(
                _prototypes.EnumeratePrototypes<JobPrototype>().Select(p => p.ID),
                Loc.GetString("cmd-jobwhitelist-hint-job"));
        }

        return CompletionResult.Empty;
    }

}

[AdminCommand(AdminFlags.Ban)]
public sealed class CharacterRemoveJobDenyCommand : LocalizedCommands
{
    [Dependency] private readonly JobCharacterWhitelistManager _manager = default!;
    [Dependency] private readonly IPrototypeManager _prototypes = default!;
    [Dependency] private readonly IPlayerLocator _playerLocator = default!;


    public override string Command => "remcharacterjobdeny";
    public override string Description => Loc.GetString("cmd-remcharacterjobdeny-desc");

    public override string Help => Loc.GetString("cmd-remcharacterjobdeny-help");

    //character
    //job

    public override async void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        if (args.Length != 2)
        {
            shell.WriteError(Loc.GetString("shell-wrong-arguments-number-need-specific",
                ("properAmount", 2),
                ("currentAmount", args.Length)));
            shell.WriteLine(Help);
            return;
        }

        var characterId = int.Parse(args[0]);
        var job = new ProtoId<JobPrototype>(args[1].Trim());
        if (!_prototypes.TryIndex(job, out var jobPrototype))
        {
            shell.WriteError(Loc.GetString("cmd-jobwhitelist-job-does-not-exist", ("job", job.Id)));
            shell.WriteLine(Help);
            return;
        }

        var getPlayer = await _manager.FindPlayerByCharacter(characterId);

        var data = await _playerLocator.LookupIdByNameAsync(getPlayer);
        if (data != null)
        {
            var guid = data.UserId;

            _manager.RemoveWhitelist(guid, characterId, new ProtoId<JobPrototype>("TAssistant"), job);

            shell.WriteLine(Loc.GetString("cmd-characterjobremoved-success"));
            return;
        }

        shell.WriteError(Loc.GetString("cmd-jobwhitelist-player-not-found", ("player", getPlayer)));
    }

    public override CompletionResult GetCompletion(IConsoleShell shell, string[] args)
    {

        if (args.Length == 2)
        {
            return CompletionResult.FromHintOptions(
                _prototypes.EnumeratePrototypes<JobPrototype>().Select(p => p.ID),
                Loc.GetString("cmd-jobwhitelist-hint-job"));
        }

        return CompletionResult.Empty;
    }

}

// Получение списков

[AdminCommand(AdminFlags.Ban)]
public sealed class CharacterShowJobWhitelistCommand : LocalizedCommands
{
    [Dependency] private readonly JobCharacterWhitelistManager _manager = default!;
    [Dependency] private readonly IPlayerManager _players = default!;


    public override string Command => "lscharacterjobwhitelist";
    public override string Description => Loc.GetString("cmd-lscharacterjobwhitelist-desc");

    public override string Help => Loc.GetString("cmd-lscharacterjobwhitelist-help");

    //character

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

        var characterId = int.Parse(args[0]);

        shell.WriteLine("---РАЗРЕШЕНИЯ---");

        var dbAllowed = await _manager.GetAllCharacterAllowed(characterId);

        if (dbAllowed.Any())
        {
            foreach (var deny in dbAllowed)
            {
                if (deny == "TAssistant")
                    continue;

                shell.WriteLine(deny);
            }
        }

        shell.WriteLine("---ЗАПРЕТЫ---");

        var dbDenies = await _manager.GetAllCharacterDenies(characterId);

        if (dbDenies.Any())
        {
            foreach (var deny in dbDenies)
            {
                if (deny == "TAssistant")
                    continue;

                shell.WriteLine(deny);
            }
        }

        shell.WriteError("Не найдено");
    }


}

[AdminCommand(AdminFlags.Ban)]
public sealed class GetListPlayersCharacters : LocalizedCommands
{
    [Dependency] private readonly IServerDbManager _db = default!;
    [Dependency] private readonly IPlayerLocator _playerLocator = default!;
    [Dependency] private readonly IPlayerManager _players = default!;

    public override string Command => "lsplayercharacters";

    public override string Description => Loc.GetString("cmd-lsplayercharacters-desc");

    public override string Help => Loc.GetString("cmd-lsplayercharacters-help");

    //player

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

        var data = await _playerLocator.LookupIdByNameAsync(player);
        if (data != null)
        {
            var guid = data.UserId;

            var characters = await _db.GetPlayerCharacters(guid);

            foreach (var character in characters)
            {
                shell.WriteLine($"ID: {character.Id} NAME: {character.CharacterName}");
            }


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



