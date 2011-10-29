﻿using System.IO;
using System.Web;
using System.Web.Mvc;
using vlko.BlogModule.Commands;
using vlko.BlogModule.Commands.ViewModel;
using vlko.core.Base;
using vlko.core.InversionOfControl;
using vlko.core.ValidationAtribute;
using vlko.web.Areas.Admin.ViewModel.FileBrowser;

namespace vlko.web.Areas.Admin.Controllers
{
	[Authorize]
	[AreaCheck("Admin")]
	public class FileBrowserController : BaseController
	{
		/// <summary>
		/// URL: FileBrowser/Index
		/// </summary>
		/// <returns>Action result.</returns>
		public ActionResult Index()
		{
			var files = IoC.Resolve<IFileBrowserCommands>().GetAllUserFileInfos(User.Identity.Name);
			return ViewWithAjax(new FileBrowserViewModel
							{
								UserFiles = files
							});
		}

		/// <summary>
		/// URL: FileBrowser/Delete
		/// </summary>
		/// <param name="ident">The ident.</param>
		/// <returns>Action result.</returns>
		public ActionResult Delete(string ident)
		{
			return ViewWithAjax(
				IoC.Resolve<IFileBrowserCommands>()
					.GetFileInfo(User.Identity.Name, ident));
		}

		/// <summary>
		/// URL: FileBrowser/Delete
		/// </summary>
		/// <param name="model">The model.</param>
		/// <returns>Action result.</returns>
		[HttpPost]
		public ActionResult Delete(FileViewModel model)
		{
			if (!IoC.Resolve<IFileBrowserCommands>()
				.DeleteFile(User.Identity.Name, model.Ident))
			{
				ModelState.AddModelError(string.Empty, vlko.BlogModule.ModelResources.FileDeleteFailedError);
				return ViewWithAjax(model);
			}
			return RedirectToActionWithAjax("Index");
		}

		/// <summary>
		/// URL: FileBrowser/Upload
		/// </summary>
		/// <returns>Action result.</returns>
		[HttpPost]
		public ActionResult Upload(HttpPostedFileBase file)
		{
			var fileBrowserActions = IoC.Resolve<IFileBrowserCommands>();

			var model = new FileBrowserViewModel
							{
								Ident = Request.Form["Ident"]
							};

			// test content length
			if (file.ContentLength > FileBrowserViewModel.MaxFileSize)
			{
				ModelState.AddModelError<FileBrowserViewModel>(item => item.Ident,
				                                               string.Format(vlko.BlogModule.ModelResources.FileTooBigError, FileBrowserViewModel.MaxFileSize));
			}
			else
			{
				// test if ident specified
				if (string.IsNullOrWhiteSpace(model.Ident))
				{
					ModelState.AddModelError<FileBrowserViewModel>(item => item.Ident, vlko.BlogModule.ModelResources.FileIdentRequireError);
				}
				else
				{
					// get new ident based on user ident and uploaded file extension
					string fileIdent = model.Ident + Path.GetExtension(file.FileName);

					// check if file exists
					var existingItem = fileBrowserActions.GetFileInfo(User.Identity.Name, fileIdent);
					if (existingItem != null)
					{
						ModelState.AddModelError<FileBrowserViewModel>(item => item.Ident, string.Format(vlko.BlogModule.ModelResources.FileIdentExistsError, fileIdent));
					}
					else
					{
						// Save file
						if (!fileBrowserActions.SaveFile(User.Identity.Name, fileIdent, file.InputStream))
						{
							ModelState.AddModelError(string.Empty, vlko.BlogModule.ModelResources.FileUploadFailedError);
						}
						else
						{
							model.Ident = null;
						}
					}
				}
			}

			// load user files
			model.UserFiles = fileBrowserActions.GetAllUserFileInfos(User.Identity.Name);

			// return normal view, for ajax we have iframe technique so normal result is needed
			return View("Index", model);
		}

	}
}
