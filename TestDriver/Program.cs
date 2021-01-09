using System;
using System.Collections.Generic;
using TootingMad.DataSources.LaCrosse;

namespace TestDriver
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var station = new WS2300(Environment.GetEnvironmentVariable("WEATHERBOT_SERIAL_PORT"));

            var sensors = new List<Sensor>
            {
                WS2300.AvailableSensors.WindDirectionCardinal,
                WS2300.AvailableSensors.WindSpeedKnots,
                WS2300.AvailableSensors.OutdoorTemperature,
                WS2300.AvailableSensors.OutdoorHumidity,
                WS2300.AvailableSensors.RelativePressure,
                WS2300.AvailableSensors.Tendency,
            };

            var results = station.GetSensors(sensors);
            foreach (var result in results)
            {
                Console.WriteLine(result);
            }
        }
    }
}
