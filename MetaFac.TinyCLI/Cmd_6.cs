using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MetaFac.TinyCLI
{
    public sealed class Cmd<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TResult> : CmdBase<TResult>
    {
        private readonly Arg<TArg1> ArgDef1;
        private readonly Arg<TArg2> ArgDef2;
        private readonly Arg<TArg3> ArgDef3;
        private readonly Arg<TArg4> ArgDef4;
        private readonly Arg<TArg5> ArgDef5;
        private readonly Arg<TArg6> ArgDef6;
        private readonly Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, ValueTask<TResult>> _actionMin;
        private readonly Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, OtherArgs, ValueTask<TResult>> _actionExt;

        public Cmd(string name, string help,
            Arg<TArg1> argDef1,
            Arg<TArg2> argDef2,
            Arg<TArg3> argDef3,
            Arg<TArg4> argDef4,
            Arg<TArg5> argDef5,
            Arg<TArg6> argDef6,
            Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, ValueTask<TResult>> action,
            CmdOptions? options,
            Func<TResult, int>? exitFunc)
            : base(name, help, options, exitFunc)
        {
            ArgDef1 = argDef1;
            ArgDef2 = argDef2;
            ArgDef3 = argDef3;
            ArgDef4 = argDef4;
            ArgDef5 = argDef5;
            ArgDef6 = argDef6;
            _actionMin = action;
            _actionExt = null!;
        }

        public Cmd(string name, string help,
            Arg<TArg1> argDef1,
            Arg<TArg2> argDef2,
            Arg<TArg3> argDef3,
            Arg<TArg4> argDef4,
            Arg<TArg5> argDef5,
            Arg<TArg6> argDef6,
            Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, OtherArgs, ValueTask<TResult>> action,
            CmdOptions? options,
            Func<TResult, int>? exitFunc)
            : base(name, help, options, exitFunc)
        {
            ArgDef1 = argDef1;
            ArgDef2 = argDef2;
            ArgDef3 = argDef3;
            ArgDef4 = argDef4;
            ArgDef5 = argDef5;
            ArgDef6 = argDef6;
            _actionMin = null!;
            _actionExt = action;
        }

        protected override async ValueTask<int> OnRun(InternalLogger? logger, string[] args)
        {
            return await RunAction(logger, args, Name, Help, async () =>
            {
                (TArg1 arg1, List<string> remaining1) = GetValue(args, ArgDef1);
                (TArg2 arg2, List<string> remaining2) = GetValue(remaining1.ToArray(), ArgDef2);
                (TArg3 arg3, List<string> remaining3) = GetValue(remaining2.ToArray(), ArgDef3);
                (TArg4 arg4, List<string> remaining4) = GetValue(remaining3.ToArray(), ArgDef4);
                (TArg5 arg5, List<string> remaining5) = GetValue(remaining4.ToArray(), ArgDef5);
                (TArg6 arg6, List<string> remaining6) = GetValue(remaining5.ToArray(), ArgDef6);
                CheckExtraArguments(remaining6);
                if (_actionExt is not null)
                {
                    return await _actionExt(arg1, arg2, arg3, arg4, arg5, arg6, new OtherArgs(remaining6));
                }
                else
                {
                    return await _actionMin(arg1, arg2, arg3, arg4, arg5, arg6);
                }
            },
            ArgDef1, ArgDef2, ArgDef3, ArgDef4, ArgDef5, ArgDef6);
        }
    }
}