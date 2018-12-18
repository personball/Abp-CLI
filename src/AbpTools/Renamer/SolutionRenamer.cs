using System;
using System.IO;
using System.Linq;
using System.Text;

namespace AbpTools.Renamer
{
    /// <summary>
    /// Used to rename a solution, code from https://gist.github.com/hikalkan/014eecdae7c8d2677b192fcebcf5ba29?tdsourcetag=s_pcqq_aiomsg
    /// </summary>
    public class SolutionRenamer
    {
        /// <summary>
        /// Create a backup of the solution before renaming?
        /// Default: true.
        /// </summary>
        public bool CreateBackup { get; set; }

        private readonly string _folder;

        private readonly string _companyNamePlaceHolder;
        private readonly string _projectNamePlaceHolder;

        private readonly string _companyName;
        private readonly string _projectName;

        /// <summary>
        /// Creates a new <see cref="SolutionRenamer"/>.
        /// </summary>
        /// <param name="folder">Solution folder (which includes .sln file)</param>
        /// <param name="companyNamePlaceHolder">Company name place holder (can be null if there is not a company name place holder)</param>
        /// <param name="projectNamePlaceHolder">Project name place holder</param>
        /// <param name="companyName">Company name. Can be null if new solution will not have a company name prefix. Should be null if <see cref="companyNamePlaceHolder"/> is null</param>
        /// <param name="projectName">Project name</param>
        public SolutionRenamer(string folder, string companyNamePlaceHolder, string projectNamePlaceHolder, string companyName, string projectName)
        {
            if (string.IsNullOrWhiteSpace(companyName))
            {
                companyName = null;
            }

            if (!Directory.Exists(folder))
            {
                throw new Exception("There is no folder: " + folder);
            }

            folder = folder.Trim('\\');

            if (companyNamePlaceHolder == null && companyName != null)
            {
                throw new Exception("Can not set companyName if companyNamePlaceHolder is null.");
            }

            _folder = folder;

            _companyNamePlaceHolder = companyNamePlaceHolder;
            _projectNamePlaceHolder = projectNamePlaceHolder ?? throw new ArgumentNullException("projectNamePlaceHolder");

            _companyName = companyName;
            _projectName = projectName ?? throw new ArgumentNullException("projectName");

            CreateBackup = true;
        }

        public void Run()
        {
            if (CreateBackup)
            {
                Backup();
            }

            if (_companyNamePlaceHolder != null)
            {
                if (_companyName != null)
                {
                    RenameDirectoryRecursively(_folder, _companyNamePlaceHolder, _companyName);
                    RenameAllFiles(_folder, _companyNamePlaceHolder, _companyName);
                    ReplaceContent(_folder, _companyNamePlaceHolder, _companyName);
                }
                else
                {
                    RenameDirectoryRecursively(_folder, _companyNamePlaceHolder + "." + _projectNamePlaceHolder, _projectNamePlaceHolder);
                    RenameAllFiles(_folder, _companyNamePlaceHolder + "." + _projectNamePlaceHolder, _projectNamePlaceHolder);
                    ReplaceContent(_folder, _companyNamePlaceHolder + "." + _projectNamePlaceHolder, _projectNamePlaceHolder);
                }
            }

            RenameDirectoryRecursively(_folder, _projectNamePlaceHolder, _projectName);
            RenameAllFiles(_folder, _projectNamePlaceHolder, _projectName);
            ReplaceContent(_folder, _projectNamePlaceHolder, _projectName);
        }

        private void Backup()
        {
            var normalBackupFolder = _folder + "-BACKUP";
            var backupFolder = normalBackupFolder;

            int backupNo = 1;
            while (Directory.Exists(backupFolder))
            {
                backupFolder = normalBackupFolder + "-" + backupNo;
                ++backupNo;
            }

            DirectoryCopy(_folder, backupFolder, true);
        }

        private static void RenameDirectoryRecursively(string directoryPath, string placeHolder, string name)
        {
            var subDirectories = Directory.GetDirectories(directoryPath, "*.*", SearchOption.TopDirectoryOnly);
            foreach (var subDirectory in subDirectories)
            {
                var newDir = subDirectory;
                if (subDirectory.Contains(placeHolder))
                {
                    newDir = subDirectory.Replace(placeHolder, name);
                    Directory.Move(subDirectory, newDir);
                }

                RenameDirectoryRecursively(newDir, placeHolder, name);
            }
        }

        private static void RenameAllFiles(string directory, string placeHolder, string name)
        {
            var files = Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                if (file.Contains(placeHolder))
                {
                    File.Move(file, file.Replace(placeHolder, name));
                }
            }
        }

        private static void ReplaceContent(string rootPath, string placeHolder, string name)
        {
            var skipExtensions = new[]
                                 {
                                     ".exe", ".dll", ".bin", ".suo", ".png", "jpg", "jpeg", ".pdb", ".obj"
                                 };

            var files = Directory.GetFiles(rootPath, "*.*", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                if (skipExtensions.Contains(Path.GetExtension(file)))
                {
                    continue;
                }

                var fileSize = GetFileSize(file);
                if (fileSize < placeHolder.Length)
                {
                    continue;
                }

                var encoding = GetEncoding(file);

                var content = File.ReadAllText(file, encoding);
                var newContent = content.Replace(placeHolder, name);
                if (newContent != content)
                {
                    File.WriteAllText(file, newContent, encoding);
                }
            }
        }

        private static long GetFileSize(string file)
        {
            return new FileInfo(file).Length;
        }

        private static Encoding GetEncoding(string filename)
        {
            // Read the BOM
            var bom = new byte[4];
            using (var file = new FileStream(filename, FileMode.Open)) file.Read(bom, 0, 4);

            // Analyze the BOM
            if (bom[0] == 0x2b && bom[1] == 0x2f && bom[2] == 0x76) return Encoding.UTF7;
            if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf) return Encoding.UTF8;
            if (bom[0] == 0xff && bom[1] == 0xfe) return Encoding.Unicode; //UTF-16LE
            if (bom[0] == 0xfe && bom[1] == 0xff) return Encoding.BigEndianUnicode; //UTF-16BE
            if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff) return Encoding.UTF32;
            return Encoding.ASCII;
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] dirs = dir.GetDirectories();

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            // If the destination directory doesn't exist, create it. 
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location. 
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }
    }
}
