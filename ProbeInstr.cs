using UnityEngine;
using System.Collections;

public class ProbeInstr : MonoBehaviour {

    public float phase2Height;
    public float phase3Height;

    Rigidbody thisObj;
    public int updateRate = 50;
    private GameObject target;
    private float[] readings;
    private int frameCounter;
    private float speed;
    // For performance calculations
    public Vector3 startPosition;
    // Add in total variance from path every step
    public double totalPathVariance;

    void Start () {
        thisObj = GetComponent<Rigidbody>();
        target = GameObject.Find("Target");
        readings = new float[2];
        frameCounter = 0;
        speed = thisObj.velocity.magnitude;
        startPosition = thisObj.position;
        totalPathVariance = 0;

	}
	public float[] getReadings()
    {
        // Initially only train with two inputs
        // Amplify this to put emphasis on phase changes
        readings[0] = getPhase()*100;
        readings[1] = getDistanceToTarget() /50;

        return readings;
    }
	// Update is called once per frame
	void Update () {
        if (frameCounter >= updateRate)
        {
            // Add path variance to total
            updatePathVariance();
            updateSpeed();
            frameCounter = -1; 
        }
        frameCounter++;
	}

    private int getPhase()
    {
        if (thisObj.position.y < phase3Height)
            return 3;
        else if (thisObj.position.y < phase2Height)
            return 2;
        else
            return 1;
    }

    // Perhaps redundant, marked for removal
    public static float DistanceToLine(Ray ray, Vector3 point)
    {
        return Vector3.Cross(ray.direction, point - ray.origin).magnitude;
    }

    // Calculates how far from optimal path the probe is, adds this to total variance
    private void updatePathVariance()
    {
        double currVariance = UnityEditor.HandleUtility.DistancePointLine(thisObj.position, startPosition,
            target.GetComponent<Transform>().position);
        if (currVariance > 50)
            totalPathVariance += currVariance;
    }

    private void updateSpeed()
    {
        this.speed = thisObj.velocity.magnitude;
    }

    public float getSpeed()
    {
        return this.speed;
    }

    public float getAltitude()
    {
        return thisObj.transform.position.y;
    }
    public float getDistanceToTarget()
    {
        return Vector3.Distance(thisObj.transform.position, target.transform.position);
    }
    // A value of 0 is upright. 
    // Increments negatively on one side, positively on the other.
    public float getRoll()
    {
        if (thisObj.transform.eulerAngles.z > 180) return thisObj.transform.eulerAngles.z - 360;
        else return thisObj.transform.eulerAngles.z;
    }
    public float getPitch()
    {
        if (thisObj.transform.eulerAngles.x > 180) return thisObj.transform.eulerAngles.x - 360;
        else return thisObj.transform.eulerAngles.x;
    }
    public float getYaw()
    {
        //return thisObj.transform.right.x;
        if (thisObj.transform.eulerAngles.y > 180) return thisObj.transform.eulerAngles.y - 360;
        else return thisObj.transform.eulerAngles.y;
    }
    
    // For use in evaluation
    public float getRollFromOrigin()
    {
        float roll = getRoll();
        return System.Math.Abs(roll);
    }
    public float getPitchFromOrigin()
    {
        float pitch = getPitch();
        return System.Math.Abs(pitch);
    }

}
