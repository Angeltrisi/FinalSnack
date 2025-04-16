using System;
using System.Runtime.CompilerServices;

namespace FinalSnack.Utilities
{
    public struct SpriteBatchData
    {
        public SpriteSortMode SortMode;
        public BlendState BlendState;
        public SamplerState SamplerState;
        public DepthStencilState DepthStencilState;
        public RasterizerState RasterizerState;
        public Effect Effect;
        public Matrix Matrix;
        public SpriteBatchData(SpriteBatch spriteBatch)
        {
            ArgumentNullException.ThrowIfNull(spriteBatch);

            SortMode = VFX.Accessors.AccessSpriteSortMode(spriteBatch);
            BlendState = VFX.Accessors.AccessBlendState(spriteBatch);
            SamplerState = VFX.Accessors.AccessSamplerState(spriteBatch);
            DepthStencilState = VFX.Accessors.AccessStencilState(spriteBatch);
            RasterizerState = VFX.Accessors.AccessRasterizerState(spriteBatch);
            Effect = VFX.Accessors.AccessEffect(spriteBatch);
            Matrix = VFX.Accessors.AccessSpriteEffect(spriteBatch).TransformMatrix ?? Matrix.Identity;
        }
        public SpriteBatchData(SpriteSortMode sortMode, BlendState blendState, SamplerState samplerState, DepthStencilState stencilState, RasterizerState rasterizerState, Effect effect, Matrix matrix)
        {
            SortMode = sortMode;
            BlendState = blendState;
            SamplerState = samplerState;
            DepthStencilState = stencilState;
            RasterizerState = rasterizerState;
            Effect = effect;
            Matrix = matrix;
        }
        public readonly SpriteBatchData ChangedSortMode(SpriteSortMode sortMode) => new(sortMode, BlendState, SamplerState, DepthStencilState, RasterizerState, Effect, Matrix);
        public readonly SpriteBatchData ChangedBlendState(BlendState blendState) => new(SortMode, blendState, SamplerState, DepthStencilState, RasterizerState, Effect, Matrix);
        public readonly SpriteBatchData ChangedSamplerState(SamplerState samplerState) => new(SortMode, BlendState, samplerState, DepthStencilState, RasterizerState, Effect, Matrix);
        public readonly SpriteBatchData ChangedStencilState(DepthStencilState stencilState) => new(SortMode, BlendState, SamplerState, stencilState, RasterizerState, Effect, Matrix);
        public readonly SpriteBatchData ChangedRasterizerState(RasterizerState rasterizerState) => new(SortMode, BlendState, SamplerState, DepthStencilState, rasterizerState, Effect, Matrix);
        public readonly SpriteBatchData ChangedEffect(Effect effect) => new(SortMode, BlendState, SamplerState, DepthStencilState, RasterizerState, effect, Matrix);
        public readonly SpriteBatchData ChangedMatrix(Matrix matrix) => new(SortMode, BlendState, SamplerState, DepthStencilState, RasterizerState, Effect, matrix);
    }
    public static class VFX
    {
        public static void Begin(this SpriteBatch spriteBatch, SpriteBatchData spriteBatchData)
        {
            spriteBatch.Begin
            (
                spriteBatchData.SortMode, spriteBatchData.BlendState, spriteBatchData.SamplerState, spriteBatchData.DepthStencilState,
                spriteBatchData.RasterizerState, spriteBatchData.Effect, spriteBatchData.Matrix
            );
        }
        public static void End(this SpriteBatch spriteBatch, out SpriteBatchData spriteBatchInfo)
        {
            spriteBatchInfo = new SpriteBatchData(spriteBatch);
            spriteBatch.End();
        }
        public static class Accessors
        {
            [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_sortMode")]
            public static extern ref SpriteSortMode AccessSpriteSortMode(SpriteBatch instance);

            [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_blendState")]
            public static extern ref BlendState AccessBlendState(SpriteBatch instance);

            [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_samplerState")]
            public static extern ref SamplerState AccessSamplerState(SpriteBatch instance);

            [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_depthStencilState")]
            public static extern ref DepthStencilState AccessStencilState(SpriteBatch instance);

            [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_rasterizerState")]
            public static extern ref RasterizerState AccessRasterizerState(SpriteBatch instance);

            [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_effect")]
            public static extern ref Effect AccessEffect(SpriteBatch instance);

            [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_spriteEffect")]
            public static extern ref SpriteEffect AccessSpriteEffect(SpriteBatch instance);
            [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_beginCalled")]
            public static extern ref bool AccessBeginCalled(SpriteBatch instance);
        }
        public static Vector2 Size(this Texture2D tex) => new(tex.Width, tex.Height);
    }
}
