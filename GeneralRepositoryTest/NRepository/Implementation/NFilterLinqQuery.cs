using System.Linq;
using vlko.core.ActiveRecord;

namespace GeneralRepositoryTest.NRepository
{
    public class NFilterLinqQuery : BaseLinqQuery<NTestObject>
    {
        /// <summary>
        /// Adds the type filter.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>This for fluent.</returns>
        public NFilterLinqQuery WhereType(TypeEnum type)
        {
            Queryable = Queryable.Where(test => test.Type == type);
            return this;
        }

        /// <summary>
        /// Wheres the text start.
        /// </summary>
        /// <param name="startPattern">The start pattern.</param>
        /// <returns>This for fluent.</returns>
        public NFilterLinqQuery WhereTextStart(string startPattern)
        {
            Queryable = Queryable.Where(test => test.Text.StartsWith(startPattern));
            return this;
        }
    }
}
