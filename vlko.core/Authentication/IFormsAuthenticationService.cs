using System.ComponentModel.Composition;

namespace vlko.core.Authentication
{
	[InheritedExport]
    public interface IFormsAuthenticationService
    {
        void SignIn(string userName, bool createPersistentCookie);
        void SignOut();
    }
}