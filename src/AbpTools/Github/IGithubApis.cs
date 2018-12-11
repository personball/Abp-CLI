using System.Threading.Tasks;
using WebApiClient;
using WebApiClient.Attributes;

namespace AbpTools.Github
{
    //[LogFilter]
    [HttpHost("https://api.github.com")]
    public interface IGithubApis : IHttpApi
    {
        [HttpGet("/repos/{userName}/{repoName}/releases/latest")]
        Task<QueryReleaseResult> QueryLatestRelease(string userName, string repoName);

        [HttpGet("/repos/{userName}/{repoName}/releases/tags/{tagName}")]
        Task<QueryReleaseResult> QueryTags(string userName,string repoName,string tagName);
    }
}
