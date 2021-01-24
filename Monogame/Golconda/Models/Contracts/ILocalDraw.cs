
using Golconda.Services.Contracts;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Golconda.Models.Contracts
{
    public interface ILocalDraw
    {
        /// <summary>
        /// Draws the object using a sprite batch.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// <param name="spriteBatch">The sprite batch.</param>
        /// <param name="projector">The projector used to convert local coordinates to screen coordinates.</param>
        void Draw(GameTime gameTime, SpriteBatch spriteBatch, IProjector projector);
    }
}