using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Caya.Framework.Core
{
    public static class IModuleFinder
    {
        public static IReadOnlyList<IModule> GetModules()
        {
            var moduleType = Assembly.GetEntryAssembly()?.GetTypes().FirstOrDefault(item => typeof(IModule).IsAssignableFrom(item));
            var array = Array.Empty<Type>();
            GetModules<IModule>(moduleType, ref array);
            return array.Select(item => Activator.CreateInstance(item) as IModule).ToImmutableList();
        }

        private static void GetDependencyModules(Type moduleType, ref Type[] array)
        {
            var types = moduleType.GetCustomAttribute<DependsOnAttribute>()?.TypeCollection ?? Array.Empty<Type>();
            foreach(var type in types)
            {
                GetDependencyModules(type, ref array);
            }
            if (!array.Contains(moduleType) && typeof(IModule).IsAssignableFrom(moduleType))
            {
                array = array.Append(moduleType).ToArray();
            }
        }

        public static IReadOnlyList<IMiddlewareModule> GetMiddlewareModules()
        {
            var moduleType = Assembly.GetEntryAssembly()?.GetTypes().FirstOrDefault(item => typeof(IModule).IsAssignableFrom(item));
            var array = Array.Empty<Type>();
            GetModules<IMiddlewareModule>(moduleType, ref array);
            return array.Select(item => Activator.CreateInstance(item) as IMiddlewareModule).OrderBy(item => item.Order).ToImmutableList();
        }

        private static void GetDependencyMiddlewareModules(Type moduleType, ref Type[] array)
        {
            var types = moduleType.GetCustomAttribute<DependsOnAttribute>()?.TypeCollection ?? Array.Empty<Type>();
            foreach (var type in types)
            {
                GetDependencyMiddlewareModules(type, ref array);
            }
            if (!array.Contains(moduleType) && typeof(IMiddlewareModule).IsAssignableFrom(moduleType))
            {
                array = array.Append(moduleType).ToArray();
            }
        }

        public static IReadOnlyList<ILifeTimeModule> GetAppLifetimeModules()
        {
            var moduleType = Assembly.GetEntryAssembly()?.GetTypes().FirstOrDefault(item => typeof(IModule).IsAssignableFrom(item));
            var array = Array.Empty<Type>();
            GetModules<ILifeTimeModule>(moduleType, ref array);
            return array.Select(item => Activator.CreateInstance(item) as ILifeTimeModule).ToImmutableList();
        }

        private static void GetModules<T>(Type moduleType, ref Type[] array) where T : IModule
        {
            var types = moduleType.GetCustomAttribute<DependsOnAttribute>()?.TypeCollection ?? Array.Empty<Type>();
            foreach (var type in types)
            {
                GetModules<T>(type, ref array);
            }
            if (!array.Contains(moduleType) && typeof(T).IsAssignableFrom(moduleType))
            {
                array = array.Append(moduleType).ToArray();
            }
        }
    }
}
