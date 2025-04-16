using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalSnack.Common
{
    public enum SimpleSlope : byte
    {
        None,
        HalfBlock,
        Ascending,
        Descending,
        HalfAscendingA,
        HalfAscendingB,
        HalfDescendingA,
        HalfDescendingB,
    }
    public enum MouseClickType : byte
    {
        None,
        Left,
        Right,
        Middle,
        Button4,
        Button5,
    }
    public enum ControllerTriggerType : byte
    {
        Left,
        Right,
    }
}
