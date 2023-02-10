using System;

namespace MetaFac.TinyCLI
{
    public abstract class ArgValue
    {
        public string? Input { get; }
        public ArgBase ArgDef { get; }
        protected abstract Type OnGetArgType();
        public Type GetArgType() => OnGetArgType();
        protected abstract object? OnGetArgValue();
        public object? GetArgValue() => OnGetArgValue();

        protected ArgValue(string? input, ArgBase argDef)
        {
            Input = input;
            ArgDef = argDef;
        }
    }
}