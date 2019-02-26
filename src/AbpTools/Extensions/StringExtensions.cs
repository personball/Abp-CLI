using System;

namespace AbpTools
{
    public static class StringExtensions
    {
        public static bool IsLatest(this string tagName)
        {
            return tagName.ToLower() == Consts.LatestString;
        }

        public static (string userName, string repoName, string tagName) TemplateNameParse(this string templateName)
        {
            if (templateName.IsNullOrWhiteSpace())
            {
                throw new ArgumentNullException(nameof(templateName));
            }

            if (templateName.IndexOf('/') <= 0 || templateName.EndsWith('/'))
            {
                Console.WriteLine("Invalid TemplateName!Please Enter As Format aspnetboilerplate/module-zero-core-template@v4.2.0 ");
                throw new ArgumentException("Invalid TemplateName", nameof(templateName));
            }

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

        public static bool IsNullOrWhiteSpace(this string v)
        {
            return string.IsNullOrWhiteSpace(v);
        }

        public static (string company, string project, string module) NameParse(this string name)
        {
            if (name.IsNullOrWhiteSpace())
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (name.IndexOf('.') <= 0 || name.EndsWith('.'))
            {
                Console.WriteLine("Invalid Format! Please Enter As Format: CompanyName.ProjectName or CompanyName.ProjectName.ModuleName!");
                throw new ArgumentException("Invalid Format", nameof(name));
            }

            var tNames = name.Split('.');
            var company = string.Empty;
            var project = string.Empty;
            var module = string.Empty;
            if (tNames.Length > 1)
            {
                company = tNames[0];
                project = tNames[1];
            }

            if (tNames.Length > 2)
            {
                module = tNames[2];
            }

            return (company, project, module);
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
