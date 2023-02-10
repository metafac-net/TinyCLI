using System;

namespace MetaFac.TinyCLI
{
    public abstract class ArgBase
    {
        public string Tag1 { get; }
        public string Tag2 { get; }
        public string Description { get; }
        public bool Required { get; }
        protected abstract Type OnGetArgType();
        public Type ArgType => OnGetArgType();
        protected abstract object OnGetDefaultValue();
        public object UntypedDefaultValue => OnGetDefaultValue();

        protected ArgBase(string tag1, string tag2, bool required, Type argType, string description)
        {
            Tag1 = tag1;
            Tag2 = tag2;
            Description = description ?? $"A value for the '--{tag2}' argument (of type {argType.Name})";
            Required = required;
        }
    }

    public sealed class Arg<T> : ArgBase
    {
        protected override Type OnGetArgType() => typeof(T);
        private T? _defaultValue { get; }
        protected override object OnGetDefaultValue() => _defaultValue ?? throw new InvalidOperationException("DefaultValue is null");
        public T TypedDefaultValue => _defaultValue ?? throw new InvalidOperationException("DefaultValue is null");

        public Func<string, T> Parser { get; }

        private Arg(string tag1, string tag2, Func<string, T> parser, bool required, T? defaultValue, string description)
            : base(tag1, tag2, required, typeof(T), description)
        {
            _defaultValue = defaultValue;
            Parser = parser;
        }

        /// <summary>
        /// Defines an optional argument.
        /// </summary>
        /// <param name="tag1"></param>
        /// <param name="tag2"></param>
        /// <param name="name"></param>
        /// <param name="parser"></param>
        /// <param name="defaultValue"></param>
        /// <param name="description"></param>
        public Arg(string tag1, string tag2, string description, Func<string, T> parser, T defaultValue)
            : this(tag1, tag2, parser, false, defaultValue, description) { }

        /// <summary>
        /// Defines a required argument.
        /// </summary>
        /// <param name="tag1"></param>
        /// <param name="tag2"></param>
        /// <param name="name"></param>
        /// <param name="parser"></param>
        /// <param name="description"></param>
        public Arg(string tag1, string tag2, string description, Func<string, T> parser)
            : this(tag1, tag2, parser, true, default, description) { }
    }
}