using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using AbpTools.Renamer;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using McMaster.Extensions.CommandLineUtils;
using Octokit;

namespace AbpTools.Commands
{
    [Command(Description = Consts.Descriptions.Init.CommandDescription)]
    class InitCommand : AbpCommandBase
    {
        [Option("-T|--template-name", Consts.Descriptions.Init.TemplateNameDescription, CommandOptionType.SingleValue)]
        public string TemplateName { get; set; } = Consts.DefaultProjectTemplateName;

        [Option("-h|--place-holder", Consts.Descriptions.Init.PlaceHolderDescription, CommandOptionType.SingleValue)]
        public string PlaceHolder { get; set; } = Consts.DefaultPlaceHolder;

        [Argument(0, nameof(ProjectName), Consts.Descriptions.Init.ProjectNameDescription, ShowInHelpText = true)]
        public string ProjectName { get; set; } = Consts.DefaultProjectName;

        [Option("-m", Consts.Descriptions.Init.MpaDescription, CommandOptionType.NoValue)]
        public bool Mpa { get; set; } = false;

        [Option("-t|--spa-type", Consts.Descriptions.Init.SpaTypeDescription, CommandOptionType.SingleValue)]
        [AllowedValues("vue", "ng", "react", IgnoreCase = true)]
        public string SpaType { get; set; }

        [Option("-b", Consts.Descriptions.Init.RenameBackupDescription, CommandOptionType.NoValue)]
        public bool RenameBackup { get; set; }

        // You can use this pattern when the parent command may have options or methods you want to
        // use from sub-commands.
        // This will automatically be set before OnExecute is invoked
        private Abp Parent { get; set; }

