using System;

namespace TwitterBot
{
    public class TwitterCredentials
    {
        public string ConsumerKey;
        public string ConsumerSecret;
        public string AccessToken;
        public string AccessSecret;

        public static TwitterCredentials GetFromEnvironment()
        {
            return new TwitterCredentials()
            {
                ConsumerKey = Environment.GetEnvironmentVariable("WEATHERBOT_CONSUMER_KEY"),
                ConsumerSecret = Environment.GetEnvironmentVariable("WEATHERBOT_CONSUMER_SECRET"),
                AccessToken = Environment.GetEnvironmentVariable("WEATHERBOT_ACCESS_TOKEN"),
                AccessSecret = Environment.GetEnvironmentVariable("WEATHERBOT_ACCESS_SECRET"),
            };
        }
    }
}
