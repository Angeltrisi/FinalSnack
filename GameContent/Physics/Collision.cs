using FinalSnack.Utilities;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;

namespace FinalSnack.GameContent.Physics
{
    public static class Collision
    {
        // AABBvPoint = Rectangle.Contains(Point)
        // AABBvAABB = Rectangle.Intersects(Rectangle)
        public static Rectangle AABBMinkowskiSum(Rectangle moving, Rectangle @static)
        {
            @static.Inflate(moving.Width / 2, moving.Height / 2);
            return @static;
        }
        public static Rectangle AABBMinkowskiDiff(Rectangle moving, Rectangle @static)
        {
            int x = moving.Left - @static.Right;
            int y = moving.Top - @static.Bottom;
            int w = moving.Width + @static.Width;
            int h = moving.Height + @static.Height;

            return new Rectangle(x, y, w, h);
        }
        public static Point AABBPenetrationVector(Rectangle minkowski)
        {
            if (!minkowski.Contains(0, 0))
                return Point.Zero;

            int centerX = minkowski.X + (minkowski.Width / 2);
            int centerY = minkowski.Y + (minkowski.Height / 2);
            int dx = (minkowski.Width / 2) - Math.Abs(centerX);
            int dy = (minkowski.Height / 2) - Math.Abs(centerY);

            if (dx < dy)
                return new(centerX < 0 ? -dx : dx, 0);
            else
                return new(0, centerY < 0 ? -dy : dy);
        }
        public static Hit RayHitAABB(Ray ray, Rectangle aabb)
        {
            float[] pos = ray.Position.Deconstruct();
            float[] magnitude = ray.Magnitude.Deconstruct();

            int[] min = aabb.Location.Deconstruct();
            int[] max = [aabb.Right, aabb.Bottom];

            float lastEntry = float.NegativeInfinity;
            float firstExit = float.PositiveInfinity;

            for (byte i = 0; i < 2; i++)
            {
                if (magnitude[i] != 0)
                {
                    float t1 = (min[i] - pos[i]) / magnitude[i];
                    float t2 = (max[i] - pos[i]) / magnitude[i];

                    lastEntry = float.Max(lastEntry, float.Min(t1, t2));
                    firstExit = float.Min(firstExit, float.Max(t1, t2));
                }
                else if (pos[i] <= min[i] || pos[i] >= max[i])
                {
                    return new();
                }
            }

            if (firstExit > lastEntry && firstExit > 0f && lastEntry < 1f)
            {
                Vector2 position = new(
                    pos[0] + magnitude[0] * lastEntry,
                    pos[1] + magnitude[1] * lastEntry);

                return new Hit(true, lastEntry, position);
            }

            return new();
        }
        public static Point ResolveAABBvAABB(Vector2 velocity, Rectangle moving, Rectangle @static)
        {
            Rectangle minkowski = AABBMinkowskiDiff(moving, @static);

            bool beginCalled = VFX.Accessors.AccessBeginCalled(Main.spriteBatch);
            if (minkowski.Contains(0, 0))
            {
                Point pen = AABBPenetrationVector(minkowski);
                if (beginCalled)
                {
                    moving.Location += pen;
                    Main.spriteBatch.DrawRectangleOutline(moving, Color.Cyan, 2f);
                }

                return pen;
            }

            if (velocity != Vector2.Zero)
            {
                Ray ray = new(moving.Center.ToVector2(), velocity);
                Hit hit = RayHitAABB(ray, AABBMinkowskiSum(moving, @static));
                
                if (hit.IsHit)
                {
                    if (VFX.Accessors.AccessBeginCalled(Main.spriteBatch))
                    {
                        Main.spriteBatch.DrawRectangleOutline(Utils.RectangleFromCenter(hit.Position, moving.Width, moving.Height), Color.Orange, 2f);
                    }
                    return hit.Position.ToPoint();
                }
            }

            return Point.Zero;
        }
    }
}
