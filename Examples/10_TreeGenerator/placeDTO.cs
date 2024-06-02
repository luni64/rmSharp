using Geolocation;

namespace rmtester
{
    class placeDTO
    {
        public string state = string.Empty;
        public string county = string.Empty;
        public string city = string.Empty;
        public double lat;
        public double lon;
        public Coordinate location => new Coordinate(lat, lon);
        public string Name => $"{city},{county},{state},United States";
        public string Reverse => $"United States,{state},{county},{city}";
        public override string ToString() => Name;
    }
}








