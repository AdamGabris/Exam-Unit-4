namespace WeatherApp;

public class WeatherForecast
{
    public string type { get; set; }
    public Geometry geometry { get; set; }
    public Properties properties { get; set; }
}

public class Geometry
{
    public string type { get; set; }
    public List<double> coordinates { get; set; }
}

public class Properties
{
    public Meta meta { get; set; }
    public List<TimeData> timeseries { get; set; }
}

public class Meta
{
    public string updated_at { get; set; }
    public Units units { get; set; }
}

public class Units
{
    public string air_pressure_at_sea_level { get; set; }
    public string air_temperature { get; set; }
    public string cloud_area_fraction { get; set; }
    public string precipitation_amount { get; set; }
    public string relative_humidity { get; set; }
    public string wind_from_direction { get; set; }
    public string wind_speed { get; set; }
}

public class TimeData
{
    public string time { get; set; }
    public Data data { get; set; }
}

public class Data
{
    public Instant instant { get; set; }
    public NextHours next_12_hours { get; set; }
    public NextHours next_1_hours { get; set; }
    public NextHours next_6_hours { get; set; }
}

public class Instant
{
    public Details details { get; set; }
}

public class NextHours
{
    public Summary summary { get; set; }
    public Details details { get; set; }
}

public class Summary
{
    public string symbol_code { get; set; }
}

public class Details
{
    public double? precipitation_amount { get; set; }
    public double? air_pressure_at_sea_level { get; set; }
    public double? air_temperature { get; set; }
    public double? cloud_area_fraction { get; set; }
    public double? relative_humidity { get; set; }
    public double? wind_from_direction { get; set; }
    public double? wind_speed { get; set; }
}
public class WeatherLogEntry
{
    public DateTime Date { get; set; }
    public Details UserMeasurements { get; set; }
    public Details YrMeasurements { get; set; }
}