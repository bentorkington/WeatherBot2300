using System;

namespace TootingMad.DataSources.LaCrosse
{
    enum Tendency
    {
        Steady,
        Rising,
        Falling,
    }

    public class TendencyConverter : IRawValueConverter
    {
        //public Unit Unit { get; set; }
        string IRawValueConverter.Unit { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public object Decode(int value)
        {
            Tendency t = (Tendency)value;
            return t.ToString();
        }

        public int Encode(decimal cookedValue)
        {
            throw new NotImplementedException();
        }
        public override string ToString()
        {
            return base.ToString();
        }
    }
}
