
using Golconda.Models;

using Microsoft.Xna.Framework;

namespace Golconda.Services.Contracts
{
    public struct Projection
    {
        public Vector2 _coordinates;
        public float _scale;
        public Rotation2 _rotation;

        public Projection(Vector2 coordinates, float scale, Rotation2 rotation)
        {
            _coordinates = coordinates;
            _scale = scale;
            _rotation = rotation;
        }
    }
}
