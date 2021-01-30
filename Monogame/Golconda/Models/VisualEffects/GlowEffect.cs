
using Microsoft.Xna.Framework;

namespace Golconda.Models
{
    public class GlowEffect : VisualEffect
    {
        public int PixelSize { get; }

        public GlowEffect(GameTime gameTime, int pixelSize)
            : base(gameTime)
        {
            PixelSize = pixelSize;
        }
    }
}