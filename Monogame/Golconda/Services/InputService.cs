using System;

using Golconda.Services.Contracts;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Golconda.Services
{
    public class InputService : IInputService
    {
        private const long ClickDelay = 1000;
        private const long DoubleClickDelay = 1000;

        public TimedValue<int> LastScroll { get; set; } = new TimedValue<int>();
        public TimedValue<ButtonState> LastLeftButton { get; set; } = new TimedValue<ButtonState>();
        public TimedValue<Point> LastMousePosition { get; set; } = new TimedValue<Point>();
        public TimeSpan LastLeftButtonChangeState { get; set; } = TimeSpan.Zero;
        public TimedValue<Point> LastLeftClick { get; set; } = new TimedValue<Point>();

        public bool IsZooming { get; set; }
        public int ZoomDelta { get; set; }
        public bool LeftMousePressed { get; set; }
        public bool Clicked { get; set; }
        public bool IsDragging { get; set; }
        public Point MousePosition { get; set; }
        public Point DragDelta { get; set; }

        public bool DoubleClicked { get; set; }

        public void Update(GameTime gameTime)
        {
            var mouseState = Mouse.GetState();
            var totalGameTime = gameTime.TotalGameTime;

            LeftMousePressed = mouseState.LeftButton == ButtonState.Pressed;
            MousePosition = mouseState.Position;

            IsZooming = Keyboard.GetState().IsKeyDown(Keys.LeftControl) && mouseState.ScrollWheelValue != LastScroll.Value;
            ZoomDelta = mouseState.ScrollWheelValue - LastScroll.Value;

            Clicked = LastLeftButton.Value == ButtonState.Pressed && mouseState.LeftButton == ButtonState.Released && (totalGameTime - LastLeftButtonChangeState).TotalMilliseconds < ClickDelay;
            DoubleClicked = (Clicked && LastLeftClick.Value == MousePosition && (totalGameTime - LastLeftClick.Time).TotalMilliseconds < DoubleClickDelay);

            IsDragging = IsDragging && mouseState.LeftButton == ButtonState.Pressed || mouseState.LeftButton == ButtonState.Pressed && LastLeftButtonChangeState != totalGameTime && LastMousePosition.Value != mouseState.Position;
            DragDelta = mouseState.Position - LastMousePosition.Value;

            if (LastLeftButton.Value != mouseState.LeftButton)
            {
                LastLeftButtonChangeState = totalGameTime;
            }
            if (Clicked)
            {
                LastLeftClick.Update(MousePosition, gameTime);
            }

            LastScroll.Update(mouseState.ScrollWheelValue, gameTime);
            LastLeftButton.Update(mouseState.LeftButton, gameTime);
            LastMousePosition.Update(mouseState.Position, gameTime);
        }
    }
}
