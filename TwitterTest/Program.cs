using System;
using System.Threading;
using System.Threading.Tasks;
using Tweetinvi;
using System.Linq;
using TootingMad.DataSources.LaCrosse;
using System.Collections.Generic;

namespace TwitterTest
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
        static TwitterCredentials GetCredentials() {

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
            TwitterCredentials creds;
            try
            {
                creds = GetCredentials();
            }
            catch (ArgumentNullException ex)
            {
                Console.WriteLine("You need to set the WEATHERBOT_* environment variables with your Twitter API keys and secrets");
                return;
            }

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



            var res = station.GetSensors(sensors);

            var resultString = $"{((decimal)res[2].Item2).ToString("F1")}°C {((decimal)res[3].Item2).ToString("F0")}%RH {((decimal)res[1].Item2).ToString("F1")} kts {res[0].Item2} {((decimal)res[4].Item2).ToString("F1")} hPa(AMSL) {res[5].Item2}";

            Console.WriteLine(resultString);

            var user = await userClient.Users.GetAuthenticatedUserAsync();
            Console.WriteLine(user);

            var tweet = await userClient.Tweets.PublishTweetAsync(resultString);
            Console.WriteLine("published the tweet: " + tweet);
        }
    }
}
