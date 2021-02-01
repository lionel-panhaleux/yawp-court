using Microsoft.Xna.Framework;

namespace Golconda.Models
{
    public struct Rotation2
    {
        private static readonly Rotation2 zeroRotation = new Rotation2(Vector2.Zero, 0f);

        public Vector2 _origin;
        public float _angle;

        public static Rotation2 Zero => zeroRotation;

        public Rotation2(Vector2 origin, float angle)
        {
            _origin = origin;
            _angle = angle;
        }

    }
}