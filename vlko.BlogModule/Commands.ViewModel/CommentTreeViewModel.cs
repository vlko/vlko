using System;
using System.Collections.Generic;

namespace vlko.BlogModule.Commands.ViewModel
{
	public class CommentTreeViewModel : CommentViewModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CommentTreeViewModel"/> class.
		/// </summary>
		public CommentTreeViewModel()
		{
			ChildNodes = new List<CommentTreeViewModel>();
		}
		/// <summary>
		/// Gets the child nodes.
		/// </summary>
		/// <value>The child nodes.</value>
		public IEnumerable<CommentTreeViewModel> ChildNodes { get; private set; }

		/// <summary>
		/// Adds the child node.
		/// </summary>
		/// <param name="childNode">The child node.</param>
		public void AddChildNode(CommentTreeViewModel childNode)
		{
			((List<CommentTreeViewModel>)ChildNodes).Add(childNode);
		}

		/// <summary>
		/// Gets or sets the top comment id.
		/// </summary>
		/// <value>The top comment id.</value>
		public Guid TopCommentId { get; set; }

		/// <summary>
		/// Gets or sets the parent comment id.
		/// </summary>
		/// <value>The parent comment id.</value>
		public Guid? ParentCommentId { get; set; }
	}
}
