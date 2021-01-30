using System;

using Microsoft.Xna.Framework;

namespace Golconda.Models
{
    public class PulsingGlowEffect : VisualEffect
    {
        public int Times { get; }
        public float MaxOpacity { get; }
        public int PixelSize { get; }

        public PulsingGlowEffect(GameTime gameTime, TimeSpan duration, int pixelSize, int times, float maxOpacity)
            : base(gameTime, duration)
        {
            Times = times;
            MaxOpacity = maxOpacity;
            PixelSize = pixelSize;
        }
    }
}