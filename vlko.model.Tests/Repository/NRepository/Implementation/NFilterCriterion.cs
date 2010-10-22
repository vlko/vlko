using NHibernate.Criterion;
using vlko.model.Implementation.NH.Repository;

namespace vlko.model.Tests.Repository.NRepository.Implementation
{
    public class NFilterCriterion : BaseCriterionQueryAction<NTestObject>
    {
        /// <summary>
        /// Adds the type filter.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>This for fluent.</returns>
        public NFilterCriterion WhereType(TypeEnum type)
        {
            Criteria = Criteria.Add(Expression.Eq("Type", type));
            return this;
        }

        /// <summary>
        /// Wheres the text start.
        /// </summary>
        /// <param name="startPattern">The start pattern.</param>
        /// <returns>This for fluent.</returns>
        public NFilterCriterion WhereTextStart(string startPattern)
        {
            Criteria = Criteria.Add(Expression.Like("Text", startPattern, MatchMode.Start));
            return this;
        }
    }
}
