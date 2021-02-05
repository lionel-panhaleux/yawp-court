using System;
using System.Collections.Generic;

using Golconda.Models.Contracts;
using Golconda.Services.Contracts;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Golconda.Models
{
    public class Hand : ILocalDraw, ILocalUpdate, ILocalTarget
    {
        private IInputService InputService { get; }
        public Action<Vector2> SizeChangedCallback { get; }
        public float MaxWidth { get; }
        public List<Local<Card>> Cards { get; } = new List<Local<Card>>();

        public Hand(IInputService inputService, Action<Vector2> sizeChangedCallback, float maxWidth)
        {
            InputService = inputService;
            SizeChangedCallback = sizeChangedCallback;
            MaxWidth = maxWidth;
        }

        public void AddCard(Card card)
        {
            var refCard = new Local<Card>(card);
            Cards.Add(refCard);
            ReorganizeHand();
        }

        public void RemoveCard(Card card)
        {
            var index = Cards.FindIndex(lc => lc.Value == card);
            if (index > -1)
            {
                Cards.RemoveAt(index);
                ReorganizeHand();
            }
        }

        private void ReorganizeHand()
        {
            if (Cards.Count == 0) return;

            float height = 0;

            // we assume all the cards have the same size
            var cardSize = Cards[0].Value._size;

            // we calculate the overlap factor so that all the cards fit within the maximum width
            var overlapFactor = Cards.Count == 1 ? 1 : -((MaxWidth / cardSize.X) - Cards.Count) / (Cards.Count - 1);
            // when there are only a few cards in hand, we want them to overlap by at least the half of their width
            if (overlapFactor < 0.5f) overlapFactor = 0.5f;

            // compute relative position of each card to each other
            var overlapWidth = cardSize.X * (1 - overlapFactor);
            float x = 0;
            foreach (var card in Cards)
            {
                card._translate = new Vector2(x, 0);
                x += overlapWidth;
                height = Math.Max(height, card.Value._size.Y);
            }

            float width = Cards.Count * cardSize.X - (Cards.Count - 1) * cardSize.X * overlapFactor; // should be equal to max width most of the time

            height *= 2; // we want the rotation center to be below the cards so they spread nicely
            var rotationCenter = new Vector2(width / 2, height);

            // recenter the hand so that 0,0 is the rotation point
            foreach (var card in Cards)
            {
                card._translate -= rotationCenter;
            }
            rotationCenter = Vector2.Zero;

            // we want the cards to spread on the eighth of a circle, but with a maximum spread when there are only a few carsds
            double stepAngle = Math.Min(Math.PI / 40, MathHelper.PiOver4 / Cards.Count);

            // The XNA SpriteBatch works in Client Space. Where "up" is Y-, not Y+ (as in Cartesian space, projection space, and what most people usually 
            // select for their world space). This makes the rotation appear as clockwise (not counter-clockwise as it would in Cartesian space).
            double angle = -stepAngle * (Cards.Count - 1) / 2;
            foreach (var card in Cards)
            {
                card._rotation = new Rotation2(rotationCenter - card._translate, (float)angle);
                angle += stepAngle;
            }

            SizeChangedCallback?.Invoke(new Vector2(width, height));
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, IProjector projector)
        {
            foreach (var card in Cards)
            {
                card.Draw(gameTime, spriteBatch, projector);
            }
        }

        public void Update(GameTime gameTime, ref bool captureEvents, IProjector projector)
        {
        }

        public bool Contains(Vector2 position, IProjector projector)
        {
            return false;
        }
    }
}
