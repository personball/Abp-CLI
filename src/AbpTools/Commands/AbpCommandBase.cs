using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;

namespace AbpTools.Commands
{
    [HelpOption("--help")]
    abstract class AbpCommandBase
    {
        [Option("-T|--template-name", Consts.Descriptions.Base.TemplateNameDescription, CommandOptionType.SingleValue)]
        public string TemplateName { get; set; } = Consts.DefaultProjectTemplateName;

        /// <summary>
        /// TODO get placeholder in project template config?
        /// </summary>
        [Option("-h|--placeholder", Consts.Descriptions.Base.PlaceHolderDescription, CommandOptionType.SingleValue)]
        public string Placeholder { get; set; } = Consts.DefaultPlaceHolder;
        
        public abstract List<string> CreateArgs();

        protected virtual Task<int> OnExecuteAsync(CommandLineApplication app)
        {
            var args = CreateArgs();

            Console.WriteLine($"Result = {Consts.ParentCommandName} " + ArgumentEscaper.EscapeAndConcatenate(args));
            return Task.FromResult(0);
        }
    }
}
