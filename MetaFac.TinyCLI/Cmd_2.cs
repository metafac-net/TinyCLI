using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MetaFac.TinyCLI
{
    public sealed class Cmd<TArg1, TArg2, TResult> : CmdBase<TResult>
    {
        private readonly Arg<TArg1> ArgDef1;
        private readonly Arg<TArg2> ArgDef2;
        private readonly Func<TArg1, TArg2, ValueTask<TResult>> _actionMin;
        private readonly Func<TArg1, TArg2, OtherArgs, ValueTask<TResult>> _actionExt;

        public Cmd(string name, string help,
            Arg<TArg1> argDef1,
            Arg<TArg2> argDef2,
            Func<TArg1, TArg2, ValueTask<TResult>> action,
            CmdOptions? options,
            Func<TResult, int>? exitFunc)
            : base(name, help, options, exitFunc)
        {
            ArgDef1 = argDef1;
            ArgDef2 = argDef2;
            _actionMin = action;
            _actionExt = null!;
        }

        public Cmd(string name, string help,
            Arg<TArg1> argDef1,
            Arg<TArg2> argDef2,
            Func<TArg1, TArg2, OtherArgs, ValueTask<TResult>> action,
            CmdOptions? options,
            Func<TResult, int>? exitFunc)
            : base(name, help, options, exitFunc)
        {
            ArgDef1 = argDef1;
            ArgDef2 = argDef2;
            _actionMin = null!;
            _actionExt = action;
        }

        protected override async ValueTask<int> OnRun(InternalLogger? logger, string[] args)
        {
            return await RunAction(logger, args, Name, Help, async () =>
            {
                (TArg1 arg1, List<string> remaining1) = GetValue(args, ArgDef1);
                (TArg2 arg2, List<string> remaining2) = GetValue(remaining1.ToArray(), ArgDef2);
                CheckExtraArguments(remaining2);
                if (_actionExt is not null)
                {
                    return await _actionExt(arg1, arg2, new OtherArgs(remaining2));
                }
                else
                {
                    return await _actionMin(arg1, arg2);
                }
            },
            ArgDef1, ArgDef2);
        }
    }
}