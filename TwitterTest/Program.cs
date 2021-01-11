using System;
using System.Threading;
using System.Threading.Tasks;
using Tweetinvi;
using System.Linq;
using TootingMad.DataSources.LaCrosse;
using System.Collections.Generic;

using ScottPlot;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Tweetinvi.Parameters;

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
            TwitterCredentials creds = GetCredentials();

            var userClient = new TwitterClient(creds.ConsumerKey, creds.ConsumerSecret, creds.AccessToken, creds.AccessSecret);

            //var station = new TootingMad.DataSources.LaCrosse.WS2300(Environment.GetEnvironmentVariable("WEATHERBOT_SERIAL_PORT"));

            var sensors = new List<Sensor>
            {
                WS2300.AvailableSensors.WindDirectionCardinal,
                WS2300.AvailableSensors.WindSpeedKnots,
                WS2300.AvailableSensors.OutdoorTemperature,
                WS2300.AvailableSensors.OutdoorHumidity,
                WS2300.AvailableSensors.RelativePressure,
                WS2300.AvailableSensors.Tendency,
            };



            //var res = station.GetSensors(sensors);

            var resultString = $"Does this work?";

            Console.WriteLine(resultString);

            var user = await userClient.Users.GetAuthenticatedUserAsync();
            Console.WriteLine(user);


            int width = 800;
            int height = 600;
            var plot = new Plot(width, height);

            DateTime[] myDates = new DateTime[60];
            var startdate = DateTime.Now;
            for (int i = 0; i<myDates.Length; i++)
            {
                myDates[i] = startdate.AddMinutes(i * 5);
            }

            double[] dataX = myDates.Select(x => x.ToOADate()).ToArray();
            double[] dataY = new double[] { 1, 4, 9, 16, 25, };
            //plot.PlotScatter(dataX, dataY);

            var scatter = plot.AddScatter(dataX, dataY);

            plot.XAxis.DateTimeFormat(true);

            //plot.SaveFig("out.png");
            var bitmap = plot.Render();
            var png = ImageToPngByte(bitmap);

            var uploadImage = await userClient.Upload.UploadTweetImageAsync(png);
            var tweetWithImage = await userClient.Tweets.PublishTweetAsync(new PublishTweetParameters(resultString)
            {
                Medias = { uploadImage }
            });

            var tweet = await userClient.Tweets.PublishTweetAsync(resultString);
            Console.WriteLine("published the tweet: " + tweetWithImage);
        }

        public static byte[] ImageToPngByte(Image img)
        {
            using (var stream = new MemoryStream())
            {
                img.Save(stream, ImageFormat.Png);
                return stream.ToArray();
            }
        }
    }
}
