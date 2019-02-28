using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using AbpTools.Commands;
using McMaster.Extensions.CommandLineUtils;

namespace AbpTools
{
    [Command(Consts.ParentCommandName)]
    [VersionOptionFromMember("--version", MemberName = nameof(GetVersion))]
    [Subcommand(
        typeof(InitCommand)
        , typeof(NewCommand)
        //, typeof(SetCommand)
        )]
    class Abp : AbpCommandBase
    {
        public static void Main(string[] args) => CommandLineApplication.Execute<Abp>(args);

        protected override Task<int> OnExecuteAsync(CommandLineApplication app)
        {
            // this shows help even if the --help option isn't specified
            app.ShowHelp();
            return Task.FromResult(1);
        }

        public override List<string> CreateArgs()
        {
            var args = new List<string>();

            return args;
        }

        private static string GetVersion()
            => typeof(Abp).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
    }
}
