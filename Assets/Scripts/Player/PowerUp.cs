using UnityEngine;

public class PowerUp : MonoBehaviour{
    public GameObject model;
    public int carID {set;get;}
    public string type {set;get;}

    private void OnTriggerEnter(Collider other){
        if (carID == other.GetInstanceID()){
            GameObject car = other.gameObject;
            PowerUpManager powerUpManager = car.GetComponent<PowerUpManager>();
            powerUpManager.currentAbility = type;

            Destroy(gameObject);
        }
    }
}
