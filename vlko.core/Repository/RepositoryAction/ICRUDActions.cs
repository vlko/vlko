namespace vlko.core.Repository.RepositoryAction
{
	public interface ICRUDActions<T> : IFindByPkAction<T>, ICreateAction<T>, IUpdateAction<T>, IDeleteAction<T> where T : class
	{
	}
}
