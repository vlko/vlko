using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using GenericRepository;

namespace vlko.core.Base
{
    /// <summary>
    /// Base controller with session.
    /// </summary>
    public class BaseController : Controller
    {
        /// <summary>
        /// Executes the specified request context.
        /// </summary>
        /// <param name="requestContext">The request context.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="requestContext"/> parameter is null.</exception>
        protected override void Execute(RequestContext requestContext)
        {
            using (RepositoryFactory.StartUnitOfWork())
            {
                base.Execute(requestContext);
            }
        }
    }
}
