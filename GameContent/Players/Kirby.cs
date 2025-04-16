using FinalSnack.Core.DataStructures;
using FinalSnack.GameContent.Physics;
using FinalSnack.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace FinalSnack.GameContent.Players
{
    public class Kirby : Entity
    {
        public Kirby(Vector2 position)
        {
            this.position = position;
        }
        public override void Init()
        {
            width = height = 64;
        }
        public override void Update(GameTime gt)
        {
            float dt = gt.Delta();
            Rectangle ground = new(0, Main.ScreenHeight - height, Main.ScreenWidth, height);

            float veloChangeControl = 950f;
            float finalXVeloChange = 0f;

            if (Input.HoldPress(new(Keys.D)))
                finalXVeloChange += veloChangeControl;
            if (Input.HoldPress(new(Keys.A)))
                finalXVeloChange -= veloChangeControl;

            float grav = 800f;
            Vector2 change = new(finalXVeloChange, grav);

            velocity += change * dt;

            float frictionPerSecond = 600f;
            float jumpPower = 400f;
            float maxSpeed = 300f;

            if (velocity.X > 0)
            {
                velocity.X = MathF.Max(velocity.X - frictionPerSecond * dt, 0);
            }
            else if (velocity.X < 0)
            {
                velocity.X = MathF.Min(velocity.X + frictionPerSecond * dt, 0);
            }
            velocity.X = MathHelper.Clamp(velocity.X, -maxSpeed, maxSpeed);

            if (Input.OneShotPress(new(Keys.Space)))
            {
                velocity.Y = -jumpPower;

                string sound;
                if (ground.Contains(Bottom.ToPoint()))
                    sound = "Sounds/Kirby/jump";
                else
                    sound = "Sounds/Kirby/float";

                SoundEngine.PlaySound(sound, register: true);
            }
            position += velocity * dt;

            Rectangle minkowski = Collision.AABBMinkowskiDiff(Hitbox, ground);
            Vector2 pen = Collision.AABBPenetrationVector(minkowski).ToVector2();

            if (pen.Y < 0)
                velocity.Y = 0;

            position += pen;
        }
        public override void Draw(SpriteBatch spriteBatch, GameTime gt)
        {
            spriteBatch.Draw(Main.WhitePixel, Hitbox, Color.Pink);
        }
    }
}
