using System;

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

    public class PulsingGlowEffect : VisualEffect
    {
        public int Times { get; }
        public float MaxOpacity { get; }
        public int PixelSize { get; }

        public PulsingGlowEffect(GameTime gameTime, int pixelSize, TimeSpan duration, int times, float maxOpacity)
            : base(gameTime, duration)
        {
            Times = times;
            MaxOpacity = maxOpacity;
            PixelSize = pixelSize;
        }
    }
}