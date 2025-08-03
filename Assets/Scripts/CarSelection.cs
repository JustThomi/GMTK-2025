using UnityEngine;
using UnityEngine.SceneManagement;

public class CarSelection : MonoBehaviour
{
    public GameObject[] Cars;
    public int selectedCar = 0;

    public void NextCar()
    {
        Cars[selectedCar].SetActive(false);
        selectedCar = (selectedCar + 1) % Cars.Length;
        Cars[selectedCar].SetActive(true);
    }

    public void PreviousCar()
    {
        Cars[selectedCar].SetActive(false);
        selectedCar--;
        if (selectedCar <0)
        {
            selectedCar += Cars.Length;
        }
        Cars[selectedCar].SetActive(true);
    }

    public void StartGame()
    {
        PlayerPrefs.SetInt("selectedCar", selectedCar);
        SceneManager.LoadScene("DuplicateWithNavMesh");
    }

}
