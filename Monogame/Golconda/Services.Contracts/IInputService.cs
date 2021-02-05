
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Golconda.Services.Contracts
{
    /// <summary>
    /// The service that centralizes all the input commands.
    /// </summary>
    public interface IInputService
    {
        Keys[] PressedKeys { get; }

        /// <summary>
        /// Indicates whether a zoom command (CTRL+scroll wheel) is emitted (true) or not (false).
        /// </summary>
        bool IsZooming { get; }

        /// <summary>
        /// Returns the value of the current zoom change (120 per notch).
        /// </summary>
        int ZoomDelta { get; }

        /// <summary>
        /// Indicates whether the left button of the mouse is pressed (true) or not (false).
        /// </summary>
        bool LeftMousePressed { get; }

        /// <summary>
        /// Indicates whether a single click just occurred (true) or not (false).
        /// </summary>
        bool Clicked { get; }

        /// <summary>
        /// Indicates whether a double click just occurred (true) or not (false).
        /// </summary>
        bool DoubleClicked { get; }

        /// <summary>
        /// Indicates wheter a drag command (left button pressed, then move while keeping the button pressed) is emitted (true) or not (false).
        /// </summary>
        bool IsDragging { get; }

        /// <summary>
        /// Returns the value of the current drag move.
        /// </summary>
        Point DragDelta { get; }

        /// <summary>
        /// Returns the screen position of the mouse (relative to the top left corner of the window if windowed, or of the screen if full-screen).
        /// </summary>
        Point MousePosition { get; }

        /// <summary>
        /// Updates the input service values with the current game frame value.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        void Update(GameTime gameTime);
    }
}