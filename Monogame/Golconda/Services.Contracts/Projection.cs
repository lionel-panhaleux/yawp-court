
using Microsoft.Xna.Framework;

namespace Golconda.Services.Contracts
{
    public struct Projection
    {
        public Vector2 _coordinates;
        public float _scale;

        public Projection(Vector2 coordinates, float scale)
        {
            _coordinates = coordinates;
            _scale = scale;
        }
    }
}
