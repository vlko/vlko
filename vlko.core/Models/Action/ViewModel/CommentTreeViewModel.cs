using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace vlko.core.Models.Action.ViewModel
{
	public class CommentTreeViewModel : CommentViewModel
	{


		/// <summary>
		/// Initializes a new instance of the <see cref="CommentTreeViewModel"/> class.
		/// </summary>
		public CommentTreeViewModel(Guid id, Guid topCommentId, Guid parentCommentId, string name, DateTime createdDate, string text, int version, User owner, string anonymousName, string clientIp, int level)
			:base(id, name, createdDate, text, version, owner, anonymousName, clientIp, level)
		{
			TopCommentId = topCommentId;
			ParentCommentId = parentCommentId;
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
