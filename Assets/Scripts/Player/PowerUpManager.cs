using UnityEngine;

public class PowerUpManager : MonoBehaviour{
    public PowerUp powerUpPrefab;

    private Collider carCollider;
    private Transform carTransform;

    void Start(){
        carCollider = GetComponent<Collider>();
        carTransform = GetComponentInChildren<Transform>();
    }

    public void SpawnPowerUps(){
        PowerUp powerUpMid = Instantiate(powerUpPrefab, carTransform.position + (carTransform.forward * 2), transform.rotation); // magic numbers :D
        powerUpMid.carID = carCollider.GetInstanceID();
        Debug.Log("PowerUp SPAWNED");
    }
}
