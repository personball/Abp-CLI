using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AbpTools.ProjectTemplates;
using AbpTools.Utils;
using McMaster.Extensions.CommandLineUtils;

namespace AbpTools.Commands
{
    [Command(Description = Consts.Descriptions.Init.CommandDescription)]
    class InitCommand : AbpCommandBase
    {
        [Argument(0, nameof(ProjectName), Consts.Descriptions.Init.ProjectNameDescription, ShowInHelpText = true)]
        public string ProjectName { get; set; } = Consts.DefaultProjectName;

        [Option("-m", Consts.Descriptions.Init.MpaDescription, CommandOptionType.NoValue)]
        public bool Mpa { get; set; } = false;

        [Option("-t|--project-type", Consts.Descriptions.Init.SpaTypeDescription, CommandOptionType.SingleValue)]
        [AllowedValues("vue", "ng", "react", IgnoreCase = true)]
        public string SpaType { get; set; }

        [Option("-b", Consts.Descriptions.Init.RenameBackupDescription, CommandOptionType.NoValue)]
        public bool RenameBackup { get; set; }

        // You can use this pattern when the parent command may have options or methods you want to
        // use from sub-commands.
        // This will automatically be set before OnExecute is invoked
        private Abp Parent { get; set; }

        public static List<string> InitExcludedFolders = new List<string> { "./vue", "./angular", "./reactjs" };

        static InitCommand()
        {
            NewCommand.IdentifierFolders.ForEach(s =>
            {
                if (!InitExcludedFolders.Contains(s))
                {
                    InitExcludedFolders.Add(s);
                }
            });
        }

        protected override async Task<int> OnExecuteAsync(CommandLineApplication app)
        {
            if (!Mpa)
            {
                //switch to personball/module-zero-core-template to reduce size of project template zip file
                if (TemplateName == Consts.DefaultProjectTemplateName)
                {
                    TemplateName = Consts.DefaultProjectTemplateForSPA;
                }

                if (string.IsNullOrWhiteSpace(SpaType))
                {
                    SpaType = Prompt.GetString(Consts.Descriptions.Init.SpaTypeDescription, defaultValue: "vue");
                    SpaType = SpaType.ToLower();
                    if ("vue,ng,react".IndexOf(SpaType) == -1)
                    {
                        SpaType = "vue";
                    }
                }
            }

            Console.WriteLine($"TemplateName\t:{TemplateName}");
            Console.WriteLine($"PlaceHolder\t:{PlaceHolder}");
            Console.WriteLine($"ProjectName\t:{ProjectName}");
            Console.WriteLine($"MPA\t\t:{Mpa}");
            Console.WriteLine($"SpaType\t\t:{SpaType}");
            Console.WriteLine($"RenameBackup\t:{RenameBackup}");

            //Create Target folder
            var projectFolder = Path.Combine(Directory.GetCurrentDirectory(), ProjectName);
            if (!Directory.Exists(projectFolder))
            {
                Directory.CreateDirectory(projectFolder);
            }

            var tplFinder = new TemplateFinder(TemplateName);

            var tplFilePath = await tplFinder.Fetch();

            ExtractHelper.ExtractZipFile(tplFilePath, projectFolder);

            var excludeFolders = new List<string>();
            InitExcludedFolders.ForEach(s =>
            {
                excludeFolders.Add(s);
            });

            if (!Mpa)
            {
                excludeFolders.Add($"./aspnet-core/src/{PlaceHolder}.Web.Mvc");
                //TODO@personball remove web.mvc entry in vs solution file(sln). 
                switch (SpaType.ToLower())
                {
                    case "ng":
                        excludeFolders.Remove("./angular");
                        break;
                    case "react":
                        excludeFolders.Remove("./reactjs");
                        break;
                    case "vue":
                    default:
                        excludeFolders.Remove("./vue");
                        break;
                }
            }

            RenameHelper.RenameFolders(projectFolder, PlaceHolder, ProjectName, RenameBackup, excludeFolders);

            Console.WriteLine("Init Completed!");
            return 0;
        }

        public override List<string> CreateArgs()
        {
            var args = Parent.CreateArgs();
            args.Add("init");

            return args;
        }
    }
}
