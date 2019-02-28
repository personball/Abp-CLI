# Abp-CLI

ABP Command Line Tool.  

[![NuGet version](https://badge.fury.io/nu/AbpTools.svg)](https://badge.fury.io/nu/AbpTools)

This command is built following [dotnet core global tools](https://docs.microsoft.com/zh-cn/dotnet/core/tools/global-tools).  

[中文版readme](https://github.com/personball/Abp-CLI/wiki)

# Install

    dotnet tool install -g AbpTools

# Usage

## Init Solution

    abplus init YourProjectName

Example

    abplus init FamilySrv.PlanFork

## New console or module

    abplus new console -n YourProjectName -T personball/abplus-zero-template

Example

    abplus new console -n Abplus.Demo -T personball/abplus-zero-template

# HelpText

abplus

```powershell
    $> abplus --help
    1.2.0

    Usage: abplus [options] [command]

    Options:
    --version  Show version information
    --help     Show help information

    Commands:
    init       Init a project from project template.  
    new        Should be executed in aspnet-core folder,   
               and create new console or module in aspnet-core/src folder.

    Run 'abplus [command] --help' for more information about a command.

    $> abplus init --help
    Init a project from project template.
```

abplus init

```powershell
    $> abplus init --help
    Init a project from project template.

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
```

abplus new

```powershell
    l> abplus new --help
    Should be executed in aspnet-core folder, and create new console or module in aspnet-core/src folder.

    Usage: abplus new [arguments] [options]

    Arguments:
    Identifier          'console' or 'module'

    Options:
    --help              Show help information
    -n|--name           Name for 'console' like 'AbpCompanyName.AbpProjectName' or for 'module' like 'AbpCompanyName.AbpProjectName.AbpModuleName'
    -T|--template-name  TemplateName <GithubUserName>/<RepoName>[@<ReleaseTag>],default as 'aspnetboilerplate/module-zero-core-template@latest'.
    -h|--placeholder    PlaceHolder in project template,default as 'AbpCompanyName.AbpProjectName'.
```

# More features todo

1. SubCommand `abplus add` for code scaffold. 
1. SubCommand `abplus set` for command config, like default template, default Placeholder. 
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
    |-console                       //Identifier folder for 'abplus new console...'
    |-module                        //Identifier folder for 'abplus new module...'
    //other folders and files not care.  
```

# MIT
