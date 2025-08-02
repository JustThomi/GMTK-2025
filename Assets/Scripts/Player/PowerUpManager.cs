using UnityEngine;
using UnityEngine.InputSystem;


public class PowerUpManager : MonoBehaviour{
    public PowerUp powerUpPrefab;
    public LayerMask groundLayer;

    private Collider carCollider;
    private Transform carTransform;
    private Rigidbody carRig;
    private CarController carController;

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
        carController = GetComponent<CarController>();

        currentAbility = "";
    }

    public void SpawnPowerUps(){
        int offset = 4;
        int currentIndex = 0;

        for(int i = 0; i < carController.trackZones.Length; i++){
            if(carController.trackZones[i] == carController.curTrackZone){
                currentIndex = i;
                break;
            }
        }

        int spawnPoint = currentIndex + offset;
        TrackZone spawnTrack = carController.trackZones[spawnPoint < carController.trackZones.Length? spawnPoint: offset]; // not that readable but chill

        Transform trackTransform = spawnTrack.GetComponent<Transform>();

        PowerUp powerUpMid = Instantiate(powerUpPrefab, trackTransform.position + new Vector3(0f, -1.5f, 0f), transform.rotation);
        powerUpMid.carID = carCollider.GetInstanceID();
        powerUpMid.type = allAbilities[Random.Range(0,allAbilities.Length)];

        PowerUp powerUpLeft = Instantiate(powerUpPrefab, trackTransform.position + new Vector3(-offset, -1.5f, 0f), transform.rotation);
        powerUpLeft.carID = carCollider.GetInstanceID();
        powerUpLeft.type = allAbilities[Random.Range(0,allAbilities.Length)];

        PowerUp powerUpRight = Instantiate(powerUpPrefab, trackTransform.position + new Vector3(offset, -1.5f, 0f), transform.rotation);
        powerUpRight.carID = carCollider.GetInstanceID();
        powerUpRight.type = allAbilities[Random.Range(0,allAbilities.Length)];
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
