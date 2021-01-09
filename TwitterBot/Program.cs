using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TootingMad.DataSources.LaCrosse;
using Tweetinvi;


namespace TwitterBot
{
    class TwitterCredentials
    {
        public string ConsumerKey;
        public string ConsumerSecret;
        public string AccessToken;
        public string AccessSecret;
    }

    class Program
    {
        static TwitterCredentials GetCredentials()
        {
            return new TwitterCredentials()
            {
                ConsumerKey = Environment.GetEnvironmentVariable("WEATHERBOT_CONSUMER_KEY"),
                ConsumerSecret = Environment.GetEnvironmentVariable("WEATHERBOT_CONSUMER_SECRET"),
                AccessToken = Environment.GetEnvironmentVariable("WEATHERBOT_ACCESS_TOKEN"),
                AccessSecret = Environment.GetEnvironmentVariable("WEATHERBOT_ACCESS_SECRET"),
            };
        }

        static async Task Main(string[] args)
        {
            TwitterCredentials creds = GetCredentials();


            var userClient = new TwitterClient(creds.ConsumerKey, creds.ConsumerSecret, creds.AccessToken, creds.AccessSecret);

            var station = new TootingMad.DataSources.LaCrosse.WS2300(Environment.GetEnvironmentVariable("WEATHERBOT_SERIAL_PORT"));

            var sensors = new List<Sensor>
            {
                WS2300.AvailableSensors.WindDirectionCardinal,
                WS2300.AvailableSensors.WindSpeedKnots,
                WS2300.AvailableSensors.OutdoorTemperature,
                WS2300.AvailableSensors.OutdoorHumidity,
                WS2300.AvailableSensors.RelativePressure,
                WS2300.AvailableSensors.Tendency,
            };

            while(true)
            {
                var timeUntilTopOfHour = TimeSpan.FromSeconds(3600 - DateTime.Now.TimeOfDay.TotalSeconds % 3600);
                Console.WriteLine($"Sleeping for {timeUntilTopOfHour}");
                System.Threading.Thread.Sleep(timeUntilTopOfHour);

                var res = station.GetSensors(sensors);

                var resultString = $"It's currently {((decimal)res[2].Item2).ToString("F1")}°C with {((decimal)res[3].Item2).ToString("F0")}% humidity.\nThe wind is {((decimal)res[1].Item2).ToString("F1")} kts from {res[0].Item2}\nAir pressure is {((decimal)res[4].Item2).ToString("F1")} hPa and {res[5].Item2.ToString().ToLower()}";

                var tweet = await userClient.Tweets.PublishTweetAsync(resultString);
                Console.WriteLine("published the tweet: " + tweet);
            }
        }
    }
}
