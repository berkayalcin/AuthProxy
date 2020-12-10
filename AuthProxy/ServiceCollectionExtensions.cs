using System;
using System.Linq;
using System.Reflection;
using Castle.DynamicProxy;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace AuthProxy
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterServices(this IServiceCollection serviceCollection,
            string serviceNamePostfix)
        {
            serviceCollection.AddScoped<IInterceptor, UnitOfWorkInterceptor>();
            var assembly = Assembly.GetCallingAssembly();
            var interfaces = assembly.GetTypes()
                .Where(t => t.IsInterface && !string.IsNullOrEmpty(t.FullName) &&
                            t.FullName.EndsWith(serviceNamePostfix));
            foreach (var @interface in interfaces)
            {
                if (string.IsNullOrEmpty(@interface.FullName))
                    continue;

                var derivedClassName = @interface.Name.Substring(1);

                var serviceName = string.Join(".", @interface.FullName.Split(".").SkipLast(1)) + $".{derivedClassName}";


                var derivedClass =
                    assembly.GetType(serviceName);
                if (derivedClass == null)
                    continue;

                serviceCollection.AddScoped(derivedClass);
                serviceCollection.AddProxiedScoped(@interface, derivedClass);
            }

            return serviceCollection;
        }

        public static void AddObjectBasedAuthorization(this IServiceCollection services)
        {
            services.AddSingleton(new ProxyGenerator());
            services.AddScoped<IInterceptor, PermissionInterceptor>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            var assemblyTypes = Assembly
                .GetCallingAssembly()
                .GetTypes();
            var authorizedObjects = assemblyTypes
                .Where(t => t.IsInterface && typeof(IAuthorizedObject).IsAssignableFrom(t))
                .ToList();

            foreach (var authorizedObject in authorizedObjects)
            {
                var derivedTypes = assemblyTypes
                    .Where(t => t.IsClass && authorizedObject.IsAssignableFrom(t))
                    .ToList();

                if (!derivedTypes.Any())
                    continue;

                foreach (var derivedType in derivedTypes)
                {
                    services.AddProxiedScoped(authorizedObject, derivedType);
                }
            }
        }


        public static void AddProxiedSingleton(this IServiceCollection services, Type @interface, Type implementation)
        {
            services.AddSingleton(implementation);
            services.AddSingleton(@interface, serviceProvider =>
            {
                var proxyGenerator = serviceProvider.GetRequiredService<ProxyGenerator>();
                var actual = serviceProvider.GetRequiredService(implementation);
                var interceptors = serviceProvider.GetServices<IInterceptor>().ToArray();
                return proxyGenerator.CreateInterfaceProxyWithTarget(@interface, actual, interceptors);
            });
        }

        public static void AddProxiedTransient(this IServiceCollection services, Type @interface, Type implementation)
        {
            services.AddTransient(implementation);
            services.AddTransient(@interface, serviceProvider =>
            {
                var proxyGenerator = serviceProvider.GetRequiredService<ProxyGenerator>();
                var actual = serviceProvider.GetRequiredService(implementation);
                var interceptors = serviceProvider.GetServices<IInterceptor>().ToArray();
                return proxyGenerator.CreateInterfaceProxyWithTarget(@interface, actual, interceptors);
            });
        }


        public static void AddProxiedScoped(this IServiceCollection services, Type @interface, Type implementation)
        {
            services.AddScoped(implementation);
            services.AddScoped(@interface, serviceProvider =>
            {
                var proxyGenerator = serviceProvider.GetRequiredService<ProxyGenerator>();
                var actual = serviceProvider.GetRequiredService(implementation);
                var interceptors = serviceProvider.GetServices<IInterceptor>().ToArray();
                return proxyGenerator.CreateInterfaceProxyWithTarget(@interface, actual, interceptors);
            });
        }

        public static void AddProxiedScoped(this IServiceCollection services, Type implementation)
        {
            services.AddScoped(implementation);
            services.AddScoped(serviceProvider =>
            {
                var proxyGenerator = serviceProvider.GetRequiredService<ProxyGenerator>();
                var actual = serviceProvider.GetRequiredService(implementation);
                var interceptors = serviceProvider.GetServices<IInterceptor>().ToArray();
                return proxyGenerator.CreateClassProxyWithTarget(implementation, actual, interceptors);
            });
        }
    }
}