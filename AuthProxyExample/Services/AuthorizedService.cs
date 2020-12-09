using System;
using AuthProxy;

namespace AuthProxyExample.Services
{
    [Authorize("HasBankCard")]
    public class AuthorizedService : IAuthorizedService
    {
        [Authorize("Withdraw_Money")]
        public void WithdrawMoney(double amount)
        {
            Console.WriteLine($"Withdraw money, ${amount}");
        }

        [Authorize("Deposit_Money")]
        public void DepositMoney(double amount)
        {
            Console.WriteLine($"Deposit money, ${amount}");
        }
    }
}