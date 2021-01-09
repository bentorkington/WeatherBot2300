namespace TootingMad.DataSources.LaCrosse
{
    public class CardinalDirectionConverter : IRawValueConverter
    {
        public string Unit { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public object Decode(int rawValue)
        {
            string[] directions =
            {
                "N", "NNE", "NE", "ENE",
                "E", "ESE", "SE", "SSE",
                "S", "SSW", "SW", "WSW",
                "W", "WNW", "NW", "NNW",
            };
            return directions[rawValue];
        }

        public int Encode(decimal cookedValue)
        {
            throw new System.NotImplementedException();
        }
    }
}

