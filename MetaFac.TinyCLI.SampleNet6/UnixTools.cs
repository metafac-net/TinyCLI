// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.Logging;

namespace MetaFac.TinyCLI.SampleNet6
{
    internal class InnerCommands1 : CommandsBase
    {
        public InnerCommands1(CmdOptions options) : base("inner1", "Inner 1 commands", options)
        {
        }
    }
    internal class InnerCommands2 : CommandsBase
    {
        public InnerCommands2(CmdOptions options) : base("inner2", "Inner 2 commands", options)
        {
            AddCommand("bye", "Returns a good-bye message",
                new Arg<string>("n", "name", "A name", s => s, "Friend"),
                (s) => new ValueTask<string>($"Goodbye, {s}."));
        }
    }
    internal class OuterCommands : CommandsBase
    {
        private ValueTask<string> Greeting(string name, int age)
        {
            Logger.LogDebug("Name={name}", name);
            Logger.LogDebug("Age={age}", age);
            if (age <= 0)
                throw new ArgumentOutOfRangeException(nameof(age), age, "Must be > 0");
            string result = age switch
            {
                < 25 => $"Yo, wassup {name}?",
                > 65 => $"Greetings, Sr. {name}.",
                _ => $"Hello, {name}!",
            };
            Logger.LogDebug("Greeting={result}", result);
            return new ValueTask<string>(result);
        }

        public OuterCommands(CmdOptions options) : base("outer", "Outer commands", options)
        {
            AddSubCommands(new InnerCommands1(options));
            AddSubCommands(new InnerCommands2(options));
            AddCommand("hello", "Returns an age relevant greeting message",
                new Arg<string>("n", "name", "A name", s => s, "Friend"),
                new Arg<int>("a", "age", "Age in years", int.Parse, 35),
                Greeting);
        }
    }
    internal class UnixTools : CommandsBase
    {
        private ValueTask<DateTime> UnixDateToDateTime(double unixDate)
        {
            return new ValueTask<DateTime>(DateTime.UnixEpoch.AddDays(unixDate).ToLocalTime());
        }

        private ValueTask<double> DateTimeToUnixDate(DateTime date)
        {
            var result = date.ToUniversalTime().Subtract(DateTime.UnixEpoch).TotalDays;
            return new ValueTask<double>(result);
        }

        public UnixTools() : base(nameof(UnixTools), "My Unix tools", null)
        {
            AddCommand(
                "unix2dt",
                "Converts a Unix date number to a DateTime",
                new Arg<double>("n", "number", "The Unix date number", double.Parse),
                UnixDateToDateTime);
            AddCommand(
                "dt2unix",
                "Converts a DateTime to a Unix date number",
                new Arg<DateTime>("d", "date", "The Unix date number", DateTime.Parse),
                DateTimeToUnixDate);
        }
    }
}