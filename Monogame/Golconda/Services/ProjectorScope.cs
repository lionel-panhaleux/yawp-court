
using System;

using Golconda.Services.Contracts;

namespace Golconda.Services
{
    public class ProjectorScope : IDisposable
    {
        public IProjector Projector { get; }

        public ProjectorScope(IProjector projector, Projection projection)
        {
            Projector = projector;
            projector.Push(projection);
        }

        public void Dispose()
        {
            Projector.Pop();
        }
    }
}
