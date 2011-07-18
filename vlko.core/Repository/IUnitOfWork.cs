using System.ComponentModel.Composition;

namespace vlko.core.Repository
{
	/// <summary>
	/// ISession represents active connection to db/webservice/whatever.
	/// </summary>
	[InheritedExport]
	public interface IUnitOfWork : IUnitOfWorkContext
	{
		IUnitOfWorkContext UnitOfWorkContext { get; }
		void InitUnitOfWorkContext(IUnitOfWorkContext unitOfWorkContext);
	}
}
