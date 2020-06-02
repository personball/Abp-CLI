**This is readme.md for chinese.**  

这里是中文版readme。

# Abp-CLI

ABP 命令行工具.  

本命令行工具按照[dotnet core global tools](https://docs.microsoft.com/zh-cn/dotnet/core/tools/global-tools)构建.  

# 安装

    dotnet tool install -g AbpTools

# 用法

## 初始化解决方案

    abplus init YourProjectName

例如

    abplus init FamilySrv.PlanFork

## 新建console项目或module项目

    abplus new console -n YourProjectName -T personball/abplus-zero-template

例如

    abplus new console -n Abplus.Demo -T personball/abplus-zero-template

# 帮助信息

命令

    $> abplus --help
    1.2.0

    Usage: abplus [options] [command]

    Options:
    --version  显示版本信息
    --help     显示帮助信息

    Commands:
    init       从项目模板初始化一个新解决方案。
    new        创建console项目或者module项目到'aspnet-core/src'目录中，  
               应在'aspnet-core'目录下执行。  

    运行 'abplus [command] --help' 以查看子命令帮助。

子命令

```powershell
    $> abplus init --help
    从模板初始化一个新解决方案。

    Usage: abplus init [arguments] [options]

    Arguments:
    ProjectName         项目名称，默认为'AbpDemo'。

    Options:
    --help              显示帮助信息。
    -T|--template-name  指定模板名 <GithubUserName>/<RepoName>[@<ReleaseTag>]，默认为
                        'aspnetboilerplate/module-zero-core-template@latest'。
    -h|--place-holder   指定项目模板中的名称占位符，默认为'AbpCompanyName.AbpProjectName'。
    -m                  该项目是否多页应用程序？默认为否。
    -t|--spa-type       如果是vuejs，请选择'vue'；
                        如果是angularjs，请选择'ng'；
                        如果是reactjs，请选择'react'。
                        任何非法输入都会替换成'vue'。
    -b                  重命名时开启备份。
```

```powershell
    $> abplus new --help
    创建console项目或者module项目到'aspnet-core/src'目录中，应在'aspnet-core'目录下执行。

    Usage: abplus new [arguments] [options]

    Arguments:
    Identifier          'console' or 'module'

    Options:
    --help              Show help information
    -n|--name           'console' 项目名，如'AbpCompanyName.AbpProjectName'或者   
                        'module' 项目名，如'AbpCompanyName.AbpProjectName.AbpModuleName'
    -T|--template-name  指定模板名 <GithubUserName>/<RepoName>[@<ReleaseTag>]，默认为
                        'aspnetboilerplate/module-zero-core-template@latest'。
    -h|--place-holder   指定项目模板中的名称占位符，默认为'AbpCompanyName.AbpProjectName'。
```

# 更多功能特性待添加

1. 用于代码生成的子命令：`abplus add`。
1. 用于命令默认选项配置的子命令: `abplus set`。
1. 其他子命令，欢迎通过issue提交想法.

# 项目模板库规范

任何人都可以通过fork [aspnetboilerplate/module-zero-core-template](https://github.com/aspnetboilerplate/module-zero-core-template)定义自己的**项目模板库**，并通过运行`abplus init -T YourGithubUserName/YourOwnProjectTemplateRepository@tagName`来初始化你的项目.  

但是有两点要注意:

1. 管理好**你自己的项目模板库**的版本发布（Releases），为最新版本设置**latest**标签，或在命令 -T 选项中用类似`@v1.2.3`的指定版本标签替换`@tagName`。  
1. 请保证以下**目录结构**：

```
    |                               //根目录
    |-angular                       //该目录可选  
    |-aspnet-core  
        |-src  
            |-{PlaceHolder}.Web.Mvc //这是多页项目所在目录  
    |-reactjs                       //该目录可选  
    |-vue                           //该目录可选  
    |-console                       //'abplus new console...' 子命令模板存放目录  
    |-module                        //'abplus new module...' 子命令模板存放目录    
    //其他目录和文件没有影响.  
```

# 本地模板文件存放目录

从版本1.2.0开始，命令会将从GitHub上拉取的项目模板zip包存放于用户主目录下的AppData\Roaming\Abplus目录下，例如`C:\Users\perso\AppData\Roaming\Abplus`，以避免重复拉取代码，提高执行效率。

# MIT


