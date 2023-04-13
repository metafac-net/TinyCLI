using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MetaFac.TinyCLI
{
    public sealed class Cmd<TResult> : CmdBase<TResult>
    {
        private readonly Func<ValueTask<TResult>> _actionMin;
        private readonly Func<OtherArgs, ValueTask<TResult>> _actionExt;

        public Cmd(string name, string help,
            Func<ValueTask<TResult>> action,
            CmdOptions? options,
            Func<TResult, int>? exitFunc)
            : base(name, help, options, exitFunc)
        {
            _actionMin = action;
            _actionExt = null!;
        }

        public Cmd(string name, string help,
            Func<OtherArgs, ValueTask<TResult>> action,
            CmdOptions? options,
            Func<TResult, int>? exitFunc)
            : base(name, help, options, exitFunc)
        {
            _actionMin = null!;
            _actionExt = action;
        }

        protected override async ValueTask<int> OnRun(InternalLogger? logger, string[] args)
        {
            return await RunAction(logger, args, Name, Help, async () =>
            {
                var remaining0 = args.ToList();
                CheckExtraArguments(remaining0);
                if (_actionExt is not null)
                {
                    return await _actionExt(new OtherArgs(remaining0));
                }
                else
                {
                    return await _actionMin();
                }
            });
        }
    }
}