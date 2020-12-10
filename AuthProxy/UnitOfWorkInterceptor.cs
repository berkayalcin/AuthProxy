using System.Reflection;
using System.Threading.Tasks;
using Castle.DynamicProxy;

namespace AuthProxy
{
    public class UnitOfWorkInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            if (invocation.MethodInvocationTarget.GetCustomAttribute(typeof(UnitOfWorkAttribute)) == null)
            {
                invocation.Proceed();
                return;
            }

            var scope = TransactionFactory.Create();
            invocation.Proceed();
            if (invocation.ReturnValue is Task result && result.IsCompletedSuccessfully)
            {
                scope.Complete();
                return;
            }

            scope.Dispose();
        }
    }
}