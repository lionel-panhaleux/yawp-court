namespace Golconda.Services.Contracts
{
    public interface IFrameCounterService
    {
        float AverageFramesPerSecond { get; }
        float CurrentFramesPerSecond { get; }
        long TotalFrames { get; }
        float TotalSeconds { get; }

        bool Update(float deltaTime);
    }
}