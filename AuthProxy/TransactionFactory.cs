using System.Transactions;

namespace AuthProxy
{
    public class TransactionFactory
    {
        public static TransactionScope Create()
        {
            return new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        }
    }
}