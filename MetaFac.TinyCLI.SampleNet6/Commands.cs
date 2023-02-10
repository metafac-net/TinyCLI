using MetaFac.TinyCLI;

namespace MetaFac.TinyCLI.SampleNet6
{
    internal class Commands : CommandsBase
    {
        public Commands() : base("MyCalc", "A simple calculator", null)
        {
            AddCommand(
                "add",
                "Adds two integers",
                new Arg<int>("x", "xValue", "The first value", int.Parse),
                new Arg<int>("y", "yValue", "The second value", int.Parse, 0),
                MyCalculator.add);
            AddCommand(
                "sub",
                "Subtracts two integers",
                new Arg<int>("a", "aValue", "The first value", int.Parse),
                new Arg<int>("b", "bValue", "The second value", int.Parse, 0),
                MyCalculator.sub);
            AddCommand(
                "div",
                "Divides two integers",
                new Arg<int>("x", "xValue", "The dividend", int.Parse),
                new Arg<int>("y", "yValue", "The divisor", int.Parse),
                MyCalculator.div);
        }
    }

}