using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using vlko.BlogModule.Action.ViewModel;

namespace vlko.BlogModule.Action
{
	[InheritedExport]
    public interface IFileBrowserAction
    {
        /// <summary>
        /// Gets all user files.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>List of files.</returns>
        IEnumerable<FileViewModel> GetAllUserFileInfos(string user);

        /// <summary>
        /// Gets the file info.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="fileIdent">The file ident.</param>
        /// <returns>File info.</returns>
        FileViewModel GetFileInfo(string user, string fileIdent);

        /// <summary>
        /// Deletes the file.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="fileIdent">The file ident.</param>
        /// <returns>True if succeed.</returns>
        bool DeleteFile(string user, string fileIdent);

        /// <summary>
        /// Saves the file.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="fileIdent">The file ident.</param>
        /// <param name="fileStream">The file stream.</param>
        /// <returns>True if succeed.</returns>
        bool SaveFile(string user, string fileIdent, Stream fileStream);
    }
}
