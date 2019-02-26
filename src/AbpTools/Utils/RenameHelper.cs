using System;
using System.Collections.Generic;
using System.IO;
using AbpTools.Utils.Renamer;

namespace AbpTools.Utils
{
    public class RenameHelper
    {
        public static void RenameFolders(string folderToProcess, string placeHolder, string projectName, bool renameBackup, List<string> excludedSubFolders)
        {
            //Folders To Rename
            Console.WriteLine("Cleaning Sub Dirs in Project Template Folder...");
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
            var companyNamePlaceHolder = placeHolder.Split('.')[0];
            var projectNamePlaceHolder = placeHolder.Split('.')[1];

            var newNames = projectName.Split('.');
            var newCompanyName = string.Empty;
            var newProjectName = projectName;
            if (newNames.Length > 1)
            {
                newCompanyName = newNames[0];
                newProjectName = newNames[1];
            }
            Console.WriteLine($"Renaming {placeHolder} to {newCompanyName}{(newNames.Length > 1 ? "." : "")}{newProjectName}...");
            var slnRenamer = new SolutionRenamer(folderToProcess, companyNamePlaceHolder, projectNamePlaceHolder, newCompanyName, newProjectName)
            {
                CreateBackup = renameBackup
            };

            slnRenamer.Run();
        }
    }
}
