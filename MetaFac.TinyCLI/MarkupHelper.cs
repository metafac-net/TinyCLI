using Spectre.Console;
using System.Text;

namespace MetaFac.TinyCLI
{
    internal static class MarkupHelper
    {
        public static Markup ExitCode(int exitCode)
        {
            StringBuilder result = new StringBuilder();
            if (exitCode == 0)
                result.Append("[green]");
            else if (exitCode > 0)
                result.Append("[cyan]");
            else
                result.Append("[red]");
            result.Append($"{exitCode}[/] [grey](0x{exitCode:X8})[/]");
            return new Markup(result.ToString());
        }
    }
}