using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace FinalSnack.Core
{
    public enum AssetRequestMode
    {
        Async,
        Immediate,
        Dont
    }
    public static class AssetManager
    {
        public static string GetMonoStereoPath(string relativePath) => Path.Combine(Accessors.GetRootDirectoryFullPath(Main.GameContent), relativePath);
        public static Asset<T> Request<T>(string path, AssetRequestMode requestMode = AssetRequestMode.Immediate) where T : class
        {
            Asset<T> asset = new(path);
            asset.Load(requestMode);
            return asset;
        }
        public static Asset<T> CreateFromRaw<T>(T raw) where T : class => new(raw);
        public static class Accessors
        {
            [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "get_RootDirectoryFullPath")]
            public static extern string GetRootDirectoryFullPath(ContentManager instance);
        }
    }
    public class Asset<T>(string path) where T : class
    {
        public bool IsLoaded { get; private set; } = false;
        public string Path { get; } = path;
        public bool IsFromRaw => string.IsNullOrEmpty(Path);
        public T Value
        {
            get
            {
                return IsLoaded ? _value : _defaultValue;
            }
            private set
            {
                _defaultValue = value;
            }
        }
        private T _value;
        internal T _defaultValue = GetDefault<T>();

        public Asset(T raw) : this(string.Empty)
        {
            _value = raw;
        }
        private static A GetDefault<A>() where A : class
        {
            if (typeof(A) == typeof(Texture2D))
                return Main.BlankPixel as A;
            if (typeof(A) == typeof(Effect))
                return Main.DefaultShader as A;
            return default;
        }
        public Asset<T> Wait()
        {
            if (IsLoaded) return this;
            while (true)
            {
                if (IsLoaded)
                    break;
            }
            return this;
        }
        public void Load(AssetRequestMode mode)
        {
            if (IsLoaded) return;

            if (mode == AssetRequestMode.Dont)
            {
                _value = null;
                return;
            }
            else if (mode == AssetRequestMode.Immediate)
            {
                _value = Main.GameContent.Load<T>(Path);
                IsLoaded = true;
            }
            else if (mode == AssetRequestMode.Async)
            {
                _ = AsyncLoad();
            }
        }
        private async Task AsyncLoad()
        {
            if (IsLoaded) return;
            try
            {
                _value = await Task.Run(() => Main.GameContent.Load<T>(Path));
                IsLoaded = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Couldn't load asset of type {typeof(T)} at {Path}, sorry!: {ex}");
                _value = _defaultValue;
                IsLoaded = false;
            }
        }
    }
}
