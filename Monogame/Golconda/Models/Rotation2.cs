using System.Diagnostics;

using Microsoft.Xna.Framework;

namespace Golconda.Models
{
    [DebuggerDisplay("{_relativeOrigin} {_angle}")]
    public struct Rotation2
    {
        private static readonly Rotation2 zeroRotation = new Rotation2(Vector2.Zero, 0f);

        public Vector2 _relativeOrigin;
        public float _angle;

        public static Rotation2 Zero => zeroRotation;

        public Rotation2(Vector2 relativeOrigin, float angle)
        {
            _relativeOrigin = relativeOrigin;
            _angle = angle;
        }

    }
}