
using Golconda.Services.Contracts;

using Microsoft.Xna.Framework;

namespace Golconda.Models.Contracts
{
    public interface ILocalTarget
    {
        bool Contains(Vector2 position, IProjector projector);
    }
}