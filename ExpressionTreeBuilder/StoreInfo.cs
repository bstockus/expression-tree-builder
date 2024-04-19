using Newtonsoft.Json;

namespace ExpressionTreeBuilder;

public class StoreSchedule
{
    [JsonProperty("dayOfWeek")]
    public DayOfWeek DayOfWeek { get; set; }

    [JsonProperty("openTime")]
    public TimeSpan OpenTime { get; set; }

    [JsonProperty("closeTime")]
    public TimeSpan CloseTime { get; set; }
}

public class StoreInfo
{
    [JsonProperty("number")] public string Number { get; set; }

    [JsonProperty("name")] public string Name { get; set; }

    [JsonProperty("storeAttributes")] public List<string> StoreAttributes { get; set; }

    [JsonProperty("open24Hours")] public bool Open24Hours { get; set; }

    [JsonProperty("siteType")] public int SiteType { get; set; }

    [JsonProperty("siteTypeDescription")] public string SiteTypeDescription { get; set; }

    [JsonProperty("hours")] public List<StoreSchedule> Hours { get; set; }

    [JsonProperty("timeZone")] public string TimeZone { get; set; } = "Central Standard Time";

    [JsonProperty("storeLeaderName")] public string StoreLeaderName { get; set; }

    [JsonProperty("divisionNumber")] public int DivisionNumber { get; set; }

    [JsonProperty("divisionDescription")] public string DivisionDescription { get; set; }

    [JsonProperty("zoneNumber")] public int ZoneNumber { get; set; }

    [JsonProperty("zoneName")] public string ZoneName { get; set; }

    [JsonProperty("zoneLeaderName")] public string ZoneLeaderName { get; set; }

    [JsonProperty("districtNumber")] public int DistrictNumber { get; set; }

    [JsonProperty("districtName")] public string DistrictName { get; set; }

    [JsonProperty("districtLeaderName")] public string DistrictLeaderName { get; set; }

    [JsonProperty("foodServiceDistrictNumber")]
    public int FoodServiceDistrictNumber { get; set; }

    [JsonProperty("foodServiceDistrictName")]
    public string FoodServiceDistrictName { get; set; }

    [JsonProperty("foodServiceDistrictLeaderName")]
    public string FoodServiceDistrictLeaderName { get; set; }

    [JsonProperty("foodServiceZoneNumber")]
    public int FoodServiceZoneNumber { get; set; }

    [JsonProperty("foodServiceZoneName")] public string FoodServiceZoneName { get; set; }

    [JsonProperty("foodServiceZoneLeaderName")]
    public string FoodServiceZoneLeaderName { get; set; }

    [JsonProperty(PropertyName = "FSDLEmail")]
    public string FsdlEmail { get; set; }

    [JsonProperty(PropertyName = "State")] public string State { get; set; }

    [JsonIgnore] public bool IsTopg { get; set; }

    [JsonProperty("isClosedForRebuild")] public bool? IsClosedForRebuild { get; set; }

    [JsonProperty("rebuildStartDate")] public DateTime? RebuildStartDate { get; set; }

    [JsonProperty("rebuildEndDate")] public DateTime? RebuildEndDate { get; set; }
}

public static class StoreInfoExtensions
{
    public static bool ContainsAttribute(this StoreInfo storeInfo, string attribute) =>
        storeInfo.StoreAttributes.Contains(attribute);

    public static string ContainsAttributeMethodName => nameof(ContainsAttribute);
}