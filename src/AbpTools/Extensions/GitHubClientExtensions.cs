using System.Threading.Tasks;
using Octokit;

namespace AbpTools
{
    public static class GitHubClientExtensions
    {
        public static async Task<Release> QueryRelease(this GitHubClient client, string userName, string repoName, string tagName)
        {
            if (tagName.IsLatest())
            {
                return await client.Repository.Release.GetLatest(userName, repoName);
            }
            else
            {
                return await client.Repository.Release.Get(userName, repoName, tagName);
            }
        }
    }
}
