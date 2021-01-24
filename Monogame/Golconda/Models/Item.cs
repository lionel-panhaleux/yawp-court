using System;

using Golconda.Models.Contracts;
using Golconda.Services.Contracts;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Golconda.Models
{
    /// <summary>
    /// Anything single entity in the game (card, Edge, but not counters).
    /// </summary>
    public abstract class Item : ILocalDraw, ILocalUpdate, ILocalTarget
    {
        /// <summary>
        /// The id of the owner. 0 = no owner, N = player N
        /// </summary>
        public int OwnerId { get; set; }

        /// <summary>
        /// The id of the controller. 0 = no controller, N = player N
        /// </summary>
        public int ControllerId { get; set; }

        public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch, IProjector projector);

        public abstract void Update(GameTime gameTime, ref bool captureEvents, IProjector projector);

        /// <summary>
        /// Indicates whether the item contains a given relative position (true) or not (false).
        /// </summary>
        /// <param name="p">The relative position.</param>
        /// <returns>True if the item contains a given relative position, false otherwise.</returns>
        public abstract bool Contains(Vector2 p, IProjector projector);

        public abstract void CreateEffect(EffectType effectType, GameTime gameTime, TimeSpan duration);
        public abstract void RemoveEffect(EffectType effectType);
    }
}
