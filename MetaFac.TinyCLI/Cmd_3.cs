using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MiniCLI
{
    public sealed class Cmd<TArg1, TArg2, TArg3, TResult> : CmdBase<TResult>
    {
        private readonly Arg<TArg1> ArgDef1;
        private readonly Arg<TArg2> ArgDef2;
        private readonly Arg<TArg3> ArgDef3;
        private readonly Func<TArg1, TArg2, TArg3, ValueTask<TResult>> _action;

        public Cmd(string name, string help,
            Arg<TArg1> argDef1,
            Arg<TArg2> argDef2,
            Arg<TArg3> argDef3,
            Func<TArg1, TArg2, TArg3, ValueTask<TResult>> action,
            CmdOptions? options = null)
            : base(name, help, options)
        {
            ArgDef1 = argDef1;
            ArgDef2 = argDef2;
            ArgDef3 = argDef3;
            _action = action;
        }

        protected override async ValueTask<int> OnRun(InternalLogger? logger, string[] args)
        {
            return await RunAction(logger, args, Name, Help, async () =>
            {
                (TArg1 arg1, List<string> remaining1) = GetValue<TArg1>(args, ArgDef1);
                (TArg2 arg2, List<string> remaining2) = GetValue<TArg2>(remaining1.ToArray(), ArgDef2);
                (TArg3 arg3, List<string> remaining3) = GetValue<TArg3>(remaining2.ToArray(), ArgDef3);
                CheckExtraArguments(remaining3);
                return await _action(arg1, arg2, arg3);
            },
            ArgDef1, ArgDef2, ArgDef3);
        }
    }
}