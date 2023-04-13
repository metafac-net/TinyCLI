using System.Collections;
using System.Collections.Generic;

namespace MetaFac.TinyCLI
{
    public sealed class OtherArgs : IReadOnlyList<string>
    {
        private readonly List<string> _args;
        public OtherArgs(List<string> args) => _args = args;
        public string this[int index] => _args[index];
        public int Count => _args.Count;
        public IEnumerator<string> GetEnumerator() => _args.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _args.GetEnumerator();
    }
}