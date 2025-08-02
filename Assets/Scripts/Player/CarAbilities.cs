using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

// Testing stuff -> don't use in final
public abstract class Ability{
    public abstract void UseAbility();
}

public class JumpAbility: Ability{
    public Rigidbody rig;
    public Transform transform;
    private int jumpForce;

    public JumpAbility(Transform carTransform, Rigidbody carRig, int force){
        transform = carTransform;
        rig = carRig;
        jumpForce = force;
    }

    public override void UseAbility(){
        bool grounded = Physics.Raycast(transform.position, Vector3.down, 0.5f);

        if (grounded) rig.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
}

public class BoostAbility: Ability{
    public Rigidbody rig;
    private Transform transform;
    private int speed;

    public BoostAbility(Transform carTransform, Rigidbody carRig, int force){
        transform = carTransform;
        rig = carRig;
        speed = force;
    }

    public override void UseAbility(){
        rig.AddForce(transform.forward * speed, ForceMode.Impulse);
    }
}

public class CarAbilities : MonoBehaviour{
    public GameObject carModel;
    private Transform carTransform;
    private Rigidbody carRig;

    List<Ability> abilities;
    private int currentAbility;
    
    public void Start(){
        carTransform = carModel.GetComponent<Transform>();
        carRig = GetComponent<Rigidbody>();

        abilities = new List<Ability>(){
            new JumpAbility(carTransform, carRig, 20),
            new BoostAbility(carTransform, carRig, 20),
        };
        currentAbility = 0;
    }

    public void OnSwapAbilityInput(InputAction.CallbackContext context){
        if (context.started) currentAbility = currentAbility + 1 >= abilities.Count ? 0: currentAbility + 1;
    }

    public void OnAbilityInput(InputAction.CallbackContext context){
        if (context.started) abilities[currentAbility].UseAbility();
    }
}
