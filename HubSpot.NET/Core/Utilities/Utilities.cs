namespace HubSpot.NET.Core.Utilities
{
    public class Utilities
    {
        /// <summary>
        /// Sleep for n seconds
        /// </summary>
        /// <param name="duration">
        /// The number of seconds to sleep
        /// </param>
        public static void Sleep(int duration = 20)
        {
            System.Threading.Thread.Sleep(duration * 1000);
        }
    }
}