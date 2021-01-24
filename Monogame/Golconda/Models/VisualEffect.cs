using System;

using Microsoft.Xna.Framework;

namespace Golconda.Models
{
    public abstract class VisualEffect
    {
        public TimeSpan CreationTime { get; set; }

        protected VisualEffect(GameTime gameTime)
        {
            CreationTime = gameTime.TotalGameTime;
        }
    }
}