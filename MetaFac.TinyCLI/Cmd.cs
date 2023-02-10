using System;
using System.Linq;
using System.Threading.Tasks;

namespace MiniCLI
{
    public sealed class Cmd<TResult> : CmdBase<TResult>
    {
        private readonly Func<ValueTask<TResult>> _action;

        public Cmd(string name, string help,
            Func<ValueTask<TResult>> action,
            CmdOptions? options = null)
            : base(name, help, options)
        {
            _action = action;
        }

        protected override async ValueTask<int> OnRun(InternalLogger? logger, string[] args)
        {
            return await RunAction(logger, args, Name, Help, async () =>
            {
                CheckExtraArguments(args.ToList());
                return await _action();
            });
        }
    }
}