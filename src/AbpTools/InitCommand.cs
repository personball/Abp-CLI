using AbpTools.Github;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using McMaster.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using WebApiClient;

namespace AbpTools
{
    [Command(Description = "init a project from project template")]
    class InitCommand : AbpCommandBase
    {
        [Option("-T|--template-name", "TemplateName <GithubUserName>/<RepoName>[@<ReleaseTag>],default as 'aspnetboilerplate/module-zero-core-template@latest'.", CommandOptionType.SingleValue)]
        public string TemplateName { get; set; } = "aspnetboilerplate/module-zero-core-template@latest";

        [Option("-h|--place-holder", "PlaceHolder in project template,default as 'AbpCompanyName.AbpProjectName'.", CommandOptionType.SingleValue)]
        public string PlaceHolder { get; set; } = "AbpCompanyName.AbpProjectName";

        [Option("-n|--project-name", "Your project name, default as 'AbpDemo'.", CommandOptionType.SingleValue)]
        public string ProjectName { get; set; } = "AbpDemo";

        [Option("-m", "Is this project a Multi-Pages Application? Default as false.", CommandOptionType.NoValue)]
        public bool Mpa { get; set; } = false;

        private const string SpaTypeDesc = "Choose 'vue' for vuejs or 'ng' for angularjs or 'react' for reactjs. Any invalid value will be replaced by 'vue'.";

        [Option("-t|--spa-type", SpaTypeDesc, CommandOptionType.SingleValue)]
        [AllowedValues("vue", "ng", "react", IgnoreCase = true)]
        public string SpaType { get; set; }

        [Option("-b", "Rename Backup", CommandOptionType.NoValue)]
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
                SpaType = Prompt.GetString(SpaTypeDesc, defaultValue: "vue");
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

            //set project folder
            var projectFolder = Path.Combine(Directory.GetCurrentDirectory(), ProjectName);

            if (!Directory.Exists(projectFolder))
            {
                Directory.CreateDirectory(projectFolder);
            }

            //Fetch Template Zip
            Console.WriteLine($"Fetching Project Template From Github.com/{TemplateName}...");

            var filePath = Path.Combine(projectFolder, $"{ProjectName}.zip");

            var tNames = TemplateName.Split(new char[] { '/', '@' });
            var userName = string.Empty;
            var repoName = string.Empty;
            var tagName = "latest";
            if (tNames.Length > 1)
            {
                userName = tNames[0];
                repoName = tNames[1];
            }

            if (tNames.Length > 2)
            {
                tagName = tNames[2];
            }

            await FetchTemplateZipFile(userName, repoName, tagName, filePath);

            //Extract Zip File
            Console.WriteLine($"Extracting Project Template Zip:{filePath}>{projectFolder}...");
            ExtractZipFile(filePath, projectFolder, repoName);

            //Folders To Rename
            Console.WriteLine("Cleaning Sub Dirs in Project Template Folder...");
            var excludeFolders = new List<string> { "./vue", "./angular", "./reactjs" };
            if (!Mpa)
            {
                excludeFolders.Add($"./aspnet-core/src/{PlaceHolder}.Web.Mvc");
                switch (SpaType.ToLower())
                {
                    case "ng":
                        excludeFolders.Remove("angular");
                        break;
                    case "react":
                        excludeFolders.Remove("reactjs");
                        break;
                    case "vue":
                    default:
                        excludeFolders.Remove("vue");
                        break;
                }
            }

            //Delete ExcludeFolders
            foreach (var excludeFolder in excludeFolders)
            {
                var directoryToDel = Path.Combine(projectFolder, excludeFolder);
                if (Directory.Exists(directoryToDel))
                {
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

            //Delete Template Zip
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            Console.WriteLine("Init Completed!");
            return 1;
        }

        public override List<string> CreateArgs()
        {
            var args = Parent.CreateArgs();
            args.Add("init");

            return args;
        }

        private async Task FetchTemplateZipFile(string userName, string repoName, string tagName, string filePath)
        {
            var client = HttpApiClient.Create<IGithubApis>();
            QueryReleaseResult result = null;

            try
            {
                if (tagName.ToLower() == "latest")
                {
                    //latest
                    result = await client.QueryLatestRelease(userName, repoName);
                }
                else
                {
                    //tag like "v4.2.0"
                    result = await client.QueryTags(userName, repoName, tagName);
                }
            }
            catch (HttpStatusFailureException ex)
            {
                Console.WriteLine($"Query Project Template {TemplateName} Fail:{ex.Message}");

                //try latest
                if (tagName.ToLower() != "latest")
                {
                    Console.WriteLine($"Try Project Template {userName}/{repoName}@latest");
                    result = await client.QueryLatestRelease(userName, repoName);
                }
            }

            var zip_url = result.zipball_url;

            using (var webClient = new WebClient())
            {
                webClient.Headers.Add("Accept-Language", " en-US");
                webClient.Headers.Add("Accept", " text/html, application/xhtml+xml, */*");
                webClient.Headers.Add("User-Agent", "dotnet tools abplus,a cli tool for ABP framework.");

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
            ZipFile zf = null;
            try
            {
                FileStream fs = File.OpenRead(archiveFilenameIn);
                zf = new ZipFile(fs);

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
                    if (entryFileName.StartsWith(repoName))
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
    }
}