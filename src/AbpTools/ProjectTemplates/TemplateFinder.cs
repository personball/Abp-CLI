using System;
using System.IO;
using System.Threading.Tasks;
using AbpTools.Utils;
using McMaster.Extensions.CommandLineUtils;
using Octokit;

namespace AbpTools.ProjectTemplates
{
    public class TemplateFinder
    {
        public static string TemplateFilesRoot = Path.Combine(Path.GetTempPath(), $".{Path.DirectorySeparatorChar}_abplus_tpls");

        public TemplateFinder(string templateName)
        {
            (var userName, var repoName, var releaseTag) = templateName.TemplateNameParse();

            UserName = userName;
            RepoName = repoName;
            ReleaseTag = releaseTag;
        }
        public string UserName { get; set; }

        public string RepoName { get; set; }

        public string ReleaseTag { get; set; }

        /// <summary>
        /// fetch project template
        /// </summary>
        /// <returns>path of project template zip file.</returns>
        public async Task<string> Fetch()
        {
            Console.WriteLine($"Querying Releases From Github.com/{UserName}/{RepoName}@{ReleaseTag}...");
            var release = await QueryRelaseFromGithub();

            var projectTemplateFileName = $"{release.Author.Login}_{RepoName}@{release.Name}.zip";
            var tplFilePath = Path.Combine(TemplateFilesRoot, $".{Path.DirectorySeparatorChar}{projectTemplateFileName}");
            if (File.Exists(tplFilePath))
            {
                return tplFilePath;
            }
            else
            {
                if (!Directory.Exists(TemplateFilesRoot))
                {
                    Directory.CreateDirectory(TemplateFilesRoot);
                }

                DownLoadHelper.DownLoadZipFile(release.ZipballUrl, tplFilePath);

                return tplFilePath;
            }
        }

        private async Task<Release> QueryRelaseFromGithub()
        {
            var github = new GitHubClient(new ProductHeaderValue(Consts.ProductName));

            Release release = null;

            try
            {
                release = await github.QueryRelease(UserName, RepoName, ReleaseTag);
            }
            catch (Exception ex)
            {
                //see https://developer.github.com/v3/#rate-limiting
                Console.WriteLine("You May Have Reach Github Api Rate-Limit.");
                Console.WriteLine("See:https://developer.github.com/v3/#rate-limiting");
                Console.WriteLine("Please Enter Your Github UserName and Password for Basic Auth.(We won't save your password!):");

                var id = Prompt.GetString($"Please Enter Your Github UserName:");
                var pwd = Prompt.GetPassword($"Please Enter Your Github Password:");
                var clientWithCredentials = new GitHubClient(new ProductHeaderValue(Consts.ProductName))
                {
                    Credentials = new Credentials(id, pwd)
                };

                release = await clientWithCredentials.QueryRelease(UserName, RepoName, ReleaseTag);

                if (release == null)
                {
                    Console.WriteLine($"Query Releases of [Github.com/{UserName}/{RepoName}@{ReleaseTag}] Fail.");
                    throw ex;
                }
            }

            return release;
        }
    }
}
