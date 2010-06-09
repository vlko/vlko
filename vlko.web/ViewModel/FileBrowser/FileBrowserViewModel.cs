using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using vlko.core;
using vlko.core.Models.Action.ViewModel;

namespace vlko.web.ViewModel.FileBrowser
{
    public class FileBrowserViewModel
    {
        /// <summary>
        /// Gets or sets the ident.
        /// </summary>
        /// <value>The ident.</value>
        [Display(ResourceType = typeof(ModelResources), Name = "FileIdent")]
        [StringLength(80, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "FileIdentRequireError")]
        public string Ident { get; set; }

        public HttpPostedFileWrapper File { get; set; }

        /// <summary>
        /// Max file upload size (in bytes).
        /// </summary>
        public const int MaxFileSize = 2097152;

        /// <summary>
        /// Allowed extensions for file browser
        /// </summary>
        public static readonly string[] AllowedExtensions = new[] {"*.jpg", "*.jpg", "*.png", "*.gif", "*.zip"};

        /// <summary>
        /// Gets or sets the user files.
        /// </summary>
        /// <value>The user files.</value>
        public IEnumerable<FileViewModel> UserFiles { get; set; }
    }
}