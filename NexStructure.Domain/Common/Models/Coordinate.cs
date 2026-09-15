using NexStructure.Domain.Common.Constants;

namespace NexStructure.Domain.Common.Models;

public record Coordinate(double Longitude, double Latitude);

public static class CoordinateExtensions
{

    public static double DistanceFrom(this Coordinate pointA, Coordinate pointB)
    {

        double latDistance = ToRadians(pointB.Latitude - pointA.Latitude);
        double lonDistance = ToRadians(pointB.Longitude - pointA.Longitude);
        double sinLat = Math.Sin(latDistance / 2);
        double sinLon = Math.Sin(lonDistance / 2);
        double a = sinLat * sinLat +
                   Math.Cos(ToRadians(pointA.Latitude)) * Math.Cos(ToRadians(pointB.Latitude)) *
                   sinLon * sinLon;
        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return Geometry.EarthRadiusInMeters * c;
    }
    private static double ToRadians(double degrees) => degrees * (Math.PI / 180);
}
