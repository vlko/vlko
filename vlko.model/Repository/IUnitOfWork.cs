namespace vlko.model.Repository
{
	/// <summary>
	/// ISession represents active connection to db/webservice/whatever.
	/// </summary>
	public interface IUnitOfWork : IUnitOfWorkContext
	{
		IUnitOfWorkContext UnitOfWorkContext { get; }
		void InitUnitOfWorkContext(IUnitOfWorkContext unitOfWorkContext);
	}
}
