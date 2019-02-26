namespace AbpTools
{
    public class Consts
    {
        public const string ParentCommandName = "abplus";
        public const string ProductName = "DotnetGlobalToolAbpTools";
        public const string LatestString = "latest";
        public const string UserAgent = "dotnet global tools abplus, a cli tool for ABP framework.(github.com/personball/Abp-CLI)";

        public const string DefaultProjectTemplateName = "aspnetboilerplate/module-zero-core-template@latest";
        public const string DefaultProjectTemplateForSPA = "personball/module-zero-core-template@latest";
        public const string DefaultPlaceHolder = "AbpCompanyName.AbpProjectName";
        public const string DefaultProjectName = "AbpDemo";

        public static class Descriptions
        {
            public static class Base
            {
                public const string TemplateNameDescription = "TemplateName <GithubUserName>/<RepoName>[@<ReleaseTag>],default as 'aspnetboilerplate/module-zero-core-template@latest'.";

                public const string PlaceHolderDescription = "PlaceHolder in project template,default as 'AbpCompanyName.AbpProjectName'.";
            }

            public static class Init
            {
                public const string CommandDescription = "Init a project from project template.";
                public const string ProjectNameDescription = "Your project name, default as 'AbpDemo'.";
                public const string MpaDescription = "Is this project a Multi-Pages Application? Default as false.";
                public const string SpaTypeDescription = "Choose 'vue' for vuejs or 'ng' for angularjs or 'react' for reactjs. Any invalid value will be replaced by 'vue'.";
                public const string RenameBackupDescription = "When enabled, it will create a backup for target folder while renaming. Default as 'false'";
            }

            public static class New
            {
                public const string CommandDescription = "Should be executed in aspnet-core folder, and create new console or module in aspnet-core/src folder.";
                public const string IdentifierDescription = "";
                public const string NameDescription = "";
            }
        }
    }
}
