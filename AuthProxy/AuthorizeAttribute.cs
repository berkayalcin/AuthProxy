using System;

namespace AuthProxy
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class AuthorizeAttribute : Attribute
    {
        private readonly string[] _permissions;
        public string[] Permissions => _permissions;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="permissions">Required Permissions to Access This Method</param>
        public AuthorizeAttribute(params string[] permissions)
        {
            _permissions = permissions;
        }
    }
}