using Microsoft.Extensions.Logging;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniCLI
{
    public abstract class CmdBase<TResult> : CmdBase
    {
        protected CmdBase(string name, string help, CmdOptions? options = null) : base(name, help, options)
        {
        }

        private void WriteAll(string name, string help, InternalLogger? logger, params ArgBase[] argDefs)
        {
            var table = new Table()
                .SquareBorder()
                .BorderColor(Color.Green);
            var col1 = new TableColumn(name);
            col1.Footer = new Markup("Credits");
            var col2 = new TableColumn(help);
            col2.Width = 100;
            col2.Footer = new Markup($"UI powered by [green]https://www.nuget.org/packages/MiniCLI[/] and [green]https://www.nuget.org/packages/Spectre.Console[/]");
            table.AddColumn(col1);
            table.AddColumn(col2);
            if (!_options.DisplayFlags.HasFlag(DisplayFlags.HideInputs) && _inputs.Count > 0)
            {
                table.AddRow(new Markup("Inputs"), GetInputsAsTable());
                table.AddRow("", "");
            }
            if (logger is not null && !_options.DisplayFlags.HasFlag(DisplayFlags.HideLogs))
            {
                if (logger.EntryCount > 0)
                {
                    table.AddRow(new Markup("Log"), logger. GetEntriesAsTable());
                    table.AddRow("", "");
                }
            }
            switch (_state)
            {
                case CmdState.Success:
                    switch (_result)
                    {
                        case null:
                            table.AddRow("Result", $"[cyan](null)[/] {typeof(TResult).Name}");
                            break;
                        case string s:
                            table.AddRow("Result", $"[cyan]{s}[/] String({s.Length})");
                            break;
                        default:
                            table.AddRow("Result", $"[cyan]{_result}[/] {typeof(TResult).Name}");
                            break;
                    }
                    break;
                case CmdState.Faulted:
                    table.AddRow(new Markup("Error"), _error!.GetRenderable(ExceptionFormats.ShortenEverything));
                    break;
                default:
                    table.AddRow(new Markup("Usage"), GetUsage(argDefs));
                    table.AddRow("", "");
                    table.AddRow(new Markup("Help"), GetCommandHelpAsTable(argDefs));
                    break;
            }
            if (!_options.DisplayFlags.HasFlag(DisplayFlags.HideExitCode))
            {
                table.AddRow("", "");
                table.AddRow(new Markup("ExitCode"), MarkupHelper.ExitCode(_exitCode));
            }
            if (_options.DisplayFlags.HasFlag(DisplayFlags.HideCredits))
                table.HideFooters();
            else
                table.ShowFooters();
            if (_options.DisplayFlags.HasFlag(DisplayFlags.HideTitle))
                table.HideHeaders();
            else
                table.ShowHeaders();
            AnsiConsole.Write(table);
        }

        protected async ValueTask<int> RunAction(InternalLogger? logger, string[] args, string commandName, string help, Func<ValueTask<TResult>> action, params ArgBase[] argDefs)
        {
            if (!HelpRequested(args))
            {
                try
                {
                    TResult result = await action().ConfigureAwait(false);
                    SetResult(result);
                }
                catch (Exception e)
                {
                    SetError(e, _options.AbnormalExitCode);
                }
            }
            WriteAll(commandName, help, logger, argDefs);
            return _exitCode;
        }

        private enum CmdState
        {
            NotRun,
            Success,
            Faulted,
        }
        private CmdState _state = CmdState.NotRun;
        private TResult? _result;
        private Exception? _error;
        protected void SetResult(TResult result, int exitCode = 0)
        {
            _state = CmdState.Success;
            _result = result;
            _exitCode = exitCode;
        }
        protected void SetError(Exception error, int exitCode = -1)
        {
            _state = CmdState.Faulted;
            _error = error;
            _exitCode = exitCode;
        }
    }
    public abstract class CmdBase
    {
        protected readonly List<ArgValue> _inputs = new List<ArgValue>();
        protected readonly CmdOptions _options;
        public string Name { get; }
        public string Help { get; }

        protected CmdBase(string name, string help, CmdOptions? options = null)
        {
            Name = name;
            Help = help;
            _options = options ?? new CmdOptions();
        }

        protected abstract ValueTask<int> OnRun(InternalLogger? logger, string[] args);
        protected int _exitCode = 0;
        public async ValueTask<int> Run(string[] args, InternalLogger? logger = null)
        {
            _exitCode = await OnRun(logger, args);
            return _exitCode;
        }

        protected bool HelpRequested(string[] args)
        {
            if (args is null || args.Length == 0) return false;
            var stringComparison = _options.CaseSensitiveTags ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
            return args[0] == "-?" || string.Equals(args[0], "--help", stringComparison);
        }

        protected Markup GetUsage(ArgBase[] argDefs)
        {
            StringBuilder usage = new StringBuilder();
            usage.Append($"[[-?|--help ]]");
            foreach (var argDef in argDefs)
            {
                usage.Append(" ");
                if (!argDef.Required)
                {
                    usage.Append("[[");
                }
                usage.Append($"-{argDef.Tag1}|--{argDef.Tag2} <value>");
                if (!argDef.Required)
                {
                    usage.Append("]]");
                }
            }
            return new Markup(usage.ToString());
        }

        protected Table GetInputsAsTable()
        {
            var table = new Table();
            table.Border = TableBorder.None;
            table.AddColumn("Name");
            table.AddColumn("Value");
            table.AddColumn("Type");
            table.HideHeaders();
            foreach (var input in _inputs)
            {
                var value = input.GetArgValue();
                switch (value)
                {
                    case null:
                        table.AddRow($"-{input.ArgDef.Tag1}|--{input.ArgDef.Tag2}", $"[cyan](null)[/]", $"{input.ArgDef.ArgType.Name}");
                        break;
                    case string s:
                        table.AddRow($"-{input.ArgDef.Tag1}|--{input.ArgDef.Tag2}", $"[cyan]{s}[/]", $"String({s.Length})");
                        break;
                    default:
                        table.AddRow($"-{input.ArgDef.Tag1}|--{input.ArgDef.Tag2}", $"[cyan]{value}[/]", $"{input.ArgDef.ArgType.Name}");
                        break;
                }
            }
            return table;
        }

        protected Table GetCommandHelpAsTable(ArgBase[] argDefs)
        {
            var table = new Table();
            table.Border = TableBorder.None;
            table.AddColumn("T1|Tag2");
            table.AddColumn("Required");
            table.AddColumn("String");
            table.AddColumn("Description");
            table.HideHeaders();
            foreach (var argDef in argDefs)
            {
                string description = $"{argDef.Description}";
                if (!argDef.Required) description += $". Default: {argDef.UntypedDefaultValue?.ToString() ?? ""}";
                table.AddRow(
                    $"-{argDef.Tag1}|--{argDef.Tag2}",
                    argDef.Required ? "Required" : "Optional",
                    argDef.ArgType.Name,
                    description);
            }
            table.AddRow($"-?|--help", "", "", "Displays this help.");
            return table;
        }

        protected void SaveValue<T>(string? input, T value, Arg<T> argDef)
        {
            _inputs.Add(new ArgValue<T>(input, value, argDef));
        }

        protected (T, List<string> unusedArgs) GetValue<T>(string[] args, Arg<T> argDef)
        {
            var stringComparison = _options.CaseSensitiveTags ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
            List<string> unusedArgs = new List<string>();
            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i];
                if (string.Equals(arg, $"-{argDef.Tag1}", stringComparison) ||
                    string.Equals(arg, $"--{argDef.Tag2}", stringComparison))
                {
                    string? argValue = null;
                    try
                    {
                        argValue = args[i + 1];
                        unusedArgs.AddRange(args.Skip(i + 2));
                        T value = argDef.Parser(argValue);
                        SaveValue(argValue, value, argDef);
                        return (value, unusedArgs);
                    }
                    catch (Exception inner)
                    {
                        throw new ArgumentException($"Value '{argValue}' is invalid or missing", $"--{argDef.Tag2}", inner);
                    }
                }
                else
                {
                    unusedArgs.Add(arg);
                }
            }
            if (argDef.Required)
            {
                throw new ArgumentException($"Required argument is missing", argDef.Tag2);
            }
            else
            {
                SaveValue(null, argDef.TypedDefaultValue, argDef);
                return (argDef.TypedDefaultValue, unusedArgs);
            }
        }

        protected void CheckExtraArguments(List<string> extraArgs)
        {
            if (!_options.AllowExtraArguments && extraArgs.Count > 0)
            {
                throw new ArgumentException($"Extra arguments not allowed: {string.Join(" ", extraArgs)}");
            }
        }


    }
}