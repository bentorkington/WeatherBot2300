using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;
using System.Diagnostics;

namespace TootingMad.DataSources.LaCrosse
{
    public enum Status
    {
        Unknown,
        Initialized,
    }
    public delegate int DataReader(int address, SerialPort comPort);


    public partial class WS2300
    {
        readonly byte[] ResetBytes = new byte[] { 0x06 };

        SerialPort _serialPort;

        Status _status = Status.Unknown;

        public WS2300(string address)
        {
            _serialPort = new SerialPort(address, 2400);
            _serialPort.RtsEnable = true;
            _serialPort.DtrEnable = false;
            _serialPort.Handshake = Handshake.None;
            _serialPort.BaudRate = 2400;
            _serialPort.ReadTimeout = 250;
        }

        enum Forecast
        {
            Rainy,
            Cloudy,
            Sunny,
        }


        static LinearConverter QuadTemperatureConverter = new LinearConverter(-30m, 0.01m, Units.DegreesCentigrade);
        static LinearConverter PressureConverter = new LinearConverter(0.0m, 0.1m, Units.Hectopascals);
        static LinearConverter HumidityConverter = new LinearConverter(0.0m, 1.0m, Units.Percent);

        public class WindDirectionConverter : IRawValueConverter
        {
            //public Unit Unit { get; set; }
            string IRawValueConverter.Unit { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

            public object Decode(int rawValue)
            {
                return rawValue.ToString();
            }

            public int Encode(decimal cookedValue)
            {
                throw new NotImplementedException();
            }
        }

        public static DataReader DataReaderByte = ReadBcd1;
        public static DataReader DataReaderBcd2 = ReadBcd2;
        public static DataReader DataReaderBcd3 = ReadBcd3;
        public static DataReader DataReaderBcd4 = ReadBcd4;
        public static DataReader DataReaderBcd5 = ReadBcd5;

        public class AvailableSensors
        {
            // temperatures
            public static Sensor OutdoorTemperature = new Sensor("Outdoor Temperature", "environment.outdoor.temperature", RawDataType.U32, QuadTemperatureConverter, 0x373, DataReaderBcd4);
            public static Sensor OutdoorHumidity = new Sensor("Outdoor Humidity", "environment.outdoor.humidity", RawDataType.U32, HumidityConverter, 0x419, DataReaderBcd2);

            public static Sensor IndoorTemperature = new Sensor("Indoor Temperature", "environment.indoor.temperature", RawDataType.U32, QuadTemperatureConverter, 0x346, DataReaderBcd4);
            public static Sensor IndoorHumidity = new Sensor("Indoor Humidity", "environment.indoor.humidity", RawDataType.U32, HumidityConverter, 0x3fb, DataReaderBcd2);

            public static Sensor DewPoint = new Sensor("Dew Point", "environment.dewPoint", RawDataType.U32, QuadTemperatureConverter, 0x3ce, DataReaderBcd4);
            public static Sensor WindChill = new Sensor("Wind Chill", "environment.outdoor.windChill", RawDataType.U32, QuadTemperatureConverter, 0x3a0, DataReaderBcd4);

            // pressures
            public static Sensor RelativePressure = new Sensor("Relative Pressure", "environment.relativePressure", RawDataType.U32, PressureConverter, 0x5e2, DataReaderBcd5);
            public static Sensor AbsolutePressure = new Sensor("Absolute Pressure", "environment.absolutePressure", RawDataType.U32, PressureConverter, 0x5d8, DataReaderBcd5);

            // rainfall
            public static Sensor RainfallPerImpulse = new Sensor("Rainfall per Impulse", "internal.rainfallPerImpulse", RawDataType.U32, new LinearConverter(0, 0.001m, Units.Millivolts), 0x437, DataReaderBcd3);
            public static Sensor RainCount = new Sensor("Rain Count", "environment.rainCount", RawDataType.U32, new LinearConverter(0, 1, Units.Millivolts), 0x43a, DataReaderBcd3);
            public static Sensor RainCount2 = new Sensor("Rain Count", "environment.rainCount", RawDataType.U32, new LinearConverter(0, 1, Units.Millivolts), 0x43d, DataReaderBcd3);
            public static Sensor LastTotalRainCount = new Sensor("Rain Count", "environment.rainCount", RawDataType.U32, new LinearConverter(0, 1, Units.Millivolts), 0x440, DataReaderBcd3);
            public static Sensor RainTotal = new Sensor("Rain Total", "environment.rainTotal", RawDataType.U32, new LinearConverter(0, 0.01m, Units.Millivolts), 0x4d2, ReadBcd5);

