using UnityEngine;

public class PlayerLevelManager : MonoBehaviour
{
    private CarController cc;

    public int currentLevel;
    public int currentXp;
    public int xpToLevelUp;

    void Start()
    {
        cc = GetComponent<CarController>();
    }

    void Update()
    {
        
    }

    public void AddXp(int amount)
    {
        currentXp += amount;

        while (currentXp >= xpToLevelUp)
        {
            int excessExp = currentXp - xpToLevelUp;

            LevelUp();

            xpToLevelUp += Mathf.RoundToInt(xpToLevelUp * 0.20f);

            currentXp = excessExp;
        }
    }

    public void LevelUp()
    {
        Debug.Log("Yeppy");
        currentLevel++;
        cc.acceleration += 10;
    }
}
