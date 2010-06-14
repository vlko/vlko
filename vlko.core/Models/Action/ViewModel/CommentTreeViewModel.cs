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
		public CommentTreeViewModel(Guid id, string name, DateTime createdDate, string text, int version, User owner, string anonymousName, string clientIp, int level)
			:base(id, name, createdDate, text, version, owner, anonymousName, clientIp, level)
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
	}
}
