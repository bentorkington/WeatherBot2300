namespace TootingMad.DataSources.LaCrosse
{
    public class LinearConverter : IRawValueConverter
    {
        public decimal Offset { get; set; }
        public decimal Factor { get; set; }
        public string Unit { get; set; }

        public LinearConverter(decimal offset, decimal factor, string unit)
        {
            Offset = offset;
            Factor = factor;
            Unit = unit;
        }

        public object Decode(int rawValue)
        {
            return (rawValue * Factor) + Offset;
        }

        public int Encode(decimal cookedValue)
        {
            return (int)((cookedValue - Offset) / Factor);
        }
    }
}

