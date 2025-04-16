using System;

namespace FinalSnack.Common
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class CachedAttribute : Attribute { }
}
