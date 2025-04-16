using System;

namespace FinalSnack.Utilities
{
    public static class Extensions
    {
        public static float Delta(this GameTime gt) => (float)gt.ElapsedGameTime.TotalSeconds;
        public static Rectangle Inflated(this Rectangle r, int halfWidth, int halfHeight)
        {
            return new(r.X - halfWidth, r.Y - halfHeight, r.Width + (halfWidth * 2), r.Height + (halfHeight * 2));
        }
        public static Point TopRight(this Rectangle r) => new(r.Right, r.Y);
        public static Point BottomRight(this Rectangle r) => new(r.Right, r.Bottom);
        public static Point BottomLeft(this Rectangle r) => new(r.Left, r.Bottom);
        public static bool HasNaNs(this Vector2 vec)
        {
            if (float.IsNaN(vec.X))
                return true;
            return float.IsNaN(vec.Y);
        }
        public static Vector2 SafeNormalize(this Vector2 v)
        {
            if (v == Vector2.Zero || v.HasNaNs())
                return Vector2.Zero;
            return Vector2.Normalize(v);
        }
        public static Vector2 DirectionTo(this Vector2 Origin, Vector2 Target) => Vector2.Normalize(Target - Origin);
        public static int[] Deconstruct(this Point p) => [p.X, p.Y];
        public static float[] Deconstruct(this Vector2 v) => [v.X, v.Y];
        public static void DrawLine(this SpriteBatch spriteBatch, Vector2 point1, Vector2 point2, Color color, float thickness = 1f)
        {
            float length = Vector2.Distance(point1, point2);
            float angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
            spriteBatch.DrawLine(point1, length, angle, color, thickness);
        }
        public static void DrawLine(this SpriteBatch spriteBatch, Vector2 point, float length, float angle, Color color, float thickness = 1f)
        {
            spriteBatch.Draw(origin: new Vector2(0f, 0.5f), scale: new Vector2(length, thickness), texture: Main.WhitePixel, position: point, sourceRectangle: null, color: color, rotation: angle, effects: SpriteEffects.None, layerDepth: 0f);
        }
        public static void DrawDottedLine(this SpriteBatch spriteBatch, Vector2 point1, Vector2 point2, Color color, float gapFrequency, float gapSize, float thickness = 1f)
        {
            float totalLength = Vector2.Distance(point1, point2);
            Vector2 direction = point1.DirectionTo(point2);

            float segmentLength = gapFrequency - gapSize;
            float t = 0f;

            while (t < totalLength)
            {
                float drawLength = Math.Min(segmentLength, totalLength - t);
                Vector2 start = point1 + direction * t;
                spriteBatch.DrawLine(start, drawLength, MathF.Atan2(direction.Y, direction.X), color, thickness);
                t += gapFrequency;
            }
        }
        /// <summary>
        /// Draws a rectangle's internal outline. Funky effects if thickness is set to more than half of the smallest dimension of the rectangle.
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="rect"></param>
        /// <param name="color"></param>
        /// <param name="thickness"></param>
        public static void DrawRectangleOutline(this SpriteBatch spriteBatch, Rectangle rect, Color color, float thickness = 1f)
        {
            Texture2D pixel = Main.WhitePixel;
            // top left to top right
            spriteBatch.Draw(pixel, new Vector2(rect.X, rect.Y), null, color, 0f, Vector2.Zero, new Vector2(rect.Width, thickness), SpriteEffects.None, 0f);
            // top right to bottom right
            spriteBatch.Draw(pixel, new Vector2(rect.Right, rect.Y + thickness), null, color, 0f, Vector2.UnitX, new Vector2(thickness, rect.Height - thickness * 2), SpriteEffects.None, 0f);
            // bottom right to bottom left (bottom left to bottom right)
            spriteBatch.Draw(pixel, new Vector2(rect.X + thickness, rect.Bottom), null, color, 0f, Vector2.UnitY, new Vector2(rect.Width - thickness, thickness), SpriteEffects.None, 0f);
            // bottom left to top left (top left to bottom left)
            spriteBatch.Draw(pixel, new Vector2(rect.X, rect.Y + thickness), null, color, 0f, Vector2.Zero, new Vector2(thickness, rect.Height - thickness), SpriteEffects.None, 0f);
        }
    }
}
