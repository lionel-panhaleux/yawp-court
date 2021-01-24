
using System;

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

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, IProjector projector)
        {
            if (Effect is GlowEffect glowEffect)
            {
                spriteBatch.Draw(CommonTextures.WhiteRectangle, projector.ProjectToScreen(new Vector2(-10, -10)), null, Color.Yellow * 0.5f, 0f, Vector2.Zero, projector.ScaleToScreen(new Vector2(_size.X + 20, _size.Y + 20)), SpriteEffects.None, 0f);
            }
            else if (Effect is PulsingGlowEffect pulsingGlowEffect)
            {
                var elapsedTime = gameTime.TotalGameTime - pulsingGlowEffect.CreationTime;
                var intensity = (float)Math.Abs(Math.Sin(2 * Math.PI * (elapsedTime.TotalMilliseconds / 5000)));
                spriteBatch.Draw(CommonTextures.WhiteRectangle, projector.ProjectToScreen(new Vector2(-10, -10)), null, Color.Red * intensity, 0f, Vector2.Zero, projector.ScaleToScreen(new Vector2(_size.X + 20, _size.Y + 20)), SpriteEffects.None, 0f);
            }

            spriteBatch.Draw(CommonTextures.CardBorder, projector.ProjectToScreen(Vector2.Zero), null, color: Color.Black, 0f, Vector2.Zero, projector.ScaleToScreenFactor, SpriteEffects.None, 0f);
            spriteBatch.Draw(Image, projector.ProjectToScreen(_textureOffset), null, Color.White, 0f, Vector2.Zero, projector.ScaleToScreenFactor, SpriteEffects.None, 0f);
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
                Effect = new GlowEffect(gameTime);
            }
            else if (effectType == EffectType.PulsingGlow)
            {
                Effect = new PulsingGlowEffect(gameTime);
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
