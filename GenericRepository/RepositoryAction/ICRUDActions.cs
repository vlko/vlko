using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenericRepository.RepositoryAction
{
	public interface ICRUDActions<T> : IFindByPkAction<T>, ICreateAction<T>, ISaveAction<T>, IDeleteAction<T> where T : class
	{
	}
}
