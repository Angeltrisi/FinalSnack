using System.Runtime.CompilerServices;

namespace FinalSnack.Utilities
{
    public static class AppUtils
    {
        public static class Accessors
        {
            [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_shouldExit")]
            public static extern ref bool GetShouldExit(Game instance);
        }
    }
}
