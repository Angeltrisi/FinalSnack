using FinalSnack.GameContent.Physics;

namespace FinalSnack.Development
{
    public static class Debugging
    {
        public static class Physics
        {
            private static Rectangle[] collisionTest;
            public static void DrawCollisionTest(SpriteBatch spriteBatch)
            {
                if (!VFX.Accessors.AccessBeginCalled(spriteBatch))
                    return;

                collisionTest ??= new Rectangle[2];

                for (int i = 0; i < collisionTest.Length; i++)
                {
                    ref Rectangle r = ref collisionTest[i];
                    if (r.IsEmpty)
                    {
                        int size = 120;
                        if (i == 0)
                            size -= 40;
                        r = new(i == 0 ? Main.ScreenWidth / 2 - size / 2 : 0, i == 0 ? Main.ScreenHeight / 2 - size / 2 : 0, size, size);
                    }
                    if (i == 0)
                    {
                        spriteBatch.DrawRectangleOutline(Collision.AABBMinkowskiSum(collisionTest[1], r), Color.Green, 2f);
                    }
                    if (i == 1)
                    {
                        r.Location = Point.Zero;
                        Point mouse = Mouse.GetState().Position;
                        Vector2 velocity = mouse.ToVector2() - r.Center.ToVector2();
                        spriteBatch.DrawLine(r.Center.ToVector2(), r.Center.ToVector2() + velocity, Color.White);
                        Collision.ResolveAABBvAABB(velocity, r, collisionTest[0]);
                        Rectangle mouseRect = new(mouse.X - r.Width / 2, mouse.Y - r.Height / 2, r.Width, r.Height);
                        Collision.ResolveAABBvAABB(Vector2.Zero, mouseRect, collisionTest[0]);
                        spriteBatch.DrawRectangleOutline(mouseRect, Color.Blue, 2f);
                    }

                    spriteBatch.DrawRectangleOutline(r, Color.White, 2f);
                }
            }
        }
        public static class Audio
        {
            public static void DrawAudioPositionData(SpriteBatch spriteBatch, in Sound sound, in Channel channel)
            {
                if (!VFX.Accessors.AccessBeginCalled(spriteBatch))
                    return;

                sound.Native.getLength(out uint length, FMOD.TIMEUNIT.PCM);
                sound.Native.getLoopPoints(out uint start, FMOD.TIMEUNIT.PCM, out uint end, FMOD.TIMEUNIT.PCM);
                channel.Native.getPosition(out uint position, FMOD.TIMEUNIT.PCM);

                spriteBatch.DrawString(Main.Peaberry.Value,
                    $"""
                    LEN {length}
                    STR {start}
                    END {end}
                    POS {position}
                    """, new(16f), Color.White);
            }
        }
    }
}
