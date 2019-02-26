using System;

namespace AbpTools
{
    public static class StringExtensions
    {
        public static bool IsLatest(this string tagName)
        {
            return tagName.ToLower() == Consts.LatestString;
        }

        public static (string userName, string repoName, string tagName) Parsing(this string templateName)
        {
            if (!string.IsNullOrWhiteSpace(templateName))
            {
                var tNames = templateName.Split(new char[] { '/', '@' });
                var userName = string.Empty;
                var repoName = string.Empty;
                var tagName = Consts.LatestString;
                if (tNames.Length > 1)
                {
                    userName = tNames[0];
                    repoName = tNames[1];
                }

                if (tNames.Length > 2)
                {
                    tagName = tNames[2];
                }

                return (userName, repoName, tagName);
            }
            else
            {
                throw new ArgumentNullException(nameof(templateName));
            }
        }

        public static string EnsureEndsWith(this string v, char b)
        {
            if (v.EndsWith(b))
            {
                return v;
            }

            return v + b;
        }
    }
}
