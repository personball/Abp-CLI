using System.Collections.Generic;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;

namespace AbpTools.Commands
{
    [Command(Description = Consts.Descriptions.Set.CommandDescription)]
    class SetCommand : AbpCommandBase
    {
        //[Argument(0, nameof(Identifier), Consts.Descriptions.New.IdentifierDescription, ShowInHelpText = true)]
        //[AllowedValues("console", "module", IgnoreCase = true)]
        //public string Identifier { get; set; }

        //[Option("-n|--name", Consts.Descriptions.New.NameDescription, CommandOptionType.SingleValue)]
        //public string Name { get; set; }

        // You can use this pattern when the parent command may have options or methods you want to
        // use from sub-commands.
        // This will automatically be set before OnExecute is invoked
        private Abp Parent { get; set; }

        protected override async Task<int> OnExecuteAsync(CommandLineApplication app)
        {
            //TODO set default template?如果存在就保存设置，不存在则下载完成后保存设置

            //TODO set default placeholder
            //TODO set default 
            return 0;
        }

        public override List<string> CreateArgs()
        {
            var args = Parent.CreateArgs();
            args.Add("set");

            return args;
        }
    }
}
