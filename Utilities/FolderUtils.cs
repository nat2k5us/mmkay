using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    using System.IO;

    using Utilities.FolderPicker;

    public static class FolderUtils
    {
        public static void ReindexFilesInFolder(IntPtr handleIntPtr)
        {
            var dialog = new FolderPickerEx
            {
                InitialDirectory = @"C:\\",
                Title = "Select a folder"
            };
            if (dialog.Show(handleIntPtr))
            {
                DirectoryInfo d = new DirectoryInfo(dialog.FileName);
                FileInfo[] infos = d.GetFiles();

                var tempPath = Path.Combine(dialog.FileName, "Temp");
                // move all files into a temp Directory
                foreach (var fileInfo in infos)
                {
                    Directory.CreateDirectory(tempPath);
                    string combined = Path.Combine(tempPath, Path.GetFileName(fileInfo.Name));
                    File.Move(fileInfo.FullName, combined);
                    File.Delete(fileInfo.FullName);

                }
                // Move them back into this folder with a new name
                DirectoryInfo dTemp = new DirectoryInfo(tempPath);
                FileInfo[] infosTemp = dTemp.GetFiles();
                int idx = 0;
                foreach (var fileInfo in infosTemp)
                {
                    var targetFile = $"{idx++}{fileInfo.Extension}";
                    var target = Path.Combine(dialog.FileName, targetFile);
                    File.Move(fileInfo.FullName, target);
                    File.Delete(fileInfo.FullName);
                }

                Directory.Delete(tempPath);
            }
        }
    }
}
