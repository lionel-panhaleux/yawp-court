
using Golconda.Models;

using Microsoft.Xna.Framework;

namespace Golconda.Services.Contracts
{
    /// <summary>
    /// Composes a sequence of projection. There cannot be more than one rotation, 
    /// and that rotation (if any) must be part of the last projection of the sequence.
    /// </summary>
    public interface IProjector
    {
        /// <summary>
        /// Projects screen coordinates to the local system.
        /// </summary>
        /// <param name="screenCoordinates">The screen coordinates.</param>
        /// <returns>The coordinates in the local system.</returns>
        Vector2 ProjectToLocal(Vector2 screenCoordinates);

        /// <summary>
        /// Projects local coordinates to the screen.
        /// </summary>
        /// <param name="localCoordinates">The local coordinates.</param>
        /// <returns>The coordinates on the screen.</returns>
        Vector2 ProjectToScreen(Vector2 localCoordinates);

        /// <summary>
        /// Scales a screen size to a local size.
        /// </summary>
        /// <param name="screeSize">The screen size to scale.</param>
        /// <returns>The local size.</returns>
        Vector2 ScaleToLocal(Vector2 screenSize);

        /// <summary>
        /// Scales a screen size to a local size.
        /// </summary>
        /// <param name="screeSize">The screen size to scale.</param>
        /// <returns>The local size.</returns>
        float ScaleToLocal(float screenSize);

        /// <summary>
        /// Scales a local size to a screen size.
        /// </summary>
        /// <param name="localSize">The local size to scale.</param>
        /// <returns>The screen size.</returns>
        Vector2 ScaleToScreen(Vector2 localSize);

        /// <summary>
        /// Scales a local size to a screen size.
        /// </summary>
        /// <param name="localSize">The local size to scale.</param>
        /// <returns>The screen size.</returns>
        float ScaleToScreen(float localSize);

        /// <summary>
        /// Returns the screen rotation.
        /// </summary>
        /// <returns></returns>
        Rotation2 GetScreenRotation();

        /// <summary>
        /// Adds a new projection inside the current projection system.
        /// </summary>
        /// <param name="projection">The projection.</param>
        void Push(Projection projection);

        /// <summary>
        /// Removes the last projection added inside the current projection system.
        /// </summary>
        void Pop();

    }
}