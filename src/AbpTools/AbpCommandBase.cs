using McMaster.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AbpTools
{
    [HelpOption("--help")]
    abstract class AbpCommandBase
    {
        public abstract List<string> CreateArgs();

        protected virtual Task<int> OnExecuteAsync(CommandLineApplication app)
        {
            var args = CreateArgs();

            Console.WriteLine("Result = abp " + ArgumentEscaper.EscapeAndConcatenate(args));
            return Task.FromResult(0);
        }
    }
}