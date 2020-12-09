using AuthProxy;

namespace AuthProxyExample.Services
{
    public interface IAuthorizedService : IAuthorizedObject
    {
        void WithdrawMoney(double amount);
        void DepositMoney(double amount);
    }
}