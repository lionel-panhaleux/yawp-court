using System;
using System.Collections.Generic;
using System.Linq;

using Golconda.Helpers;
using Golconda.Models.Contracts;
using Golconda.Services.Contracts;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Golconda.Models
{
    public class Hand : ILocalDraw, ILocalUpdate, ILocalTarget
    {
        private IInputService InputService { get; }

        public List<Local<Card>> Cards { get; } = new List<Local<Card>>();

        public Hand(IInputService inputService)
        {
            InputService = inputService;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, IProjector projector)
        {
        }

        public void Update(GameTime gameTime, ref bool captureEvents, IProjector projector)
        {
        }

        public bool Contains(Vector2 position, IProjector projector)
        {
            return false;
        }
    }


    public class Board : ILocalDraw, ILocalUpdate, ILocalTarget
    {
        private IInputService InputService { get; }

        public List<Local<Item>> Items { get; } = new List<Local<Item>>();

        // todo replace with a list
        public Local<Item> SelectedItem { get; set; }

        public Local<Item> HoveredCard { get; set; }

        public Board(IInputService inputService)
        {
            InputService = inputService;
        }

        /// <inheritdoc />
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, IProjector projector)
        {
            spriteBatch.ProjectionDraw(projector, CommonTextures.BoardBackground, Vector2.Zero, 8f);

            foreach (var boardItem in Items)
            {
                boardItem.Draw(gameTime, spriteBatch, projector);
            }
        }

        /// <inheritdoc />
        public void Update(GameTime gameTime, ref bool captureEvents, IProjector projector)
        {
            if (!captureEvents)
            {
                return;
            }

            if (InputService.LeftMousePressed)
            {
                if (!InputService.IsDragging)
                {
                    var mousePosition = InputService.MousePosition.ToVector2();
                    var selectedItem = Items.LastOrDefault(i => i.Contains(mousePosition, projector));
                    SelectedItem = selectedItem;
                    foreach (var item in Items) item.Value.RemoveEffect(EffectType.Glow);
                    if (SelectedItem != null) SelectedItem.Value.CreateEffect(EffectType.Glow, gameTime, TimeSpan.MaxValue);
                }
                else if (SelectedItem != null)
                {
                    var dragDelta = projector.ScaleToLocal(InputService.DragDelta.ToVector2());
                    SelectedItem._translate += dragDelta;
                    captureEvents = false;
                }
            }
            else
            {
                if (InputService.DoubleClicked)
                {
                    if (SelectedItem != null) SelectedItem.Value.CreateEffect(EffectType.PulsingGlow, gameTime, TimeSpan.FromSeconds(1));
                }
                else
                {
                    var mousePosition = InputService.MousePosition.ToVector2();
                    HoveredCard = Items.Where(i => i.Value is Card).LastOrDefault(i => i.Contains(mousePosition, projector));
                }
            }
            return;
        }

        public bool Contains(Vector2 position, IProjector Iprojector)
        {
            return true;
        }
    }
}
