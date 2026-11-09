using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using MyStore.Models;

namespace MyStore.Filters
{
    public class AuthorizeRoleAttribute : AuthorizeAttribute
    {
        private readonly string _role;

        public AuthorizeRoleAttribute() : this(null) { }

        public AuthorizeRoleAttribute(string role)
        {
            _role = role;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext == null || httpContext.User == null || !httpContext.User.Identity.IsAuthenticated)
                return false;

            if (string.IsNullOrEmpty(_role))
                return true;

            var authCookie = httpContext.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie == null) return false;

            var ticket = FormsAuthentication.Decrypt(authCookie.Value);
            if (ticket == null) return false;

            return !string.IsNullOrEmpty(ticket.UserData) &&
                   ticket.UserData.Split(',')[0].Trim().Equals(_role, System.StringComparison.OrdinalIgnoreCase);
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);

            if (filterContext.Result is HttpUnauthorizedResult)
            {
                filterContext.Result = new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary
                {
                    { "area", _role == UserRoles.Admin ? "Admin" : "" },
                    { "controller", "Account" },
                    { "action", "Login" },
                    { "returnUrl", filterContext.HttpContext.Request.RawUrl }
                });
            }
        }
    }
}
