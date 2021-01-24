using System;

using Microsoft.Xna.Framework;

namespace Golconda.Services
{
    /// <summary>
    /// A pair containing a value and a timestamp (set ot GameTime.TotalGameTime)
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    public class TimedValue<T>
    {
        public T Value { get; private set; }
        public TimeSpan Time { get; private set; }

        public void Update(T value, GameTime time)
        {
            Value = value;
            Time = time.TotalGameTime;
        }
    }
}
