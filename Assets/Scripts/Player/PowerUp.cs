using UnityEngine;

public class PowerUp : MonoBehaviour{
    public GameObject model;
    public int carID {set;get;}

    private void OnTriggerEnter(Collider other){

        if (carID == other.GetInstanceID()){
            Debug.Log("PowerUp collected");
            Destroy(gameObject);
        }
    }
}
