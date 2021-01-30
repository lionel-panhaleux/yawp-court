
using Golconda.Services.Contracts;

using Microsoft.Xna.Framework;

namespace Golconda.Models.Contracts
{
    public interface ILocalTarget
    {
        /// <summary>
        /// Indicates whether the item contains a given relative position (true) or not (false).
        /// </summary>
        /// <param name="p">The relative position.</param>
        /// <returns>True if the item contains a given relative position, false otherwise.</returns>
        bool Contains(Vector2 position, IProjector projector);
    }
}