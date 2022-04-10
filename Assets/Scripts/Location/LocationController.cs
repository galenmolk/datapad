using System.Collections;
using System.Device.Location;
using System.Globalization;
using UnityEngine;

public class LocationController : MonoBehaviour
{
    private Location _columbusCircle = new(40.767997, -73.981934);
    
    private void Start()
    {
        DebugText.Instance.Print("Start");
        PermissionHandler.OnPermissionGranted.AddListener(PermissionGranted);
    }

    private void PermissionGranted()
    {        
        DebugText.Instance.Print("LocationController.PermissionGranted");
        StartCoroutine(GetLocation());
    }

    private IEnumerator GetLocation()
    {
        if (!Input.location.isEnabledByUser)
        {
            DebugText.Instance.Print("isEnabledByUser == false");
            yield break;
        }

        Input.location.Start();

        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return YieldRegistry.WaitForSeconds(1f);
            maxWait--;
        }

        if (maxWait < 1)
        {
            DebugText.Instance.Print("Timed out");
            yield break;
        }

        if (Input.location.status == LocationServiceStatus.Failed)
        {
            DebugText.Instance.Print("Unable to determine device location");
            yield break;
        }

        DebugText.Instance.Print("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);
        LocationInfo location = Input.location.lastData;

        GeoCoordinate coord = new GeoCoordinate(location.latitude, location.longitude, location.altitude, location.horizontalAccuracy, location.verticalAccuracy, 0d, 0d);
        double meters = coord.GetDistanceTo(_columbusCircle.Coord);
        DebugText.Instance.Print(meters.ToString(CultureInfo.InvariantCulture));
        Input.location.Stop();
    }
}
