using System.Linq;
using vlko.model.Repository;
using vlko.model.Repository.NH;

namespace vlko.model.Tests.Repository.NRepository.Implementation
{
	public class NFilterLinqQueryAction : BaseLinqQueryAction<NTestObject>
	{
		/// <summary>
		/// Adds the type filter.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns>This for fluent.</returns>
		public IQueryResult<NTestObject> WhereType(TypeEnum type)
		{
			return Result(Queryable.Where(test => test.Type == type));
		}

		/// <summary>
		/// Wheres the text start.
		/// </summary>
		/// <param name="startPattern">The start pattern.</param>
		/// <returns>This for fluent.</returns>
		public IQueryResult<NTestObject> WhereTextStart(string startPattern)
		{
			return Result(Queryable.Where(test => test.Text.StartsWith(startPattern)));
		}
	}
}
