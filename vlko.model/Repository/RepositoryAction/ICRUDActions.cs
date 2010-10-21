namespace vlko.model.Repository.RepositoryAction
{
	public interface ICRUDActions<T> : IFindByPkAction<T>, ICreateAction<T>, ISaveAction<T>, IDeleteAction<T> where T : class
	{
	}
}
