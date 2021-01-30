using Microsoft.Xna.Framework;

namespace Golconda.Models
{
    public class SlidingCard
    {
        public Card Card { get; set; }
        public SlideEffect SlideEffect { get; set; }

        public SlidingCard(GameTime gameTime, Card card, Vector2 initialPosition)
        {
            Card = card;
            SlideEffect = new SlideEffect(gameTime, initialPosition);
        }
    }
}