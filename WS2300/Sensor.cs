using System.IO.Ports;

namespace TootingMad.DataSources.LaCrosse
{
    public class Sensor
    {
        public string Description { get; set; }
        public string Identifier { get; }
        public DataReader Reader;
        public int Register { get; }
        public IRawValueConverter Converter { get; }
        RawDataType _rawType;

        public Sensor(string description, string identifier, RawDataType rawType, IRawValueConverter converter, int address, DataReader reader)
        {
            Identifier = identifier;
            Description = description;
            Register = address;
            Reader = reader;
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

        public string Id { get; }

        public override string ToString()
        {
            return Description;
        }

        public int GetValue(SerialPort comPort)
        {
            return Reader(Register, comPort);
        }
    }
}
