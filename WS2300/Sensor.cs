using System.IO.Ports;

namespace TootingMad.DataSources.LaCrosse
{
    public class Sensor
    {
        public string Description { get; set; }
        public DataReader DataReader;
        public int Register { get; }
        public IRawValueConverter Converter { get; }
        RawDataType _rawType;

        public Sensor(string description, RawDataType rawType, IRawValueConverter converter, int address, DataReader reader)
        {
            Description = description;
            Register = address;
            DataReader = reader;
            Converter = converter;
            _rawType = rawType;
        }

        public string Address
        {
            get
            {
                char sizeChar = '?';
                switch (_rawType)
                {
                    case RawDataType.U8: sizeChar = 'c'; break;
                    case RawDataType.U16: sizeChar = 'd'; break;
                    case RawDataType.U32: sizeChar = 'q'; break;
                }
                return $"{sizeChar}@{Register}";
            }
        }

        public override string ToString()
        {
            return Description;
        }

        public int GetValue(SerialPort comPort)
        {
            return DataReader(Register, comPort);
        }
    }
}
