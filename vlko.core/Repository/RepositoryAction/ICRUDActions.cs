namespace vlko.core.Repository.RepositoryAction
{
	public interface ICrudCommands<T> : IFindByPkCommand<T>, ICreateCommand<T>, IUpdateCommand<T>, IDeleteCommand<T> where T : class
	{
	}
}
