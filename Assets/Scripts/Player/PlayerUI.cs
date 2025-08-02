using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public TextMeshProUGUI carPositionText;
    public TextMeshProUGUI lapText;
    public TextMeshProUGUI countdownText;
    public TextMeshProUGUI levelText;
    public CarController car;
    public PlayerLevelManager lvlManager;
    public Slider expSlider;

    void Update()
    {
        carPositionText.text = car.racePosition.ToString() + " / " + GameManager.instance.cars.Count.ToString();
        lapText.text = (car.curLap == 0)
             ? "Lap: " + (car.curLap + 1).ToString() + " / " + GameManager.instance.lapsToFinish.ToString()
             : "Lap: " + car.curLap.ToString() + " / " + GameManager.instance.lapsToFinish.ToString();
        expSlider.maxValue = lvlManager.xpToLevelUp;
        expSlider.value = lvlManager.currentXp;
        levelText.text = "Level: " + lvlManager.currentLevel.ToString();
    }

    public void StartCountdownDisplay()
    {
        StartCoroutine(Countdown());

        IEnumerator Countdown()
        {
            countdownText.gameObject.SetActive(true);
            countdownText.text = "3";
            yield return new WaitForSeconds(1.0f);
            countdownText.text = "2";
            yield return new WaitForSeconds(1.0f);
            countdownText.text = "1";
            yield return new WaitForSeconds(1.0f);
            countdownText.text = "GO!";
            yield return new WaitForSeconds(1.0f);
            countdownText.gameObject.SetActive(false);
        }
    }

}