using System.Linq;
using vlko.core.ActiveRecord;

namespace GeneralRepositoryTest.NRepository
{
    public class NFilterLinqQueryAction : BaseLinqQueryAction<NTestObject>
    {
        /// <summary>
        /// Adds the type filter.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>This for fluent.</returns>
        public NFilterLinqQueryAction WhereType(TypeEnum type)
        {
            Queryable = Queryable.Where(test => test.Type == type);
            return this;
        }

        /// <summary>
        /// Wheres the text start.
        /// </summary>
        /// <param name="startPattern">The start pattern.</param>
        /// <returns>This for fluent.</returns>
        public NFilterLinqQueryAction WhereTextStart(string startPattern)
        {
            Queryable = Queryable.Where(test => test.Text.StartsWith(startPattern));
            return this;
        }
    }
}
