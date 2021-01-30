using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Golconda.Helpers
{
    public static class SpriteBatchExtension
    {
        /// <summary>
        /// An alias to the most-used Draw call.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch.</param>
        /// <param name="texture">A texture.</param>
        /// <param name="position">The drawing location on screen.</param>
        /// <param name="scale">A scaling of this sprite.</param>
        /// <param name="color">A color mask.</param>
        public static void Draw(this SpriteBatch spriteBatch, Texture2D texture, Vector2 position, Vector2 scale, Color color)
        {
            spriteBatch.Draw(texture, position, null, color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }

        /// <summary>
        /// An alias to the most-used Draw call.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch.</param>
        /// <param name="texture">A texture.</param>
        /// <param name="position">The drawing location on screen.</param>
        /// <param name="scale">A scaling of this sprite.</param>
        /// <param name="color">A color mask.</param>
        public static void Draw(this SpriteBatch spriteBatch, Texture2D texture, Vector2 position, float scale, Color color)
        {
            spriteBatch.Draw(texture, position, null, color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }

        /// <summary>
        /// An alias to the most-used Draw call.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch.</param>
        /// <param name="texture">A texture.</param>
        /// <param name="position">The drawing location on screen.</param>
        /// <param name="scale">A scaling of this sprite.</param>
        public static void Draw(this SpriteBatch spriteBatch, Texture2D texture, Vector2 position, float scale)
        {
            spriteBatch.Draw(texture, position, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }
    }
}
