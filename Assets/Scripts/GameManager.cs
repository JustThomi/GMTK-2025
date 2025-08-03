using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public List<CarController> cars = new List<CarController>();
    public Transform[] spawnPoints;
    public float lapsToFinish;

    public GameObject carAI;

    public float positionUpdateRate = 0.05f;
    private float lastPositionUpdateTime;

    public bool gameStarted = false;

    public int playersToBegin = 2;

    public int botsSpanwed;

    public static GameManager instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        while (botsSpanwed < playersToBegin - 1)
        {
            Instantiate(carAI);
            botsSpanwed++;
        }
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
        PlayerUI[] uis = FindObjectsByType<PlayerUI>(FindObjectsSortMode.None);

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

        for (int x = 0; x < cars.Count; x++)
        {
            cars[x].racePosition = x + 1;
        }
    }

    int SortPosition(CarController a, CarController b)
    {
        if (a.curLap > b.curLap) return -1;
        if (a.curLap < b.curLap) return 1;

        if (a.zonesPassed > b.zonesPassed) return -1;
        if (a.zonesPassed < b.zonesPassed) return 1;

        float aDist = Vector3.Distance(a.transform.position, a.curTrackZone.transform.position);
        float bDist = Vector3.Distance(b.transform.position, b.curTrackZone.transform.position);

        const float tolerance = 0.3f;
        float diff = aDist - bDist;

        if (Mathf.Abs(diff) < tolerance)
            return 0;

        return diff < 0 ? -1 : 1;
    }
}
