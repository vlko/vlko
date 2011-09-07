using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NLog;
using vlko.BlogModule.Commands;
using vlko.BlogModule.Commands.ViewModel;
using vlko.core.Services;
using System.ComponentModel.Composition;

namespace vlko.BlogModule.RavenDB.Commands
{
    public class FileBrowserCommands : IFileBrowserCommands
    {
    	private readonly IAppInfoService _appInfo;
    	public readonly string UserDirectoryName = "user_content";

		/// <summary>
		/// Gets the base path.
		/// </summary>
    	public string BasePath
    	{
    		get
    		{
				var result = _appInfo.RootPath.TrimEnd('\\');
				result += "\\" + UserDirectoryName;
    			return result;
    		}
    	}

		/// <summary>
		/// Gets the base URL path.
		/// </summary>
		public string BaseUrlPath
		{
			get
			{
				var result = _appInfo.RootUrl.TrimEnd('/');
				result += "/" + UserDirectoryName;
				return result;
			}
		}

    	/// <summary>
		/// Initializes a new instance of the <see cref="FileBrowserCommands"/> class.
		/// </summary>
		/// <param name="appInfo">The app info.</param>
		[ImportingConstructor]
		public FileBrowserCommands(IAppInfoService appInfo)
		{
			_appInfo = appInfo;
		}

        /// <summary>
        /// Gets all user files.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>List of files.</returns>
        public IEnumerable<FileViewModel> GetAllUserFileInfos(string user)
        {
            List<FileViewModel> result = new List<FileViewModel>();
            string userDirectory = Path.Combine(BasePath, SanitizeFileName(user));
            if (Directory.Exists(userDirectory))
            {
                foreach (var file in new DirectoryInfo(userDirectory).EnumerateFiles())
                {
                    result.Add(new FileViewModel
                                   {
                                       Ident = file.Name,
                                       Size = file.Length,
                                       Url = BaseUrlPath + "/" + SanitizeFileName(user) + "/" + file.Name
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
                    File.Delete(Path.Combine(BasePath, SanitizeFileName(user), SanitizeFileName(fileInfo.Ident)));
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
            string path = Path.Combine(BasePath, SanitizeFileName(user));
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