
using System.Collections.Generic;

using Golconda.Services.Contracts;

using Microsoft.Xna.Framework;

namespace Golconda.Services
{
    /// <summary>
    /// Contains the list of projections used to project nested local coordinates onto the screen.
    /// </summary>
    public class Projector : IProjector
    {
        private readonly List<Projection> _projections = new List<Projection>();

        /// <inheritdoc />
        public float ScaleToScreenFactor { get; private set; }

        /// <inheritdoc />
        public float ScaleToLocalFactor { get; private set; }

        /// <inheritdoc />
        public Vector2 ProjectToScreen(Vector2 localCoordinates)
        {
            return ProjectToScreen(localCoordinates, 0);
        }

        /// <summary>
        /// Recursively projects local coordinates throughout all the coordinates systems until it reaches the screen system.
        /// </summary>
        /// <param name="coordinates">The coordinates.</param>
        /// <param name="index">The index of the projection.</param>
        /// <returns>The resulting coordinates.</returns>
        private Vector2 ProjectToScreen(Vector2 coordinates, int index)
        {
            if (index >= _projections.Count)
            {
                return coordinates;
            }
            return _projections[index]._coordinates + _projections[index]._scale * ProjectToScreen(coordinates, index + 1);
        }

        /// <inheritdoc />
        public Vector2 ProjectToLocal(Vector2 screenCoordinates)
        {
            return ProjectToLocal(screenCoordinates, 0);
        }

        /// <summary>
        /// Recursively projects screen coordinates throughout all the coordinates systems until it reaches the last local system.
        /// </summary>
        /// <param name="coordinates">The coordinates.</param>
        /// <param name="index">The index of the projection.</param>
        /// <returns>The resulting coordinates.</returns>
        private Vector2 ProjectToLocal(Vector2 coordinates, int index)
        {
            if (index >= _projections.Count)
            {
                return coordinates;
            }
            return ProjectToLocal((coordinates - _projections[index]._coordinates) / _projections[index]._scale, index + 1);
        }

        /// <inheritdoc />
        public Vector2 ScaleToScreen(Vector2 localSize)
        {
            return localSize * ScaleToScreenFactor;
        }

        /// <inheritdoc />
        public Vector2 ScaleToLocal(Vector2 screenSize)
        {
            return screenSize / ScaleToScreenFactor;
        }

        /// <inheritdoc />
        public void Push(Projection projection)
        {
            _projections.Add(projection);
            UpdateScales();
        }

        /// <inheritdoc />
        public void Pop()
        {
            _projections.RemoveAt(_projections.Count - 1);
            UpdateScales();
        }

        private void UpdateScales()
        {
            ScaleToScreenFactor = 1f;
            ScaleToLocalFactor = 1f;
            foreach (var projection in _projections)
            {
                ScaleToScreenFactor *= projection._scale;
                ScaleToLocalFactor /= projection._scale;
            }
        }
    }
}
