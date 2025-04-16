using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalSnack.Common
{
    /// <summary>
    /// Allows you to load assets in the appropriate place.
    /// If you implement this, this class or the base class of this class needs to have the <see cref="CachedAttribute"/> attribute.
    /// </summary>
    public interface INeedAssets
    {
        void LoadAssets();
        void UnloadAssets();
    }
}
