using System.Threading.Tasks;

namespace AuthProxyExample.Services
{
    public interface IIdentityService
    {
        Task SignIn(string email, string fullName);
        Task SignOut();
    }
}