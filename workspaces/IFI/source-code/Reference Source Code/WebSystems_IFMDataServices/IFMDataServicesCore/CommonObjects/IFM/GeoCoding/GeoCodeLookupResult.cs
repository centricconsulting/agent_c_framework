namespace IFM.DataServicesCore.CommonObjects.IFM.GeoCoding
{
    public class GeoCodeLookupResult
    {
        public string HouseNumber { get; set; }
        public string StreetName { get; set; }
        public string City { get; set; }
        public string Township { get; set; }
        public string County { get; set; }
        public string State { get; set; }
        public string State_Long { get; set; }
        public string Nation { get; set; }
        public string ZipCode { get; set; }
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public Coordinate Coordinates  { get; set; }
        public bool IsZipCodeOnlyLookup { get; set; }
        public bool IsFullAddressLookup { get; set; }
        public bool IsLowConfidenceResult { get; set; }
    }

    public class Coordinate
    {
        public double Lat { get; set; }
        public double Lng { get; set; }
    }
}
