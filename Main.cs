global using FinalSnack.Core;
global using FinalSnack.Utilities;
global using Microsoft.Xna.Framework;
global using Microsoft.Xna.Framework.Content;
global using Microsoft.Xna.Framework.Graphics;
global using Microsoft.Xna.Framework.Input;
global using FmodForFoxes;
using FinalSnack.Core.DataStructures;
using FinalSnack.GameContent.Players;
using System.Reflection;
using System;
using FinalSnack.Development;

namespace FinalSnack
{
    public class Main : Game
    {
        private readonly INativeFmodLibrary _nativeLibrary;
        private readonly GraphicsDeviceManager _graphics;
        public static SpriteBatch spriteBatch;
        public static Main Instance { get; private set; }
        public static GraphicsDevice Graphics => Instance.GraphicsDevice;
        public static int ScreenWidth => Instance.Window.ClientBounds.Width;
        public static int ScreenHeight => Instance.Window.ClientBounds.Height;
        public static ContentManager GameContent => Instance.Content;
        public static Assembly Code { get; private set; }
        public static Texture2D BlankPixel { get; private set; }
        public static Texture2D WhitePixel { get; private set; }
        public static Effect DefaultShader { get; private set; }
        public static Asset<SpriteFont> Peaberry { get; private set; }
#pragma warning disable
        private static bool _needsToExit = false;
        private static bool _isActive = true;
#pragma warning restore
        public static double TotalRunSeconds { get; private set; }

        private static Sound dedede;
        private static Channel dededeC;
        private static bool hasPlayed = false;
        public static bool FirstUpdate => TotalRunSeconds == 0.0d;
        public static Entity[] EntitiesNeedToUpdate { get; private set; } = new Entity[512];
        public static Kirby MyKirby { get; private set; }
        public static bool GameFocused => Instance.IsActive;
        public Main(INativeFmodLibrary nativeLibrary)
        {
            Instance = this;
            Code = Assembly.GetExecutingAssembly();
            _nativeLibrary = nativeLibrary;
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Assets";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            FmodManager.Init(_nativeLibrary, FmodInitMode.Core, "Assets");

            GameMeta.Initialize();

            Window.AllowUserResizing = true;
            // do not remove this call
            base.Initialize();

            // actually initialize game!

            MyKirby = new(Vector2.One * 32f);
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            DefaultShader = VFX.Accessors.AccessSpriteEffect(spriteBatch);

            BlankPixel = new Texture2D(GraphicsDevice, 1, 1);

            WhitePixel = new Texture2D(GraphicsDevice, 1, 1);
            WhitePixel.SetData([Color.White]);

            Peaberry = AssetManager.Request<SpriteFont>("Fonts/Peaberry");

            GameCache.LoadAssets();

            dedede = SoundEngine.LoadSound("Music/dedede", true);
        }
        protected override void UnloadContent()
        {
            GameCache.UnloadAssets();

            ShaderManager.Unload();

            SoundEngine.ClearCache();
            FmodManager.Unload();
        }
        protected override void OnExiting(object sender, ExitingEventArgs args)
        {
            base.OnExiting(sender, args);

            if (!_needsToExit)
            {
                _needsToExit = true;
                args.Cancel = true;
            }
        }
        protected override void Update(GameTime gameTime)
        {
            if (FirstUpdate)
            {
                dededeC = dedede.Play();
                dededeC.Looping = true;
                hasPlayed = true;
            }
            if (false && !hasPlayed && dedede != null)
            {
                try
                {
                    Channel c = dedede.Play();
                    c.Looping = true;
                    hasPlayed = true;
                }
                catch(NullReferenceException)
                {

                }
            }

            // we do not call base.update here, it just increases update time for no reason
            TotalRunSeconds += gameTime.ElapsedGameTime.TotalSeconds;

            FmodManager.Update();

            // single update start

            // input should be updated right before playable elements are.
            Input.Update(gameTime);

            Entity[] entities = EntitiesNeedToUpdate;

            for (int i = 0; i < entities.Length; i++)
            {
                Entity e = entities[i];
                if (e is null)
                    continue;
                e.Update(gameTime);
            }
            // single update end

            if (_needsToExit)
            {
                // add saving logic
                _isActive = false;
                Exit();
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new(21, 26, 24));

            float scale = 3f;
            //spriteBatch.Begin(transformMatrix: Matrix.CreateScale(scale) * Matrix.CreateTranslation(ScreenWidth * 0.3f, ScreenHeight * 0.13f, 0f));
            spriteBatch.Begin();

            Entity[] entities = EntitiesNeedToUpdate;

            for (int i = 0; i < entities.Length; i++)
            {
                Entity e = entities[i];
                if (e is null)
                    continue;
                e.Draw(spriteBatch, gameTime);
            }

            Debugging.Audio.DrawAudioPositionData(spriteBatch, dedede, dededeC);

            spriteBatch.End();
            // similarly, we do not call base.draw here
        }
    }
}