            // other
            public static Sensor WindDirectionDegrees = new Sensor("Wind Direction", "environment.outdoor.wind.direction", RawDataType.U32, new LinearConverter(0m, 22.5m, Units.Degrees), 0x52c, DataReaderByte);
            public static Sensor WindDirectionCardinal = new Sensor("Wind Direction", "environment.outdoor.wind.direction", RawDataType.U32, new CardinalDirectionConverter(), 0x52c, DataReaderByte);
            public static Sensor WindSpeedMetresPerSecond = new Sensor("Wind Speed", "environment.outdoor.wind.speed", RawDataType.U32, new LinearConverter(0, 0.1m, Units.MetresPerSecond), 0x529, DataReaderBcd3);
            public static Sensor WindSpeedKnots = new Sensor("Wind Speed", "environment.outdoor.wind.speed", RawDataType.U32, new LinearConverter(0, 0.194384m, Units.Knots), 0x529, DataReaderBcd3);

            public static Sensor Tendency = new Sensor("Tendency", "environment.pressureTendency", RawDataType.U8, new TendencyConverter(), 0x26c, DataReaderByte);

            public static List<Sensor> AllSensors
            {
                get
                {
                    return new List<Sensor>
                    {
                        OutdoorTemperature,
                        OutdoorHumidity,
                        RelativePressure,
                    };
                }
            }
        }

        public List<Sensor> Sensors = new List<Sensor>()
        {
            new Sensor("Indoor Temperature", "environment.indoor.temperature", RawDataType.U32, QuadTemperatureConverter, 0x346, DataReaderBcd4),
            //new EnumSensor(SensorFormat.Enum, 0x26b, "environmental.forecast", typeof(Forecast)),
            //new EnumSensor(SensorFormat.Enum, 0x26c, "environmental.tendency", typeof(Tendency)),
            //new Sensor(SensorFormat.QuadTemperature, 0x346, "environmental.temperature.indoor"),

            new Sensor("Outdoor Temperature", "environment.outdoor.temperature", RawDataType.U32, QuadTemperatureConverter, 0x373, DataReaderBcd4),
            new Sensor("Dew Point", "environment.dewPoint", RawDataType.U32, QuadTemperatureConverter, 0x3ce, DataReaderBcd4),
            new Sensor("Absolute Pressure", "environment.absolutePressure", RawDataType.U32, PressureConverter, 0x5d8, DataReaderBcd5),
            new Sensor("Relative Pressure", "environment.relativePressure", RawDataType.U32, PressureConverter, 0x5e2, DataReaderBcd5),
            new Sensor("Indoor Humidity", "environment.indoor.humidity", RawDataType.U32, HumidityConverter, 0x3fb, DataReaderBcd2),
            new Sensor("Outdoor Humidity", "environment.outdoor.humidity", RawDataType.U32, HumidityConverter, 0x3fb, DataReaderBcd2),
            new Sensor("Wind Chill", "environment.outdoor.windChill", RawDataType.U32, QuadTemperatureConverter, 0x3a0, DataReaderBcd4),
            new Sensor("Wind Direction", "environment.outdoor.wind.direction", RawDataType.U32, new LinearConverter(0m, 22.5m, Units.Degrees), 0x52c, DataReaderByte),
            new Sensor("Wind Speed", "environment.outdoor.wind.speed", RawDataType.U32, new LinearConverter(0, 0.1m, Units.MetresPerSecond), 0x529, DataReaderBcd3),
            

            //new Sensor(SensorFormat.TriSpeed, 0x529, "en.wind.speed"),  // M/S
            //new Sensor(SensorFormat.Angle, 0x52c, "en.wind.direction"), //  * 22.5 degrees
            //new Sensor(SensorFormat.Pressure, 0x5d8, "en.pressure.absolute"),
            //new Sensor(SensorFormat.Pressure, 0x5e2, "en.pressure.relative"),
        };

        public List<Tuple<Sensor, object>> GetSensors(List<Sensor> sensors)
        {
            _serialPort.Open(); // thows UnauthorizedAccessException (inner IOException("Device or resource busy") if already open somewhere else

            try
            {
                var results = new List<Tuple<Sensor, object>>();

                foreach (var sensor in sensors)
                {
                    var value = sensor.GetValue(_serialPort);
                    var cookedValue = sensor.Converter.Decode(value);

                    results.Add(new Tuple<Sensor, object>(sensor, cookedValue));
                }
                return results;
            }
            finally
            {
                _serialPort.Close();
            }
        }


