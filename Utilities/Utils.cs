using System;

namespace FinalSnack.Utilities
{
    public static class Utils
    {
        public static Rectangle RectangleFromCenter(Vector2 center, int width, int height)
        {
            int x = (int)MathF.Floor(center.X - width / 2f);
            int y = (int)MathF.Floor(center.Y - height / 2f);
            return new Rectangle(x, y, width, height);
        }
    }
}
