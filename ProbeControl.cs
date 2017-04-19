using UnityEngine;
using System.Collections;

public class ProbeControl : MonoBehaviour {
    Rigidbody thisObj; // Apply control outputs to this
    Transform targetObj;
    public float thrust = 800f;
    public float fuel = 200f;
    public float turnPow = 5f;
    public NeuralNetwork.Network network;
    private ProbeInstr instruments;
    private ParticleSystem thisJet;
    private bool jetFiring = false;

    void Start() {
        thisObj = GetComponent<Rigidbody>();
        targetObj = GameObject.Find("Target").GetComponent<Transform>();
        instruments = GetComponent<ProbeInstr>();
        thisJet = GetComponentInChildren<ParticleSystem>();
    }
	
	void Update () {
        // Only for use in testing
        checkForInput();

        // Stop the jet graphic if stationary
        //if (instruments.getSpeed() < 1)
            //thisJet.Stop();
    }
    void FixedUpdate()
    {
        NNControl();
    }

    // Retrieve instructions from the neural network
    void NNControl()
    {
        if (thisObj.position.y > 10)
        {
            float[] outputs = network.update(instruments.getReadings());
            centerAwayFromTarget(outputs[0]);
            cruiseTowardTarget(outputs[1]);
            centerOnTarget(outputs[2]);
            fireThruster(outputs[3]);
            goVertical(outputs[4]);
        }
        else
        {
            thisJet.Stop();
        }
    }

    // Ensure no collisions between probes
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "probe")
            Physics.IgnoreCollision(collision.collider, GetComponent<Collider>());
    }

    // For use only in testing
    void checkForInput()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            if (fuel > 0)
            {
                fireMainThruster();
            }
        }
        if (Input.GetKey(KeyCode.D))
        {
            rollRight();
        } if (Input.GetKey(KeyCode.S))
        {
            goVertical(1f);
        } if (Input.GetKey(KeyCode.W))
        {
            pitchDown();
        }
        if (Input.GetKey(KeyCode.A))
        {
            rollLeft();
        }
    }
    // NN control functions
    // Used for activation function allowing -1 to 1
    void yaw(float strength)
    {
        thisObj.AddTorque((transform.up * turnPow) * strength);
    }
    void pitch(float strength)
    {
        thisObj.AddTorque((transform.right * turnPow) * strength);
    }
    void roll(float strength)
    {
        thisObj.AddTorque((transform.forward * turnPow) * strength);
    }
    void fireThruster(float strength)
    {
        if (strength > 0f && fuel > 0)
        {
            if (jetFiring == false)
            {
                jetFiring = true;
                thisJet.Play();
            }
                
            thisObj.AddForce((thisObj.transform.up * thrust) * strength);
            fuel -= strength;
        } else
        {
            jetFiring = false;
            thisJet.Stop();
        }
    }
    void centerOnTarget(float strength)
    {
        if (strength > 0.5f)
        {
            Quaternion targetRotation;
            Vector3 direction = (targetObj.position - thisObj.transform.position);
            targetRotation = Quaternion.FromToRotation(Vector3.up, direction);
            thisObj.transform.rotation = Quaternion.Lerp(thisObj.transform.rotation, targetRotation, Time.deltaTime * 1f);

        }
    }
    void centerAwayFromTarget(float strength)
    {
        if (strength > 0.5f)
        {
            Quaternion targetRotation;
            Vector3 direction = (thisObj.transform.position - targetObj.position);
            targetRotation = Quaternion.FromToRotation(thisObj.transform.up, direction);
            thisObj.transform.rotation = Quaternion.Lerp(thisObj.transform.rotation, targetRotation, Time.deltaTime * 1f);
        }
    }
    void cruiseTowardTarget(float strength)
    {
        if (strength > 0.5)
        {
            Quaternion targetRotation;
            Vector3 direction = (targetObj.position - thisObj.transform.position);
            targetRotation = Quaternion.FromToRotation(thisObj.transform.up, direction);
            thisObj.transform.rotation = Quaternion.Lerp(thisObj.transform.rotation, targetRotation, Time.deltaTime * 1f);
        }
    }
    void goVertical(float strength)
    {
        if (strength > 0.5)
        {
            Quaternion targetRotation;
            Vector3 direction = Vector3.up;
            targetRotation = Quaternion.FromToRotation(Vector3.up, direction);
            thisObj.transform.rotation = Quaternion.Lerp(thisObj.transform.rotation, targetRotation, Time.deltaTime * 1f);
        }
    }

    // User control functions
    // For use only in testing
    void fireMainThruster()
    {
        thisObj.AddForce(thisObj.transform.up * thrust);
        fuel -= 1;
    }
    void yawLeft()
    {
        thisObj.AddTorque(transform.up * -turnPow);
    }
    
    void yawRight()
    {
        thisObj.AddTorque(transform.up * turnPow);
    }
    void pitchUp()
    {
        thisObj.AddTorque(transform.right * turnPow);
    }
    void pitchDown()
    {
        thisObj.AddTorque(transform.right * -turnPow);
    }
    void rollLeft()
    {
        thisObj.AddTorque(transform.forward * turnPow);
    }
    void rollRight()
    {
        thisObj.AddTorque(transform.forward * -turnPow);
    }
}
