using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public List<CarController> cars = new List<CarController>();
    public Transform[] spawnPoints;

    public float positionUpdateRate = 0.05f;
    private float lastPositionUpdateTime;

    public bool gameStarted = false;

    public int playersToBegin = 2;

    public static GameManager instance;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (Time.time - lastPositionUpdateTime > positionUpdateRate)
        {
            lastPositionUpdateTime = Time.time;
            UpdateCarRacePositions();
        }

        if(!gameStarted && cars.Count == playersToBegin)
        {
            gameStarted = true;
            StartCountdown();
        }

    }

    private void StartCountdown()
    {
        PlayerUI[] uis = FindObjectsOfType<PlayerUI>();

        for (int x = 0; x < uis.Length; ++x)
            uis[x].StartCountdownDisplay();

        Invoke("BeginGame", 3.0f);
    }

    private void BeginGame()
    {
       for(int x = 0; x < cars.Count; ++x)
        {
            cars[x].canControl = true;
        }
    }

    private void UpdateCarRacePositions()
    {
        cars.Sort(SortPosition);

        for(int x = 0; x < cars.Count; x++)
        {
            cars[x].racePosition = cars.Count - x;
        }
    }

    int SortPosition (CarController a, CarController b)
    {
        if(a.zonesPassed > b.zonesPassed)
            return 1;
        else if(b.zonesPassed > a.zonesPassed)
            return -1;

        float aDist = Vector3.Distance(a.transform.position, a.curTrackZone.transform.position);
        float bDist = Vector3.Distance(b.transform.position, b.curTrackZone.transform.position);

        return aDist > bDist ? -1 : 1;
    }

}
