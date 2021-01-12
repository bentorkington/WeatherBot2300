using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ScottPlot;
using TootingMad.DataSources.LaCrosse;
using Tweetinvi;
using Tweetinvi.Parameters;

namespace TwitterBot
{

    class Program
    {
        static async Task Main(string[] args)
        {
            var customCulture = new System.Globalization.CultureInfo("en-NZ");
            customCulture.DateTimeFormat.ShortDatePattern = "yyyy-MM-dd";
            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;

            TwitterCredentials creds = TwitterCredentials.GetFromEnvironment();

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
                WS2300.AvailableSensors.RainTotal,
            };

            List<DateTime> timeHistory = new List<DateTime>();
            List<double> temperatures = new List<double>();
            List<double> barometers = new List<double>();
            DateTime startTime = DateTime.Now;

            while(true)
            {
                var timeUntilTopOfMinute = TimeSpan.FromSeconds(60 - DateTime.Now.TimeOfDay.TotalSeconds % 60);
                Console.WriteLine($"Sleeping for {timeUntilTopOfMinute}");
                System.Threading.Thread.Sleep(timeUntilTopOfMinute);


                DateTime now = DateTime.Now;

                Decimal lastRain = -1m;

                try
                {
                    var res = station.GetSensors(sensors);

                    timeHistory.Add(now);
                    temperatures.Add(Decimal.ToDouble((decimal)res[2].Item2));
                    barometers.Add(Decimal.ToDouble((decimal)res[4].Item2));

                    

                    if (now.Hour != startTime.Hour)
                    {
                        decimal elapsedRain = 0m;
                        if (lastRain >= 0m)
                        {
                            elapsedRain = (decimal)res[6].Item2 - lastRain;
                        }

                        var resultString = $"It's currently {((decimal)res[2].Item2).ToString("F1")}°C with {((decimal)res[3].Item2).ToString("F0")}% humidity.\nThe wind is {((decimal)res[1].Item2).ToString("F1")} kts from {res[0].Item2}\nAir pressure is {((decimal)res[4].Item2).ToString("F1")} hPa and {res[5].Item2.ToString().ToLower()}, {elapsedRain}mm rain";

                        lastRain = (decimal)res[6].Item2;
                        //int width = 1280;
                        //int height = 720;
                        //var plot = new Plot(width, height);

                        //double[] dataX = timeHistory.Select(x => x.ToOADate()).ToArray();
                        //plot.XAxis.DateTimeFormat(true);
                        //plot.YAxis.Label("°C");
                        //plot.YAxis.Color(Color.Red);

                        //plot.SetAxisLimitsY(0, 45);

                        //var tempScatter = plot.AddScatter(dataX, temperatures.ToArray());
                        //tempScatter.LineWidth = 2;
                        //tempScatter.Color = Color.Red;

                        //var baroScatter = plot.AddScatter(dataX, barometers.ToArray());
                        //baroScatter.LineWidth = 2;
                        //baroScatter.Color = Color.DodgerBlue;

                        //var yAxis3 = plot.AddAxis(ScottPlot.Renderable.Edge.Left, axisIndex: 2);
                        //baroScatter.YAxisIndex = 2;
                        //yAxis3.Label("hPa");
                        //yAxis3.Color(baroScatter.Color);

                        //var image = ImageToPngByte(plot.Render());
                        //var uploadedImage = await userClient.Upload.UploadTweetImageAsync(image);
                        //var tweetParams = new PublishTweetParameters(resultString)
                        //{
                        //    Medias = { uploadedImage }
                        //};
                        //var tweet = await userClient.Tweets.PublishTweetAsync(tweetParams);

                        var tweet = await userClient.Tweets.PublishTweetAsync(resultString);
                        Console.WriteLine("published the tweet: " + tweet);

                        startTime = now;
                        timeHistory = new List<DateTime>();
                        temperatures = new List<double>();
                        barometers = new List<double>();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
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
