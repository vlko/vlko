using System.Linq;
using vlko.core.NH.Repository;
using vlko.core.Repository;

namespace vlko.BlogModule.NH.Tests.Repository.NRepository.Implementation
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
