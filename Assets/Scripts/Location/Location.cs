using System.Device.Location;

public class Location
{
    public Location(double latitude, double longitude)
    {
        _latitude = latitude;
        _longitude = longitude;

        Coord = new GeoCoordinate(latitude, longitude);
    }

    public GeoCoordinate Coord { get; private set; }

    private readonly double _latitude;
    private readonly double _longitude;
}
