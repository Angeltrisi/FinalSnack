using FinalSnack.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace FinalSnack.Core
{
    public static class GameMeta
    {
        public static void Initialize()
        {
            Type[] types = Main.Code.GetTypes();
            for (int i = 0; i < types.Length; i++)
            {
                Type t = types[i];

                if (t.IsAbstract)
                    continue;

                Attribute[] attributes = [.. t.GetCustomAttributes()];
                
                for (int j = 0; j < attributes.Length; j++)
                {
                    Attribute attr = attributes[j];
                    
                    Type attrType = attr.GetType();

                    if (attrType == typeof(CachedAttribute))
                    {
                        GameCache.Register(t);
                    }
                }
            }
        }
    }
    public static class GameCache
    {
        private static Dictionary<Type, object> _cache = [];
        internal static void Register(Type type)
        {
            _cache[type] = Activator.CreateInstance(type);
        }
        internal static void LoadAssets()
        {
            foreach (var value in _cache.Values)
            {
                if (value is INeedAssets assetRequester)
                {
                    assetRequester.LoadAssets();
                }
            }
        }
        internal static void UnloadAssets()
        {
            foreach (var value in _cache.Values)
            {
                if (value is INeedAssets assetRequester)
                {
                    assetRequester.UnloadAssets();
                }
            }
        }
        public static object Get(Type type)
        {
            if (_cache.TryGetValue(type, out object obj))
                return obj;
            Debug.WriteLine($"Attempted to get instance of Type {type.FullName}, but it wasn't found in the cache. It is likely that this Type does not implement {nameof(CachedAttribute)}.");
            return null;
        }
        public static T Get<T>() where T : class => Get(typeof(T)) as T;
    }
}
