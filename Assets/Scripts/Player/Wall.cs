using UnityEngine;

public class Wall : MonoBehaviour
{
    public float timer = 5.0f;

    public void Update(){
        timer -= Time.deltaTime;
        if (timer <= 0.0f) Destroy(gameObject);
    }
}
