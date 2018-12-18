# Abp-CLI

ABP Command Line Tool.  

[![NuGet version](https://badge.fury.io/nu/AbpTools.svg)](https://badge.fury.io/nu/AbpTools)

This command is built following [dotnet core global tools](https://docs.microsoft.com/zh-cn/dotnet/core/tools/global-tools).  

[中文版readme](https://github.com/personball/Abp-CLI/wiki)

# Install

    dotnet tool install -g AbpTools

# Usage

    abplus init YourProjectName

Example

    abplus init FamilySrv.PlanFork

# HelpText

abplus

    $> abplus --help
    1.0.3

    Usage: abplus [options] [command]

    Options:
    --version  Show version information
    --help     Show help information

    Commands:
    init       Init a project from project template.

    Run 'abplus [command] --help' for more information about a command.

    $> abplus init --help
    Init a project from project template.

abplus init

    Usage: abplus init [arguments] [options]

    Arguments:
    ProjectName         Your project name, default as 'AbpDemo'.

    Options:
    --help              Show help information
    -T|--template-name  TemplateName <GithubUserName>/<RepoName>[@<ReleaseTag>], default as 
                        'aspnetboilerplate/module-zero-core-template@latest'.
    -h|--place-holder   PlaceHolder in project template,default as 'AbpCompanyName.AbpProjectName'.
    -m                  Is this project a Multi-Pages Application? Default as false.
    -t|--spa-type       Choose 'vue' for vuejs or 'ng' for angularjs or 'react' for reactjs.
                        Any invalid value will be replaced by 'vue'.
    -b                  Rename Backup

# More features todo

1. SubCommand `abplus add` for code scaffold. 
1. Other SubCommand.

# Project Template Repository Specification

Everyone can custom your own **Project Template Repository** by fork [aspnetboilerplate/module-zero-core-template](https://github.com/aspnetboilerplate/module-zero-core-template), and use `abplus init -T YourGithubUserName/YourOwnProjectTemplateRepository@tagName` to init your project.  

But there are two notices:

1. Manage releases of **Your Own Project Template Repository**, set a **latest** tag to latest release, or replace `@tagName` in command option -T like `@v1.2.3`  
1. Keep the **Folder Structure** as below:

```
    |                               //root
    |-angular                       //optional  
    |-aspnet-core  
        |-src  
            |-{PlaceHolder}.Web.Mvc //MPA project  
    |-reactjs                       //optional  
    |-vue                           //optional  
    //other folders and files not care.  
```

# MIT
