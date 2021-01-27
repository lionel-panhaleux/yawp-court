using System;

using Microsoft.Xna.Framework;

namespace Golconda.Models
{
    public abstract class VisualEffect
    {
        public TimeSpan CreationTime { get; }
        public TimeSpan Duration { get; }

        protected VisualEffect(GameTime gameTime)
        {
            CreationTime = gameTime.TotalGameTime;
            Duration = TimeSpan.MaxValue;
        }

        protected VisualEffect(GameTime gameTime, TimeSpan duration)
        {
            CreationTime = gameTime.TotalGameTime;
            Duration = duration;
        }
    }
}