        decimal ReadQuad(int baseAddress)
        {
            throw new NotImplementedException();
        }

        public static int ReadBcd4(int address, SerialPort comPort)
        {
            var Response = ReadBytes(2, address, comPort);

            return (Response[0] & 0xf) +
            (Response[0] >> 4) * 10 +
            (Response[1] & 0xf) * 100 +
            (Response[1] >> 4) * 1000;
        }

        public static int ReadBcd1(int address, SerialPort comPort)
        {
            var Response = ReadBytes(1, address, comPort);

            return (Response[0] & 0xf);
        }

        public static int ReadBcd2(int address, SerialPort comPort)
        {
            var Response = ReadBytes(1, address, comPort);

            return (Response[0] & 0xf) +
            (Response[0] >> 4) * 10;
        }

        public static int ReadBcd3(int address, SerialPort comPort)
        {
            var Response = ReadBytes(2, address, comPort);

            return (Response[0] & 0xf) +
            (Response[0] >> 4) * 10 +
            (Response[1] & 0xf) * 100;
        }

        public static int ReadBcd5(int address, SerialPort comPort)
        {
            var Response = ReadBytes(3, address, comPort);

            return (Response[0] & 0xf) +
            (Response[0] >> 4) * 10 +
            (Response[1] & 0xf) * 100 +
            (Response[1] >> 4) * 1000 +
            (Response[2] & 0xf) * 10000;
        }

        public static byte[] ReadBytes(int count, int baseAddress, SerialPort comPort)
        {
            byte[] CommandBytes = new byte[5];
            byte[] ExpectedBytes = new byte[5];
            byte[] ResetBytes = new byte[] { 0x06 };

            if (count < 1)
                throw new ArgumentException("count cannot be less than 1");

            if (count > 15)
                throw new ArgumentException("count cannot be greater than 15");

            int RetriesLeft = 10;
            bool Successful = false;
            bool IsReset = true;
            byte[] Answer = new byte[count];

            CommandBytes[0] = (byte)(0x82 + ((baseAddress & 0xf000) >> 10));
            CommandBytes[1] = (byte)(0x82 + ((baseAddress & 0x0f00) >> 6));
            CommandBytes[2] = (byte)(0x82 + ((baseAddress & 0x00f0) >> 2));
            CommandBytes[3] = (byte)(0x82 + ((baseAddress & 0x000f) << 2));
            CommandBytes[4] = (byte)(0xc6 + (count - 1) * 4);

            ExpectedBytes[0] = (byte)(0x00 + ((baseAddress & 0xf000) >> 12));
            ExpectedBytes[1] = (byte)(0x10 + ((baseAddress & 0x0f00) >> 8));
            ExpectedBytes[2] = (byte)(0x20 + ((baseAddress & 0x00f0) >> 4));
            ExpectedBytes[3] = (byte)(0x30 + ((baseAddress & 0x000f) << 0));
            ExpectedBytes[4] = (byte)(0x31 + (count - 1));

            while (RetriesLeft-- > 0 && Successful == false)
            {
                while (!IsReset)
                {
                    comPort.Write(ResetBytes, 0, 1);

                    try
                    {
                        if (comPort.ReadByte() == 0x2)
                            IsReset = true;

                        try
                        {
                            while (true)
                                comPort.ReadByte();
                        }
                        catch (TimeoutException)
                        {
                            // swallow the exception, buffer is empty
                        }
                    }
                    catch (TimeoutException)
                    {
                        // some error resetting, try again
                    }
                }

                try
                {
                    int ExpectedSum = 0;

                    for (int i = 0; i < 5; i++)
                    {
                        comPort.Write(CommandBytes, i, 1);
                        byte Got = (byte)comPort.ReadByte();

                        if (Got != ExpectedBytes[i])
                            throw new Exception();
                    }
                    for (int i = 0; i < count; i++)
                    {
                        Answer[i] = (byte)comPort.ReadByte();
                        ExpectedSum += Answer[i];
                    }

                    int Checksum = comPort.ReadByte();
                    if (Checksum != (ExpectedSum & 0xff))
                        throw new Exception("Invalid checksum");

                    Successful = true;

                }
                catch
                {
                    IsReset = false;        // some comms error, reset comms and try again
                }

            }
            if (Successful)
                return Answer;

            throw new Exception("Retry count exceeded");
        }
    }
}
