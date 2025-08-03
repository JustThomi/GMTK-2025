using System.Runtime.CompilerServices;
using UnityEngine;

public class TrackZone : MonoBehaviour
{
    public bool isGate;

    public int xpToAdd;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CarController car = other.GetComponent<CarController>();
            car.curTrackZone = this;
            car.zonesPassed++;

            if(car.curLap == 0)
                car.curLap++;

            if (isGate)// && car.zonesPassed == car.trackZones.Length + 1)
            {
                car.zonesPassed = 1;
                car.curLap++;
                PlayerLevelManager lvl = other.GetComponent<PlayerLevelManager>();
                lvl.AddXp(xpToAdd);
                //GameManager.instance.CheckIsWinner(car);
            }
        }
    }
}
