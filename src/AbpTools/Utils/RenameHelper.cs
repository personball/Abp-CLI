using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AbpTools.Utils.Renamer;

namespace AbpTools.Utils
{
    public class RenameHelper
    {
        public static void RenameFolders(
            string folderToProcess, string placeholder, string projectName, bool renameBackup, List<string> excludedSubFolders)
        {
            //Folders To Rename
            if (excludedSubFolders.Any())
            {
                Console.WriteLine("Cleaning Sub Dirs in Project Template Folder...");
            }

            //Delete ExcludeFolders
            foreach (var excludeFolder in excludedSubFolders)
            {
                var directoryToDel = Path.Combine(folderToProcess, excludeFolder);
                if (Directory.Exists(directoryToDel))
                {
                    Console.WriteLine($"Exclude SubFolder:{directoryToDel}");
                    Directory.Delete(directoryToDel, true);
                }
            }

            //Rename Folder
            (var companyNamePlaceholder, var projectNamePlaceholder, var moduleNamePlaceholder) = placeholder.NameParse();
            (var newCompanyName, var newProjectName, var newModuleName) = projectName.NameParse();

            Console.WriteLine($"Renaming {placeholder} to {projectName}...");
            var slnRenamer = new SolutionRenamer(folderToProcess,
                companyNamePlaceholder, projectNamePlaceholder, moduleNamePlaceholder,
                newCompanyName, newProjectName, newModuleName)
            {
                CreateBackup = renameBackup
            };

            slnRenamer.Run();
        }
    }
}
