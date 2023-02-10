using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MiniCLI
{
    public sealed class Cmd<TArg1, TResult> : CmdBase<TResult>
    {
        private readonly Arg<TArg1> ArgDef1;
        private readonly Func<TArg1, ValueTask<TResult>> _action;

        public Cmd(string name, string help,
            Arg<TArg1> argDef1,
            Func<TArg1, ValueTask<TResult>> action,
            CmdOptions? options = null)
            : base(name, help, options)
        {
            ArgDef1 = argDef1;
            _action = action;
        }

        protected override async ValueTask<int> OnRun(InternalLogger? logger, string[] args)
        {
            return await RunAction(logger, args, Name, Help, async () =>
            {
                (TArg1 arg1, List<string> remaining1) = GetValue<TArg1>(args, ArgDef1);
                CheckExtraArguments(remaining1);
                return await _action(arg1);
            },
            ArgDef1);
        }
    }
}