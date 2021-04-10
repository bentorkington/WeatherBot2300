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
                WS2300.AvailableSensors.RainfallPerImpulse,
                WS2300.AvailableSensors.RainCount,
                WS2300.AvailableSensors.RainCount2,
                WS2300.AvailableSensors.RainTotal,
                WS2300.AvailableSensors.LastTotalRainCount,

            };

            var results = station.GetSensors(sensors);
            foreach (var result in results)
            {
                Console.WriteLine(result);
            }

            for(int i=0; i<15; i++) {
                var s = new List<Sensor> {
                    new Sensor("4m", RawDataType.U32, new LinearConverter(0, 1m, Units.MetresPerSecond), 0x479 + i * 2, DataReaderBcd2),
                };
                results = station.GetSensors(s);
                Console.WriteLine(result);
            }
        }
    }
}
