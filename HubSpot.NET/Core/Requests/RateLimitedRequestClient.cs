using System;
using System.Net;
using RestSharp;

namespace HubSpot.NET.Core.Requests
{
    public class RateLimitedRequestClient : RestClient
    {
        public RateLimitedRequestClient(
            string baseUrl, 
            int attempts = 3, 
            int retryDelay = 10000,
            bool jitter = true) : base(baseUrl)
        {
            Attempts = attempts;
            RetryDelay = retryDelay;
            TimeUnit = Utilities.Utilities.UnitOfTime.Milliseconds;
            Jitter = jitter;
        }

        private int Attempts { get; set; }
        private int RetryDelay { get; set; }
        private Utilities.Utilities.UnitOfTime TimeUnit { get; set; }
        private bool Jitter { get; set; }

        public RestResponse Execute(RestRequest request)
        {
            var jitterValue = 0;
            if (Jitter)
            {
                var random = new Random();
                jitterValue = random.Next(1, 10000);
            }
            
            var response = ExecuteAsync(request);
            var attemptCount = 1;
            
            while (attemptCount <= Attempts && response.Result.StatusCode != (HttpStatusCode)429)
            {
                // TODO - Remove debugging
                var messageRetryDelay = (RetryDelay ^ attemptCount) + jitterValue;
                Console.WriteLine($"We've been throttled! Sleeping for {messageRetryDelay.ToString()}ms.");
                response = ExecuteAsync(request);
                attemptCount++;
                Utilities.Utilities.Sleep(RetryDelay ^ attemptCount, TimeUnit);
                Utilities.Utilities.Sleep(jitterValue, TimeUnit);
            }
            return response.Result;
        }
    }
}