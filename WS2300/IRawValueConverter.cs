namespace TootingMad.DataSources.LaCrosse
{
    public interface IRawValueConverter
    {
        string Unit { get; set; }

        object Decode(int rawValue);
        int Encode(decimal cookedValue);
    }
}
