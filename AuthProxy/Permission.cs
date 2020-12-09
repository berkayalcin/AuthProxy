using System;
using System.IO;
using System.Security.Claims;

namespace AuthProxy
{
    public class Permission
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}