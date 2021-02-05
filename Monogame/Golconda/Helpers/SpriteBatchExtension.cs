using Golconda.Services.Contracts;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Golconda.Helpers
{
    public static class SpriteBatchExtension
    {
        /// <summary>
        /// Draws a texture using the projector to display it on the screen.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch.</param>
        /// <param name="projector">The projector used to project local coordinates into screen coordinates.</param>
        /// <param name="texture">The texture.</param>
        /// <param name="position">The local coordinates.</param>
        /// <param name="scale">The local scale.</param>
        /// <param name="color">The color mask.</param>
        public static void ProjectionDraw(this SpriteBatch spriteBatch, IProjector projector, Texture2D texture, Vector2 position, Vector2 scale, Color color)
        {
            var screenRotation = projector.GetScreenRotation();
            Vector2 screenPosition = projector.ProjectToScreen(position);

            if (screenRotation._angle != 0)
            {
                screenPosition += projector.ScaleToScreen(screenRotation._relativeOrigin - position); // compensate the change of origin induced by Draw when the angle is not 0
                spriteBatch.Draw(texture, screenPosition, null, color, screenRotation._angle, projector.ScaleToScreen(screenRotation._relativeOrigin - position), projector.ScaleToScreen(scale), SpriteEffects.None, 0f);
            }
            else
            {
                spriteBatch.Draw(texture, screenPosition, null, color, 0, Vector2.Zero, projector.ScaleToScreen(scale), SpriteEffects.None, 0f);
            }
        }

        /// <summary>
        /// Draws a texture using the projector to display it on the screen.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch.</param>
        /// <param name="projector">The projector used to project local coordinates into screen coordinates.</param>
        /// <param name="texture">The texture.</param>
        /// <param name="position">The local coordinates.</param>
        /// <param name="scale">The local scale.</param>
        /// <param name="color">The color mask.</param>
        public static void ProjectionDraw(this SpriteBatch spriteBatch, IProjector projector, Texture2D texture, Vector2 position, float scale, Color color)
        {
            var screenRotation = projector.GetScreenRotation();
            Vector2 screenPosition = projector.ProjectToScreen(position);
            
            if(screenRotation._angle != 0)
            {
                screenPosition += projector.ScaleToScreen(screenRotation._relativeOrigin - position); // compensate the change of origin induced by Draw when the angle is not 0
                spriteBatch.Draw(texture, screenPosition, null, color, screenRotation._angle, projector.ScaleToScreen(screenRotation._relativeOrigin - position), projector.ScaleToScreen(scale), SpriteEffects.None, 0f);
            }
            else
            {
                spriteBatch.Draw(texture, screenPosition, null, color, 0, Vector2.Zero, projector.ScaleToScreen(scale), SpriteEffects.None, 0f);
            }
        }

        /// <summary>
        /// Draws a texture using the projector to display it on the screen.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch.</param>
        /// <param name="projector">The projector used to project local coordinates into screen coordinates.</param>
        /// <param name="texture">The texture.</param>
        /// <param name="position">The local coordinates.</param>
        /// <param name="color">The color mask.</param>
        public static void ProjectionDraw(this SpriteBatch spriteBatch, IProjector projector, Texture2D texture, Vector2 position, Color color)
        {
            ProjectionDraw(spriteBatch, projector, texture, position, 1f, color);
        }

        /// <summary>
        /// Draws a texture using the projector to display it on the screen.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch.</param>
        /// <param name="projector">The projector used to project local coordinates into screen coordinates.</param>
        /// <param name="texture">The texture.</param>
        /// <param name="position">The local coordinates.</param>
        /// <param name="scale">The local scale.</param>
        public static void ProjectionDraw(this SpriteBatch spriteBatch, IProjector projector, Texture2D texture, Vector2 position, float scale)
        {
            ProjectionDraw(spriteBatch, projector, texture, position, scale, Color.White);
        }

        /// <summary>
        /// Draws a texture using the projector to display it on the screen.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch.</param>
        /// <param name="projector">The projector used to project local coordinates into screen coordinates.</param>
        /// <param name="texture">The texture.</param>
        /// <param name="position">The local coordinates.</param>
        /// <param name="scale">The local scale</param>
        public static void ProjectionDraw(this SpriteBatch spriteBatch, IProjector projector, Texture2D texture, Vector2 position)
        {
            ProjectionDraw(spriteBatch, projector, texture, position, 1f, Color.White);
        }
    }
}
