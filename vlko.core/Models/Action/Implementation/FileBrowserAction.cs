using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NLog;
using vlko.core.Models.Action.ViewModel;
using vlko.core.Services;

namespace vlko.core.Models.Action
{
    public class FileBrowserAction : IFileBrowserAction
    {
        public readonly string UserDirectoryName = "user_content";
        private readonly string _basePath;
        private readonly string _baseUrlPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileBrowserAction"/> class.
        /// </summary>
        /// <param name="appInfoService">The app info service.</param>
        public  FileBrowserAction(IAppInfoService appInfoService)
        {
            _baseUrlPath = appInfoService.RootUrl.TrimEnd('/');
            _baseUrlPath += "/" + UserDirectoryName;
            _basePath = appInfoService.RootPath.TrimEnd('\\');
            _basePath += "\\" + UserDirectoryName;
        }

        /// <summary>
        /// Gets all user files.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>List of files.</returns>
        public IEnumerable<FileViewModel> GetAllUserFileInfos(string user)
        {
            List<FileViewModel> result = new List<FileViewModel>();
            string userDirectory = Path.Combine(_basePath, SanitizeFileName(user));
            if (Directory.Exists(userDirectory))
            {
                foreach (var file in new DirectoryInfo(userDirectory).EnumerateFiles())
                {
                    result.Add(new FileViewModel
                                   {
                                       Ident = file.Name,
                                       Size = file.Length,
                                       Url = _baseUrlPath + "/" + SanitizeFileName(user) + "/" + file.Name
                                   });
                }
            }
            return result;
        }

        /// <summary>
        /// Gets the file info.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="fileIdent">The file ident.</param>
        /// <returns>File info.</returns>
        public FileViewModel GetFileInfo(string user, string fileIdent)
        {
            fileIdent = SanitizeFileName(fileIdent);
            return GetAllUserFileInfos(user).FirstOrDefault(file => file.Ident == fileIdent);
        }

        /// <summary>
        /// Deletes the file.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="fileIdent">The file ident.</param>
        /// <returns>True if succeed.</returns>
        public bool DeleteFile(string user, string fileIdent)
        {
            var fileInfo = GetFileInfo(user, fileIdent);
            if (fileInfo != null)
            {
                try
                {
                    File.Delete(Path.Combine(_basePath, SanitizeFileName(user), SanitizeFileName(fileInfo.Ident)));
                    return true;
                }
                catch (Exception e)
                {
                    LogManager.GetCurrentClassLogger().ErrorException(
                        string.Format("Unable to delete file {0} for user {1}.", fileInfo.Ident, user),
                        e);
                }
            }
            return false;
        }

        /// <summary>
        /// Saves the file.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="fileIdent">The file ident.</param>
        /// <param name="fileStream">The file stream.</param>
        /// <returns>True if succeed.</returns>
        public bool SaveFile(string user, string fileIdent, Stream fileStream)
        {
            string path = Path.Combine(_basePath, SanitizeFileName(user));
            string file = Path.Combine(path, SanitizeFileName(fileIdent));
            try
            {
                Directory.CreateDirectory(path);
                using (FileStream fileWriteStream = new FileStream(file, FileMode.CreateNew))
                {
                    fileStream.CopyTo(fileWriteStream);
                    fileWriteStream.Flush();
                    fileWriteStream.Close();
                }
                return true;
            }
            catch (Exception e)
            {
                LogManager.GetCurrentClassLogger().ErrorException(
                    string.Format("Unable to create file {0} for user {1}.", file, user),
                    e);
            }
            return false;
        }

        /// <summary>
        /// Sanitizes the name of the file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>Sanitized</returns>
        public static string SanitizeFileName(string fileName)
        {
            StringBuilder result = new StringBuilder(fileName);
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                result.Replace(c.ToString(), "-");
            }
            return result.ToString();
        }
    }
}