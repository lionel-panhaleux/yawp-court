
using Golconda.Services.Contracts;

using Microsoft.Xna.Framework;

namespace Golconda.Models.Contracts
{
    public interface ILocalUpdate
    {
        /// <summary>
        /// Updates the state of the object.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// <param name="captureEvents">Indicates whether the board should capture events (true) or not (false). Must be set to false if any event was captured so that other components don't capture the events.</param>
        /// <param name="projector">The projector used to convert screen coordinates to local coordinates.</param>
        void Update(GameTime gameTime, ref bool captureEvents, IProjector projector);
    }
}