using Microsoft.Xna.Framework;

namespace Golconda.Models
{
    /// <summary>
    /// A vertical sliding effect from the bottom of the screen towards the top.
    /// </summary>
    public class SlideEffect : VisualEffect
    {
        private readonly float _verticalVelocity;
        private readonly float _friction;
        private readonly float _finalDistance;

        private Vector2 _position;

        public SlideEffect(GameTime gameTime, Vector2 finalPosition) : base(gameTime)
        {
            _verticalVelocity = 10f;
            _friction = 7.5f;
            // final position d=1/2 v0²/f
            _finalDistance = (float)(0.5 * _verticalVelocity * _verticalVelocity / _friction);
            _position = finalPosition - new Vector2(0, _finalDistance);
        }

        public Vector2 GetPosition(GameTime gameTime)
        {
            var elapsed = (gameTime.TotalGameTime - CreationTime).TotalSeconds;

            var maxDuration = _verticalVelocity / _friction;

            if (elapsed > maxDuration)
            {

                return _position - new Vector2(0, _finalDistance);
            }

            // given a velocity v(t) = v0 - ft
            // we integrate the position : p(t) = p0 + v0⋅t - 1/2⋅ft²
            return _position - new Vector2(0, (float)(_verticalVelocity * elapsed - 0.5 * _friction * elapsed * elapsed));
        }
    }
}