
using System;

using Golconda.Helpers;
using Golconda.Models.Contracts;
using Golconda.Services.Contracts;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Golconda.Models
{
    /// <summary>
    /// A card.
    /// </summary>
    public class Card : Item
    {
        /// <summary>
        /// The front texture of the card.
        /// </summary>
        public Texture2D Image { get; }

        public VisualEffect Effect { get; set; }

        /// <summary>
        /// The unscaled size of the card.
        /// </summary>
        public readonly Vector2 _size = new Vector2(334, 476);

        /// <summary>
        /// The size of the card texture (without the borders).
        /// </summary>
        public readonly Vector2 _sizeWithoutBorders = new Vector2(308, 438);

        public readonly Vector2 _textureOffset = new Vector2((334 - 308) / 2, (476 - 438) / 2);

        //public const float BorderRatio = 1.086269325823323f; // the ratio between the size of the card without borders and the size of the card with borders

        public Card(Texture2D image)
        {
            Image = image;
        }

        /// <inheritdoc />
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, IProjector projector)
        {
            if (Effect != null && Effect.Duration < gameTime.TotalGameTime - Effect.CreationTime)
            {
                Effect = null;
            }

            if (Effect is GlowEffect glowEffect)
            {
                var effectLocalSize = projector.ScaleToLocal(new Vector2(glowEffect.PixelSize, glowEffect.PixelSize));
                spriteBatch.ProjectionDraw(projector, CommonTextures.CardBorder, effectLocalSize * -1, (_size + 2 * effectLocalSize) / _size, Color.Yellow * 0.5f);
            }
            else if (Effect is PulsingGlowEffect pulsingGlowEffect)
            {
                var elapsedTime = gameTime.TotalGameTime - pulsingGlowEffect.CreationTime;

                var singleDuration = Effect.Duration.Ticks / pulsingGlowEffect.Times;
                var ratio = (elapsedTime.Ticks % singleDuration) / (float)singleDuration;

                var opacity = (1 - ratio) * pulsingGlowEffect.MaxOpacity;
                var effectSize = ratio * pulsingGlowEffect.PixelSize;
                var effectLocalSize = projector.ScaleToLocal(new Vector2(effectSize, effectSize));

                spriteBatch.ProjectionDraw(projector, CommonTextures.CardBorder, effectLocalSize * -1, (_size + 2 * effectLocalSize) / _size, Color.Red * opacity);
            }

            spriteBatch.ProjectionDraw(projector, CommonTextures.CardBorder, Vector2.Zero, 1f, Color.Black);
            spriteBatch.ProjectionDraw(projector, Image, _textureOffset, 1f);
        }

        /// <summary>
        /// Draws the card without any effects.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// <param name="spriteBatch">The sprite batch.</param>
        /// <param name="projector">The projector used to convert local coordinates to screen coordinates.</param>
        public void DrawNaked(GameTime gameTime, SpriteBatch spriteBatch, IProjector projector)
        {
            spriteBatch.ProjectionDraw(projector, CommonTextures.CardBorder, Vector2.Zero, 1f, Color.Black);
            spriteBatch.ProjectionDraw(projector, Image, _textureOffset, 1f);
        }

        public override void Update(GameTime gameTime, ref bool captureEvents, IProjector projector)
        {
        }

        /// <inheritdoc />
        public override bool Contains(Vector2 position, IProjector projector)
        {
            var local = projector.ProjectToLocal(position);
            return local.X >= 0 && local.X <= _size.X
                && local.Y >= 0 && local.Y <= _size.Y;
        }

        public override void CreateEffect(EffectType effectType, GameTime gameTime, TimeSpan duration)
        {
            if (effectType == EffectType.Glow)
            {
                Effect = new GlowEffect(gameTime, 5);
            }
            else if (effectType == EffectType.PulsingGlow)
            {
                Effect = new PulsingGlowEffect(gameTime, duration, 10, 3, 0.5f);
            }
        }

        public override void RemoveEffect(EffectType effectType)
        {
            switch (effectType)
            {
                case EffectType.Glow: if (Effect is GlowEffect) Effect = null; break;
            }
        }
    }
}
