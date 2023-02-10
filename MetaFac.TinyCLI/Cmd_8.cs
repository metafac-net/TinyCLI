using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MetaFac.TinyCLI
{
    public sealed class Cmd<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TResult> : CmdBase<TResult>
    {
        private readonly Arg<TArg1> ArgDef1;
        private readonly Arg<TArg2> ArgDef2;
        private readonly Arg<TArg3> ArgDef3;
        private readonly Arg<TArg4> ArgDef4;
        private readonly Arg<TArg5> ArgDef5;
        private readonly Arg<TArg6> ArgDef6;
        private readonly Arg<TArg7> ArgDef7;
        private readonly Arg<TArg8> ArgDef8;
        private readonly Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, ValueTask<TResult>> _action;

        public Cmd(string name, string help,
            Arg<TArg1> argDef1,
            Arg<TArg2> argDef2,
            Arg<TArg3> argDef3,
            Arg<TArg4> argDef4,
            Arg<TArg5> argDef5,
            Arg<TArg6> argDef6,
            Arg<TArg7> argDef7,
            Arg<TArg8> argDef8,
            Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, ValueTask<TResult>> action,
            CmdOptions? options = null)
            : base(name, help, options)
        {
            ArgDef1 = argDef1;
            ArgDef2 = argDef2;
            ArgDef3 = argDef3;
            ArgDef4 = argDef4;
            ArgDef5 = argDef5;
            ArgDef6 = argDef6;
            ArgDef7 = argDef7;
            ArgDef8 = argDef8;
            _action = action;
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
                (TArg7 arg7, List<string> remaining7) = GetValue(remaining6.ToArray(), ArgDef7);
                (TArg8 arg8, List<string> remaining8) = GetValue(remaining7.ToArray(), ArgDef8);
                CheckExtraArguments(remaining8);
                return await _action(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
            },
            ArgDef1, ArgDef2, ArgDef3, ArgDef4, ArgDef5, ArgDef6, ArgDef7, ArgDef8);
        }
    }
}