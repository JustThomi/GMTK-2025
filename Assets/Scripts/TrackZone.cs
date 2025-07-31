using System.Runtime.CompilerServices;
using UnityEngine;

public class TrackZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CarController car = other.GetComponent<CarController>();
            car.curTrackZone = this;
            car.zonesPassed++;
        }
    }
}
