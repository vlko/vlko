using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using System.Threading.Tasks;
using vlko.core.Roots;
using vlko.core.web.Tools;

namespace vlko.core.web.Base
{
    /// <summary>
    /// Base async controller with session.
    /// </summary>
    public abstract class BaseAsyncController<TSessionType> : Controller where TSessionType : class, DBAccess.ISession
    {
        public const string StatusRedirectMessage = "StatusRedirectMessage";
        public TSessionType DBSession { get; set; }
        /// <summary>
        /// Gets the user info (returns null if user not authenticated).
        /// </summary>
        public User CurrentUser { get; protected set; }

        /// <summary>
        /// Allowed database idents for current controller.
        /// </summary>
        public virtual string[] AllowedDatabaseIdents
        {
            get { return new string[] { }; }
        }

        /// <summary>
        /// Current database ident to identify specific database.
        /// </summary>
        public string CurrentDatabaseIdent { get; private set; }

        /// <summary>
        /// Use status result with storing message in HttpContext.Items (to get message use GetStatusRedirectMessage())
        /// </summary>
        protected virtual bool UseStatusRedirectMessage { get => HttpContext.Features.Get<IStatusCodePagesFeature>() != null; }

        /// <summary>
        /// Detects the database ident for current request .
        /// </summary>
        /// <returns></returns>
        protected virtual string DetectDatabaseIdent(HttpRequest requestContext)
        {
            // detect subdomain as db ident
            return DatabaseIdentResolvingStategies.ResolveBySubdomain(Request);
        }

        /// <summary>
        /// Loads the user specific data with default db connection for current request.
        /// Returns true if we should redirect to self to apply actual claims change
        /// </summary>
        protected abstract Task<bool> LoadUserSpecificData(ActionExecutingContext filterContext);
        protected abstract TSessionType InitDBSession(string dbIdent);
        protected abstract IActionResult AdditionalProtectionRedirect();

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var detectedDbIdent = DetectDatabaseIdent(context.HttpContext.Request);
            if (detectedDbIdent != null && AllowedDatabaseIdents.Contains(detectedDbIdent))
            {
                CurrentDatabaseIdent = detectedDbIdent;
                ViewBag.CurrentDatabaseIdent = CurrentDatabaseIdent;
            }

            if (context.HttpContext.User.Identity.IsAuthenticated)
            {
                // if true, then redirect to self to apply actual claims change
                if (await LoadUserSpecificData(context))
                {
                    context.Result = new RedirectResult(context.HttpContext.Request.Path + context.HttpContext.Request.QueryString);
                }
            }

            var hasAdditionalProtectionRedirect = AdditionalProtectionRedirect();
            if (hasAdditionalProtectionRedirect != null)
            {
                context.Result = hasAdditionalProtectionRedirect;
                return;
            }
            DBSession = InitDBSession(CurrentDatabaseIdent);
            if (context.Result == null)
            {
                await next();
            }
            DisposeDBSession();
        }

        /// <summary>
        /// Returns not found result and stores message in HttpContext.Items (to get message use GetStatusRedirectMessage())
        /// </summary>
        public ActionResult NotFound(string notFoundMessage)
        {
            if (UseStatusRedirectMessage && !Request.IsAjaxRequest())
            {
                HttpContext.Items[StatusRedirectMessage] = notFoundMessage;
                return base.NotFound();
            }
            return base.NotFound(notFoundMessage);
        }

        /// <summary>
        /// Returns unauthorized result and stores message in HttpContext.Items (to get message use GetStatusRedirectMessage())
        /// </summary>
        public ActionResult Unauthorized(string unauthorizedMessage)
        {
            if (UseStatusRedirectMessage && !Request.IsAjaxRequest())
            {
                HttpContext.Items[StatusRedirectMessage] = unauthorizedMessage;
                return base.Unauthorized();
            }
            return base.Unauthorized(unauthorizedMessage);
        }

        /// <summary>
        /// Returns status code result and stores message in HttpContext.Items (to get message use GetStatusRedirectMessage())
        /// </summary>
        public ActionResult StatusCode(int statusCode, string statusMessage)
        {
            if (UseStatusRedirectMessage && !Request.IsAjaxRequest())
            {
                HttpContext.Items[StatusRedirectMessage] = statusMessage;
                return base.StatusCode(statusCode);
            }
            return base.StatusCode(statusCode, statusMessage);
        }


        /// <summary>
        /// Returns status redirect message from NotFound or Unauthorized result
        /// </summary>
        public string GetStatusRedirectMessage()
        {
            if (HttpContext.Items.ContainsKey(StatusRedirectMessage))
            {
                return HttpContext.Items[StatusRedirectMessage] as string;
            }
            return null;
        }

        public void DisposeDBSession()
        {
            DBSession?.Dispose();
            DBSession = null;
        }
    }
}
