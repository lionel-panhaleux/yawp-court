using Golconda.Models.Contracts;
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

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, IProjector projector)
        {
            projector.Push(new Projection(_origin, _scale));
            Value.Draw(gameTime, spriteBatch, projector);
            projector.Pop();
        }

        public void Update(GameTime gameTime, ref bool captureEvents, IProjector projector)
        {
            projector.Push(new Projection(_origin, _scale));
            Value.Update(gameTime, ref captureEvents, projector);
            projector.Pop();
        }

        public bool Contains(Vector2 position, IProjector projector)
        {
            projector.Push(new Projection(_origin, _scale));
            var result = Value.Contains(position, projector);
            projector.Pop();
            return result;
        }
    }
}
