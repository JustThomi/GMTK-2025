using UnityEngine;
using UnityEngine.InputSystem;


public class PowerUpManager : MonoBehaviour{
    public PowerUp powerUpPrefab;
    public LayerMask groundLayer;

    private Collider carCollider;
    private Transform carTransform;
    private Rigidbody carRig;

    // test basic abilities
    public string[] allAbilities = new string[]{
        "jump",
        "boost",
    };
    public string currentAbility {set;get;}

    void Start(){
        carCollider = GetComponent<Collider>();
        carTransform = GetComponentInChildren<Transform>();
        carRig = GetComponent<Rigidbody>();

        currentAbility = "";
    }

    public void SpawnPowerUps(){
        int offset = 4;
        PowerUp powerUpMid = Instantiate(powerUpPrefab, carTransform.position + (carTransform.forward * offset), transform.rotation); // magic numbers :D
        powerUpMid.carID = carCollider.GetInstanceID();
        powerUpMid.type = allAbilities[Random.Range(0,allAbilities.Length)];
        Debug.Log("Spawned");
    }

    public void jump(){
        int jumpForce = 20;
        bool grounded = Physics.Raycast(carTransform.position, Vector3.down, 10.0f, groundLayer);

        if (grounded){
            carRig.AddForce(carTransform.up * jumpForce, ForceMode.Impulse);
        } 
    }

    public void boost(){
        int speed = 20;
        carRig.AddForce(carTransform.forward * speed, ForceMode.Impulse);
    }
    
    public void OnAbilityInput(InputAction.CallbackContext context){
        Debug.Log("current ability: " + currentAbility);
        if (context.started){
            switch (currentAbility){
                case "jump":
                    jump();
                    break;
                case "boost":
                    boost();
                    break;
                default:
                    Debug.Log("Skill issue");
                    break;
            }
            currentAbility = ""; // one time use
        }
    }
}
