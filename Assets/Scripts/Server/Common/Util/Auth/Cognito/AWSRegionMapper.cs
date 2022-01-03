using System.Collections;
using System.Collections.Generic;
using Amazon;

public sealed class AWSRegionMapper
{
    public static Dictionary<string, RegionEndpoint> regionMapper = new Dictionary<string, RegionEndpoint>();

    static AWSRegionMapper() {
        regionMapper.Add(RegionEndpoint.AFSouth1.DisplayName, RegionEndpoint.AFSouth1);
        regionMapper.Add(RegionEndpoint.APEast1.DisplayName, RegionEndpoint.APEast1);
        regionMapper.Add(RegionEndpoint.APNortheast1.DisplayName, RegionEndpoint.APNortheast1);
        regionMapper.Add(RegionEndpoint.APNortheast2.DisplayName, RegionEndpoint.APNortheast2);
        regionMapper.Add(RegionEndpoint.APNortheast3.DisplayName, RegionEndpoint.APNortheast3);
        regionMapper.Add(RegionEndpoint.APSouth1.DisplayName, RegionEndpoint.APSouth1);
        regionMapper.Add(RegionEndpoint.APSoutheast1.DisplayName, RegionEndpoint.APSoutheast1);
        regionMapper.Add(RegionEndpoint.APSoutheast2.DisplayName, RegionEndpoint.APSoutheast2);
        regionMapper.Add(RegionEndpoint.CACentral1.DisplayName, RegionEndpoint.CACentral1);
        regionMapper.Add(RegionEndpoint.CNNorth1.DisplayName, RegionEndpoint.CNNorth1);
        regionMapper.Add(RegionEndpoint.CNNorthWest1.DisplayName, RegionEndpoint.CNNorthWest1);
        regionMapper.Add(RegionEndpoint.EUCentral1.DisplayName, RegionEndpoint.EUCentral1);
        regionMapper.Add(RegionEndpoint.EUNorth1.DisplayName, RegionEndpoint.EUNorth1);
        regionMapper.Add(RegionEndpoint.EUSouth1.DisplayName, RegionEndpoint.EUSouth1);
        regionMapper.Add(RegionEndpoint.EUWest1.DisplayName, RegionEndpoint.EUWest1);
        regionMapper.Add(RegionEndpoint.EUWest2.DisplayName, RegionEndpoint.EUWest2);
        regionMapper.Add(RegionEndpoint.EUWest3.DisplayName, RegionEndpoint.EUWest3);
        regionMapper.Add(RegionEndpoint.MESouth1.DisplayName, RegionEndpoint.MESouth1);
        regionMapper.Add(RegionEndpoint.SAEast1.DisplayName, RegionEndpoint.SAEast1);
        regionMapper.Add(RegionEndpoint.USEast1.DisplayName, RegionEndpoint.USEast1);
        regionMapper.Add(RegionEndpoint.USEast2.DisplayName, RegionEndpoint.USEast2);
        regionMapper.Add(RegionEndpoint.USGovCloudEast1.DisplayName, RegionEndpoint.USGovCloudEast1);
        regionMapper.Add(RegionEndpoint.USGovCloudWest1.DisplayName, RegionEndpoint.USGovCloudWest1);
        regionMapper.Add(RegionEndpoint.USIsobEast1.DisplayName, RegionEndpoint.USIsobEast1);
        regionMapper.Add(RegionEndpoint.USIsoEast1.DisplayName, RegionEndpoint.USIsoEast1);
        regionMapper.Add(RegionEndpoint.USIsoWest1.DisplayName, RegionEndpoint.USIsoWest1);
        regionMapper.Add(RegionEndpoint.USWest1.DisplayName, RegionEndpoint.USWest1);
        regionMapper.Add(RegionEndpoint.USWest2.DisplayName, RegionEndpoint.USWest2);
    }
}
