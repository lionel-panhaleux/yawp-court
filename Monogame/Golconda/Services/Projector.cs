using System;
using System.Collections.Generic;

using Golconda.Models;
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

        public float ScaleToScreenFactor { get; set; }

        public float ScaleToLocalFactor { get; set; }

        private bool ContainsRotation { get; set; }

        /// <inheritdoc />
        public Vector2 ProjectToScreen(Vector2 localCoordinates)
        {
            return ProjectToScreen(localCoordinates, 0, _projections.Count - 1);
        }

        /// <summary>
        /// Recursively projects local coordinates throughout all the coordinates systems until it reaches the screen system.
        /// </summary>
        /// <param name="coordinates">The coordinates.</param>
        /// <param name="index">The index of the projection.</param>
        /// <returns>The resulting coordinates.</returns>
        private Vector2 ProjectToScreen(Vector2 coordinates, int index, int lastIndex)
        {
            if (index > lastIndex)
            {
                return coordinates;
            }

            Projection currentProjection = _projections[index];
            var result = currentProjection._coordinates + currentProjection._scale * ProjectToScreen(coordinates, index + 1, lastIndex);

            //if (currentProjection._rotation._angle != 0)
            //{
            //    result = Rotate(currentProjection._rotation._origin, currentProjection._rotation._angle, result);
            //}
            return result;
        }

        private Vector2 Rotate(Vector2 origin, double angle, Vector2 point)
        {
            double s = Math.Sin(angle);
            double c = Math.Cos(angle);

            // translate point back to origin:
            var po = point - origin;

            // rotate point
            var r = new Vector2((float)(po.X * c - po.Y * s), (float)(po.X * s + po.Y * c));

            // translate point back:
            return r + origin;
        }

        /// <inheritdoc />
        public Vector2 ProjectToLocal(Vector2 screenCoordinates)
        {
            return ProjectToLocal(screenCoordinates, 0, _projections.Count - 1);
        }

        /// <summary>
        /// Recursively projects screen coordinates throughout all the coordinates systems until it reaches the last local system.
        /// </summary>
        /// <param name="coordinates">The coordinates.</param>
        /// <param name="index">The index of the projection.</param>
        /// <returns>The resulting coordinates.</returns>
        private Vector2 ProjectToLocal(Vector2 coordinates, int index, int lastIndex)
        {
            if (index > lastIndex)
            {
                return coordinates;
            }

            Projection currentProjection = _projections[index];
            var result = ProjectToLocal((coordinates - currentProjection._coordinates) / _projections[index]._scale, index + 1, lastIndex);
            //if (currentProjection._rotation._angle != 0)
            //{
            //    result = Rotate(currentProjection._rotation._origin, -currentProjection._rotation._angle, result);
            //}
            return result;
        }

        /// <inheritdoc />
        public Rotation2 GetScreenRotation()
        {
            return !ContainsRotation
                ? Rotation2.Zero
                : _projections[_projections.Count - 1]._rotation; // the rotation is relative to the screen position of the sprite (which is already transformed), so we don't need to transform any coordinates!
        }

        /// <inheritdoc />
        public Vector2 ScaleToScreen(Vector2 localSize)
        {
            return localSize * ScaleToScreenFactor;
        }

        /// <inheritdoc />
        public float ScaleToScreen(float localSize)
        {
            return localSize * ScaleToScreenFactor;
        }

        /// <inheritdoc />
        public Vector2 ScaleToLocal(Vector2 screenSize)
        {
            return screenSize / ScaleToScreenFactor;
        }

        /// <inheritdoc />
        public float ScaleToLocal(float screenSize)
        {
            return screenSize / ScaleToScreenFactor;
        }

        /// <inheritdoc />
        public void Push(Projection projection)
        {
            if (ContainsRotation)
            {
                throw new Exception("Cannot add projections after a rotation. Rotation must be final.");
            }
            if (projection._rotation._angle != 0)
            {
                ContainsRotation = true;
            }
            _projections.Add(projection);
            UpdateScales();
        }

        /// <inheritdoc />
        public void Pop()
        {
            ContainsRotation = false; // since a rotation (if any) is possible only on the last projection, it is necessarily removed as soon as we remove a projection
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
