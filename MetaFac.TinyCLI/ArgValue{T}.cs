using System;

namespace MiniCLI
{
    internal sealed class ArgValue<T> : ArgValue
    {
        public T ParsedValue { get; }
        public ArgValue(string? input, T parsedValue, Arg<T> argDef) : base(input, argDef)
        {
            ParsedValue = parsedValue;
        }
        protected override Type OnGetArgType() => typeof(T);

        protected override object? OnGetArgValue()
        {
            return ParsedValue;
        }
    }
}