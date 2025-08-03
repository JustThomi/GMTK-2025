using UnityEngine;

public class PowerUp : MonoBehaviour{
    public GameObject model;
    public int carID {set;get;}
    public string type {set;get;}
    public float timer = 6.0f;

    private void OnTriggerEnter(Collider other){
        if (carID == other.GetInstanceID()){
            GameObject car = other.gameObject;
            PowerUpManager powerUpManager = car.GetComponent<PowerUpManager>();
            powerUpManager.currentAbility = type;

            Destroy(gameObject);
        }
    }

    public void Update(){
        timer -= Time.deltaTime;
        if (timer <= 0.0f) Destroy(gameObject);
    }
}
