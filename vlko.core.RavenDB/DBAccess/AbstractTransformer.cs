using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace vlko.core.RavenDB.DBAccess
{
    public abstract class AbstractTransformer<TFrom, TProjection> where TFrom : class
    {
        public Func<IQueryable<TFrom>, IQueryable<TProjection>> TransformResult { get; set; }
        public Func<IQueryable<TFrom>, IDictionary<string, string>, IQueryable<TProjection>> TransformResultWithParameters { get; set; }

        public Action<IDictionary<string, string>> PrepareDefaultParameters { get; set; }

        protected void SetValueIfNotExists(IDictionary<string, string> parameters, string key, string defaultValue)
        {
            if (!parameters.ContainsKey(key))
            {
                parameters[key] = defaultValue;
            }
        }
    }

}
