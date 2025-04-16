using System;
using System.Collections.Generic;
using FinalSnack.Common;

namespace FinalSnack.Core
{
    public static class ShaderManager
    {
        private static readonly Dictionary<byte, Asset<Effect>> _effectRegistry = [];

        public static Dictionary<byte, Asset<Effect>> EffectRegistry => _effectRegistry;

        /// <summary>
        /// Loads or retrieves a shader into the registry.
        /// </summary>
        /// <param name="id">The ID of the shader that will be loaded. This is mandatory.</param>
        /// <param name="shaderPath">Only leave as null if you know for a fact that this shader is already loaded.</param>
        /// <returns></returns>
        public static Asset<Effect> LoadShader(byte id, string shaderPath = null)
        {
            if (_effectRegistry.TryGetValue(id, out Asset<Effect> value))
                return value;

            Asset<Effect> shader = AssetManager.Request<Effect>(shaderPath);
            _effectRegistry[id] = shader;

            return shader;
        }
        #region Parameter Extensions
        public static void SetParameter(this Asset<Effect> effect, string name, bool boolean) => effect.Value.Parameters[name].SetValue(boolean);
        public static void SetParameter(this Asset<Effect> effect, string name, int integer) => effect.Value.Parameters[name].SetValue(integer);
        public static void SetParameter(this Asset<Effect> effect, string name, int[] intArray) => effect.Value.Parameters[name].SetValue(intArray);
        public static void SetParameter(this Asset<Effect> effect, string name, float single) => effect.Value.Parameters[name].SetValue(single);
        public static void SetParameter(this Asset<Effect> effect, string name, float[] floatArray) => effect.Value.Parameters[name].SetValue(floatArray);
        public static void SetParameter(this Asset<Effect> effect, string name, Matrix matrix) => effect.Value.Parameters[name].SetValue(matrix);
        public static void SetParameter(this Asset<Effect> effect, string name, Matrix[] matrices) => effect.Value.Parameters[name].SetValue(matrices);
        public static void SetParameter(this Asset<Effect> effect, string name, Quaternion rotation) => effect.Value.Parameters[name].SetValue(rotation);
        public static void SetParameter(this Asset<Effect> effect, string name, Texture2D texture) => effect.Value.Parameters[name].SetValue(texture);
        public static void SetParameter(this Asset<Effect> effect, string name, Vector2 vector2) => effect.Value.Parameters[name].SetValue(vector2);
        public static void SetParameter(this Asset<Effect> effect, string name, Vector2[] vector2) => effect.Value.Parameters[name].SetValue(vector2);
        public static void SetParameter(this Asset<Effect> effect, string name, Vector3 vector3) => effect.Value.Parameters[name].SetValue(vector3);
        public static void SetParameter(this Asset<Effect> effect, string name, Vector3[] vector3) => effect.Value.Parameters[name].SetValue(vector3);
        public static void SetParameter(this Asset<Effect> effect, string name, Vector4 vector4) => effect.Value.Parameters[name].SetValue(vector4);
        public static void SetParameter(this Asset<Effect> effect, string name, Vector4[] vector4) => effect.Value.Parameters[name].SetValue(vector4);
        #endregion
        public static void ApplyShader(byte shaderID, Color? uColor = null, Vector2? uSize = null)
        {
            if (VFX.Accessors.AccessBeginCalled(Main.spriteBatch))
            {
                Main.spriteBatch.End(out SpriteBatchData spriteBatchInfo);
                Main.spriteBatch.Begin(spriteBatchInfo.ChangedSortMode(SpriteSortMode.Immediate));
            }
            if (_effectRegistry.TryGetValue(shaderID, out Asset<Effect> value))
            {
                if (uColor.HasValue)
                    value.SetParameter("uColor", uColor.Value.ToVector4());
                if (uSize.HasValue)
                    value.SetParameter("uSize", uSize.Value);
                value.Value.CurrentTechnique.Passes[0].Apply();
            }
            else
            {
                throw new Exception($"Shader {ShaderID.Search(shaderID) ?? "Invalid Shader"} not found in the registry.");
            }
        }
        public static void ResetShader(SpriteSortMode resetSortMode = SpriteSortMode.Deferred)
        {
            if (VFX.Accessors.AccessBeginCalled(Main.spriteBatch))
            {
                Main.spriteBatch.End(out SpriteBatchData spriteBatchInfo);
                Main.spriteBatch.Begin(spriteBatchInfo.ChangedSortMode(resetSortMode));
            }
        }
        public static void Unload()
        {
            _effectRegistry.Clear();
        }
    }
}
