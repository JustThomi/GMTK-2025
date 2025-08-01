using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class CarController : MonoBehaviour
{
    internal enum DriverType
    {
        AI,
        Player
    }

    [SerializeField]
    private DriverType driverController;
    [Header("AI Drive")]
    public TrackWaypoints waypoints;
    public List<Transform> nodes = new List<Transform>();
    public Transform currentWaypoint;
    [Range(0, 6)] public int distanceOffset;
    [Range(0, 15)] public float steerForce;

    public float acceleration;
    public float turnSpeed;

    public Transform carModel;
    private Vector3 startModelOffset;

    public float groundCheckRate;
    private float lastGroundCheckTime;

    private float curYRot;
    public bool canControl;

    private bool accelerateInput;
    private float turnInput;

    public TrackZone curTrackZone;
    public int zonesPassed;
    public int racePosition;
    public int curLap;

    public Rigidbody rig;

    private void Start()
    {
        startModelOffset = carModel.transform.localPosition;

        waypoints = GameObject.FindGameObjectWithTag("Path").GetComponent<TrackWaypoints>();
        nodes = waypoints.nodes;
    }

    private void Update()
    {
        switch (driverController)
        {
            case DriverType.AI:
                AISteer();
                break;
            case DriverType.Player:
                PlayerSteer();
                break;
        }

        carModel.position = transform.position + startModelOffset;

        //curYRot += turnInput * turnSpeed * Time.deltaTime;

        //carModel.eulerAngles = new Vector3(0, curYRot, 0);

        CheckGround();

    }

    private void FixedUpdate()
    {
        CalculateDistanceOfWaypoints();

        switch (driverController)
        {
            case DriverType.AI:
                AIDrive();
                break;
            case DriverType.Player:
                PlayerDrive();
                break;
        }
    }

    void CheckGround()
    {
        Ray ray = new Ray(transform.position + new Vector3(0, -0.75f, 0), Vector3.down);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1.0f))
        {
            carModel.up = hit.normal;
        }
        else
        {
            carModel.up = Vector3.up;
        }

        carModel.Rotate(new Vector3(0, curYRot, 0), Space.Self);
    }
    
    // called when we press down the accelerate input
    public void OnAccelerateInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            accelerateInput = true;
        else
            accelerateInput = false;
    }

    // called when we modify the turn input
    public void OnTurnInput(InputAction.CallbackContext context)
    {
        turnInput = context.ReadValue<float>();
    }

    private void AIDrive()
    {
        rig.AddForce(carModel.forward * acceleration, ForceMode.Acceleration);
    }

    private void AISteer()
    {
        Vector3 relative = transform.InverseTransformPoint(currentWaypoint.transform.position);

        relative /= relative.magnitude;

        float turnRate = Vector3.Dot(rig.linearVelocity.normalized, carModel.forward);

        turnRate = Mathf.Abs(turnRate);

        turnInput = relative.x;

        curYRot += turnInput * turnSpeed * turnRate * Time.deltaTime;
    }

    private void PlayerDrive()
    {
        if (accelerateInput == true)
        {
            rig.AddForce(carModel.forward * acceleration, ForceMode.Acceleration);
        }
    }

    private void PlayerSteer()
    {
        float turnRate = Vector3.Dot(rig.linearVelocity.normalized, carModel.forward);

        turnRate = Mathf.Abs(turnRate);

        curYRot += turnInput * turnSpeed * turnRate * Time.deltaTime;
    }

    private void CalculateDistanceOfWaypoints()
    {
        Vector3 position = gameObject.transform.position;
        float distance = Mathf.Infinity;

        for(int i = 0; i < nodes.Count; i++)
        {
            Vector3 difference = nodes[i].position - position;
            float currentDistance = difference.magnitude;

            if(currentDistance < distance)
            {
                int targetIndex = (i + distanceOffset) % nodes.Count;
                currentWaypoint = nodes[targetIndex];
                distance = currentDistance;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(currentWaypoint.position, 3f);
    }
}