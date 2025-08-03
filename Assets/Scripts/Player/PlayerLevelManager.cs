using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class PlayerLevelManager : MonoBehaviour
{
    private CarController cc;
    public UnityEvent hasLeveledUp;

    [Header("XP Floating Numbers")]
    public Canvas UICanvas;
    public Camera referenceCamera; 
    public float textFontSize;
    public TMP_FontAsset font;
    public Color textColor;
    public float xNumberOffset;
    public float yNumberOffset;

    [Header("Level Values")]
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

        GenerateFloatingText("+" + amount.ToString() + "XP", transform);

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
        hasLeveledUp.Invoke();
    }

    public void GenerateFloatingText(string text, Transform target, float duration = 1f, float speed = 1f)
    {
        if (!UICanvas)
        {
            return;
        }

        if (!referenceCamera)
        {
            referenceCamera = Camera.main;
        }

        StartCoroutine(GenerateFloatingTextCoroutine(text, target, duration, speed));
    }
    private IEnumerator GenerateFloatingTextCoroutine(string text, Transform target, float duration = 1f, float speed = 1f)
    {
        GameObject textObj = new GameObject("XP Floating Text");
        RectTransform rect = textObj.AddComponent<RectTransform>();
        TextMeshProUGUI tmPro = textObj.AddComponent<TextMeshProUGUI>();

        tmPro.text = text;
        tmPro.horizontalAlignment = HorizontalAlignmentOptions.Center;
        tmPro.verticalAlignment = VerticalAlignmentOptions.Middle;
        tmPro.fontSize = textFontSize;
        tmPro.color = textColor;
        if (font) tmPro.font = font;

        textObj.transform.SetParent(UICanvas.transform, false);
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);

        rect.anchoredPosition = new Vector2(xNumberOffset, yNumberOffset);

        WaitForEndOfFrame w = new WaitForEndOfFrame();
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;

            tmPro.color = new Color(tmPro.color.r, tmPro.color.g, tmPro.color.b, 1 - t / duration);

            rect.anchoredPosition += new Vector2(0, speed * Time.deltaTime * 50f);

            yield return w;
        }

        Destroy(textObj);
    }
}