        protected override async Task<int> OnExecuteAsync(CommandLineApplication app)
        {
            if (TemplateName.IndexOf('/') <= 0 || TemplateName.EndsWith('/'))
            {
                Console.WriteLine("Invalid TemplateName!Please Enter As Format aspnetboilerplate/module-zero-core-template@v4.2.0 ");
            }

            if (!Mpa && string.IsNullOrWhiteSpace(SpaType))
            {
                SpaType = Prompt.GetString(Consts.Descriptions.Init.SpaTypeDescription, defaultValue: "vue");
                SpaType = SpaType.ToLower();
                if ("vue,ng,react".IndexOf(SpaType) == -1)
                {
                    SpaType = "vue";
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

            //Path to save project tempalte zip file
            var projectTemplateZipFilePath = Path.Combine(projectFolder, $"{ProjectName}.zip");

            (var userName, var repoName, var tagName) = TemplateName.Parsing();

            await FetchTemplateZipFile(userName, repoName, tagName, projectTemplateZipFilePath);

            ExtractZipFile(projectTemplateZipFilePath, projectFolder, repoName);

            RenameFolders(projectFolder);

            //Delete Project Template Zip File
            if (File.Exists(projectTemplateZipFilePath))
            {
                File.Delete(projectTemplateZipFilePath);
            }

            Console.WriteLine("Init Completed!");
            return 1;
        }

        private async Task FetchTemplateZipFile(string userName, string repoName, string tagName, string filePath)
        {
            Console.WriteLine($"Fetching Project Template From Github.com/{userName}/{repoName}@{tagName}...");

            var github = new GitHubClient(new ProductHeaderValue(Consts.ProductName));

            Release release = null;

            try
            {
                release = await github.QueryRelease(userName, repoName, tagName);
            }
            catch (Exception ex)//TODO@personball catch 403,Rate Limit
            {
                //see https://developer.github.com/v3/#rate-limiting
                Console.WriteLine("You May Have Reach Github Api Rate-Limit.");
                Console.WriteLine("See:https://developer.github.com/v3/#rate-limiting");
                Console.WriteLine("Please Enter Your Github UserName and Password for Basic Auth.");

                var id = Prompt.GetString($"Please Enter Your Github UserName:");
                var pwd = Prompt.GetPassword($"Please Enter Your Github Password:");
                var clientWithCredentials = new GitHubClient(new ProductHeaderValue(Consts.ProductName))
                {
                    Credentials = new Credentials(id, pwd)
                };

                release = await clientWithCredentials.QueryRelease(userName, repoName, tagName);

                if (release == null)
                {
                    Console.WriteLine($"Query Project Template[Github.com/{userName}/{repoName}@{tagName}] Fail.");
                    throw ex;
                }
            }

            DownLoadZipFile(release.ZipballUrl, filePath);
        }

        private void DownLoadZipFile(string zip_url, string filePath)
        {
            using (var webClient = new WebClient())
            {
                webClient.Headers.Add("Accept-Language", " en-US");
                webClient.Headers.Add("Accept", " text/html, application/xhtml+xml, */*");
                webClient.Headers.Add("User-Agent", Consts.UserAgent);

                Console.WriteLine($"Start fetch zip file:{zip_url}");
                Console.WriteLine($"Downloading...");

                try
                {
                    webClient.DownloadFile(zip_url, filePath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    throw;
                }

                Console.WriteLine($"Download success and save as {filePath}");
            }
        }

        private void ExtractZipFile(string archiveFilenameIn, string outFolder, string repoName)
        {
            Console.WriteLine($"Extracting Project Template Zip:{archiveFilenameIn}...");
            Console.WriteLine($"Extracting To:{outFolder}...");

            ZipFile zf = null;
            try
            {
                FileStream fs = File.OpenRead(archiveFilenameIn);
                zf = new ZipFile(fs);
                var firstZipEntry = zf[0];

                foreach (ZipEntry zipEntry in zf)
                {
                    if (!zipEntry.IsFile)
                    {
                        continue;           // Ignore directories
                    }
                    String entryFileName = zipEntry.Name;
                    // to remove the folder from the entry:- entryFileName = Path.GetFileName(entryFileName);
                    // Optionally match entrynames against a selection list here to skip as desired.
                    // The unpacked length is available in the zipEntry.Size property.

                    byte[] buffer = new byte[4096];     // 4K is optimum
                    Stream zipStream = zf.GetInputStream(zipEntry);

                    // Manipulate the output filename here as desired.
                    //remove first level folder
                    if (firstZipEntry.IsDirectory)
                    {
                        entryFileName = entryFileName.Substring(entryFileName.IndexOf("/") + 1);
                    }

                    String fullZipToPath = Path.Combine(outFolder, entryFileName);
                    string directoryName = Path.GetDirectoryName(fullZipToPath);
                    if (directoryName.Length > 0)
                        Directory.CreateDirectory(directoryName);

                    // Unzip file in buffered chunks. This is just as fast as unpacking to a buffer the full size
                    // of the file, but does not waste memory.
                    // The "using" will close the stream even if an exception occurs.
                    using (FileStream streamWriter = File.Create(fullZipToPath))
                    {
                        StreamUtils.Copy(zipStream, streamWriter, buffer);
                    }
                }
            }
            finally
            {
                if (zf != null)
                {
                    zf.IsStreamOwner = true; // Makes close also shut the underlying stream
                    zf.Close(); // Ensure we release resources
                }
            }
        }

        private void RenameFolders(string projectFolder)
        {
            //Folders To Rename
            Console.WriteLine("Cleaning Sub Dirs in Project Template Folder...");
            var excludeFolders = new List<string> { "./vue", "./angular", "./reactjs" };
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

            //Delete ExcludeFolders
            foreach (var excludeFolder in excludeFolders)
            {
                var directoryToDel = Path.Combine(projectFolder, excludeFolder);
                if (Directory.Exists(directoryToDel))
                {
                    Console.WriteLine($"ExcludeFolders Delete:{directoryToDel}");
                    Directory.Delete(directoryToDel, true);
                }
            }

            //Rename Folder
            var companyNamePlaceHolder = PlaceHolder.Split('.')[0];
            var projectNamePlaceHolder = PlaceHolder.Split('.')[1];

            var newNames = ProjectName.Split('.');
            var newCompanyName = string.Empty;
            var newProjectName = ProjectName;
            if (newNames.Length > 1)
            {
                newCompanyName = newNames[0];
                newProjectName = newNames[1];
            }
            Console.WriteLine($"Renaming {PlaceHolder} to {newCompanyName}{(newNames.Length > 1 ? "." : "")}{newProjectName}...");
            var slnRenamer = new SolutionRenamer(projectFolder, companyNamePlaceHolder, projectNamePlaceHolder, newCompanyName, newProjectName)
            {
                CreateBackup = RenameBackup
            };

            slnRenamer.Run();
        }

        public override List<string> CreateArgs()
        {
            var args = Parent.CreateArgs();
            args.Add("init");

            return args;
        }
    }
}
