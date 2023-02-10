using Microsoft.Extensions.Logging;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MetaFac.TinyCLI
{
    public abstract class CommandsBase
    {
        private readonly Dictionary<string, CommandsBase> _subCommands = new Dictionary<string, CommandsBase>();
        private readonly Dictionary<string, CmdBase> _commands = new Dictionary<string, CmdBase>();
        private readonly InternalLogger _logger;
        protected ILogger Logger => _logger;

        public string Name { get; }
        public string Help { get; }
        public CmdOptions Options { get; }

        protected CommandsBase(string name, string help, CmdOptions? options = null)
        {
            Name = name;
            Help = help;
            Options = options ?? new CmdOptions();
            _logger = new InternalLogger(Options.LogOptions);
        }

        protected bool HelpRequested(string[] args)
        {
            if (args is null || args.Length == 0) return false;
            var stringComparison = Options.CaseSensitiveTags ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
            return args[0] == "-?" || string.Equals(args[0], "--help", stringComparison);
        }

        private Table GetCommandListAsTable()
        {
            var table = new Table();
            table.Border = TableBorder.None;
            table.AddColumn("Command");
            table.AddColumn("Description");
            table.HideHeaders();
            foreach (var command in _subCommands)
            {
                table.AddRow($"[cyan]{command.Key}[/]", command.Value.Help);
            }
            foreach (var command in _commands)
            {
                table.AddRow($"[cyan]{command.Key}[/]", command.Value.Help);
            }
            return table;
        }

        private void WriteAll(string name, string help, int exitCode, Exception? error)
        {
            var table = new Table()
                .SquareBorder()
                .BorderColor(Color.Green);
            var col1 = new TableColumn(name);
            col1.Footer = new Markup("Credits");
            var col2 = new TableColumn(help);
            col2.Width = 100;
            col2.Footer = new Markup($"UI powered by [green]https://www.nuget.org/packages/MetaFac.TinyCLI[/] and [green]https://www.nuget.org/packages/Spectre.Console[/]");
            table.AddColumn(col1);
            table.AddColumn(col2);
            if (error is not null)
            {
                table.AddRow(new Markup("Error"), error.GetRenderable(ExceptionFormats.ShortenEverything));
                table.AddRow("", "");
            }
            table.AddRow(new Markup("Commands"), GetCommandListAsTable());
            table.AddRow("", "");
            table.AddRow("", "For additional help about a command, use the '--help' option.");
            if (!Options.DisplayFlags.HasFlag(DisplayFlags.HideExitCode))
            {
                table.AddRow("", "");
                table.AddRow(new Markup("ExitCode"), MarkupHelper.ExitCode(exitCode));
            }
            if (Options.DisplayFlags.HasFlag(DisplayFlags.HideCredits))
                table.HideFooters();
            else
                table.ShowFooters();
            if (Options.DisplayFlags.HasFlag(DisplayFlags.HideTitle))
                table.HideHeaders();
            else
                table.ShowHeaders();
            AnsiConsole.Write(table);
        }

        public async ValueTask<int> Run(string[] args)
        {
            int exitCode = 0;
            if (args is null || args.Length == 0 || HelpRequested(args))
            {
                WriteAll(Name, Help, exitCode, null);
            }
            else
            {
                try
                {
                    string command = args[0];
                    var remainingArgs = args.Skip(1).ToArray();

                    if (_subCommands.TryGetValue(command, out CommandsBase? subCommand))
                    {
                        exitCode = await subCommand.Run(remainingArgs);
                    }
                    else if (_commands.TryGetValue(command, out CmdBase? cmd))
                    {
                        exitCode = await cmd.Run(remainingArgs, _logger);
                    }
                    else
                        throw new ArgumentException($"Command not found: {command}");
                }
                catch (Exception e)
                {
                    exitCode = Options.AbnormalExitCode;
                    WriteAll(Name, Help, exitCode, e);
                }
            }
            return exitCode;
        }

        protected void AddSubCommands(CommandsBase subCommand)
        {
            _subCommands.Add(subCommand.Name, subCommand);
        }

        protected void AddCommand<TArg1, TResult>(
            string command, string help,
            Arg<TArg1> argDef1,
            Func<TArg1, ValueTask<TResult>> action)
        {
            _commands.Add(command, new Cmd<TArg1, TResult>(command, help, argDef1, action, Options));
        }

        protected void AddCommand<TArg1, TArg2, TResult>(
            string command, string help,
            Arg<TArg1> argDef1,
            Arg<TArg2> argDef2,
            Func<TArg1, TArg2, ValueTask<TResult>> action)
        {
            _commands.Add(command, new Cmd<TArg1, TArg2, TResult>(
                command, help, argDef1, argDef2, action, Options));
        }

        protected void AddCommand<TArg1, TArg2, TArg3, TResult>(
            string command, string help,
            Arg<TArg1> argDef1,
            Arg<TArg2> argDef2,
            Arg<TArg3> argDef3,
            Func<TArg1, TArg2, TArg3, ValueTask<TResult>> action)
        {
            _commands.Add(command, new Cmd<TArg1, TArg2, TArg3, TResult>(
                command, help, argDef1, argDef2, argDef3, action, Options));
        }

        protected void AddCommand<TArg1, TArg2, TArg3, TArg4, TResult>(
            string command, string help,
            Arg<TArg1> argDef1,
            Arg<TArg2> argDef2,
            Arg<TArg3> argDef3,
            Arg<TArg4> argDef4,
            Func<TArg1, TArg2, TArg3, TArg4, ValueTask<TResult>> action)
        {
            _commands.Add(command, new Cmd<TArg1, TArg2, TArg3, TArg4, TResult>(
                command, help, argDef1, argDef2, argDef3, argDef4, action, Options));
        }

        protected void AddCommand<TArg1, TArg2, TArg3, TArg4, TArg5, TResult>(
            string command, string help,
            Arg<TArg1> argDef1,
            Arg<TArg2> argDef2,
            Arg<TArg3> argDef3,
            Arg<TArg4> argDef4,
            Arg<TArg5> argDef5,
            Func<TArg1, TArg2, TArg3, TArg4, TArg5, ValueTask<TResult>> action)
        {
            _commands.Add(command, new Cmd<TArg1, TArg2, TArg3, TArg4, TArg5, TResult>(
                command, help, argDef1, argDef2, argDef3, argDef4, argDef5, action, Options));
        }

        protected void AddCommand<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TResult>(
            string command, string help,
            Arg<TArg1> argDef1,
            Arg<TArg2> argDef2,
            Arg<TArg3> argDef3,
            Arg<TArg4> argDef4,
            Arg<TArg5> argDef5,
            Arg<TArg6> argDef6,
            Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, ValueTask<TResult>> action)
        {
            _commands.Add(command, new Cmd<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TResult>(
                command, help, argDef1, argDef2, argDef3, argDef4, argDef5, argDef6, action, Options));
        }

        protected void AddCommand<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TResult>(
            string command, string help,
            Arg<TArg1> argDef1,
            Arg<TArg2> argDef2,
            Arg<TArg3> argDef3,
            Arg<TArg4> argDef4,
            Arg<TArg5> argDef5,
            Arg<TArg6> argDef6,
            Arg<TArg7> argDef7,
            Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, ValueTask<TResult>> action)
        {
            _commands.Add(command, new Cmd<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TResult>(
                command, help, argDef1, argDef2, argDef3, argDef4, argDef5, argDef6, argDef7, action, Options));
        }

        protected void AddCommand<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TResult>(
            string command, string help,
            Arg<TArg1> argDef1,
            Arg<TArg2> argDef2,
            Arg<TArg3> argDef3,
            Arg<TArg4> argDef4,
            Arg<TArg5> argDef5,
            Arg<TArg6> argDef6,
            Arg<TArg7> argDef7,
            Arg<TArg8> argDef8,
            Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, ValueTask<TResult>> action)
        {
            _commands.Add(command, new Cmd<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TResult>(
                command, help, argDef1, argDef2, argDef3, argDef4, argDef5, argDef6, argDef7, argDef8, action, Options));
        }

        protected void AddCommand<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TResult>(
            string command, string help,
            Arg<TArg1> argDef1,
            Arg<TArg2> argDef2,
            Arg<TArg3> argDef3,
            Arg<TArg4> argDef4,
            Arg<TArg5> argDef5,
            Arg<TArg6> argDef6,
            Arg<TArg7> argDef7,
            Arg<TArg8> argDef8,
            Arg<TArg9> argDef9,
            Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, ValueTask<TResult>> action)
        {
            _commands.Add(command, new Cmd<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TResult>(
                command, help, argDef1, argDef2, argDef3, argDef4, argDef5, argDef6, argDef7, argDef8, argDef9, action, Options));
        }
    }
}