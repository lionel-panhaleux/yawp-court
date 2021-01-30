using Golconda.Models.Contracts;
using Golconda.Services;
using Golconda.Services.Contracts;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Golconda.Models
{
    /// <summary>
    /// Stores an item and its local coordinate system.
    /// </summary>
    /// <typeparam name="T">The type of item.</typeparam>
    public class Local<T>
        where T : ILocalDraw, ILocalTarget, ILocalUpdate
    {
        public T Value { get; }
        public Vector2 _origin = Vector2.Zero;
        public float _scale = 1f;

        public Local(T value)
        {
            Value = value;
        }

        public Local(T value, float scale)
        {
            Value = value;
            _scale = scale;
        }

        public Projection GetProjection()
        {
            return new Projection(_origin, _scale);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, IProjector projector)
        {
            using (new ProjectorScope(projector, GetProjection()))
            {
                Value.Draw(gameTime, spriteBatch, projector);
            }
        }

        public void Update(GameTime gameTime, ref bool captureEvents, IProjector projector)
        {
            using (new ProjectorScope(projector, GetProjection()))
            {
                Value.Update(gameTime, ref captureEvents, projector);
            }
        }

        public bool Contains(Vector2 position, IProjector projector)
        {
            using (new ProjectorScope(projector, GetProjection()))
            {
                return Value.Contains(position, projector);
            }
        }
    }
}
