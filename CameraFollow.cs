using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CameraFollow : MonoBehaviour {
    public Vector3 probeOffset;
    public GameObject probe;
    public GameObject target;
    public GeneticAlgorithm.Population pop;
    public Text Alt;
    public Text Vel;
    public Text Dist;
    public Text Fuel;
    public Text Gen;
    public Text Phase;
    public Text Impact;
    private bool landed = false;
    private float zoom;

	// Use this for initialization
	void Start () {
        zoom = 10;
        probeOffset = new Vector3(0, zoom, -3);
        pop = GameObject.Find("EvolveController").GetComponent<GeneticAlgorithm>().pop;
	}
	
	// Update is called once per frame
	void Update () {
        Debug.DrawLine(transform.position - probeOffset, target.GetComponent<Transform>().position, Color.green);
        transform.position = probe.GetComponent<Transform>().position + probeOffset;
        writeUI();
        handleZoom();
    }

    private void handleZoom()
    {
        // Zoom with the mouse wheel
        float mouseZoomDelta = -(Input.GetAxis("Mouse ScrollWheel"));
        if (mouseZoomDelta != 0)
        {
            float nextZoom = (10 * mouseZoomDelta);
            zoom += nextZoom;
            probeOffset = new Vector3(0, zoom, -3);
        }
    }

    void writeUI()
    {
        // Write all UI elements
        ProbeInstr instr = probe.GetComponent<ProbeInstr>();
        Alt.text = "Altitude: " + instr.getAltitude();
        Vel.text = "Velocity: " + instr.getSpeed();
        Dist.text = "Target Distance: " + instr.getDistanceToTarget();
        Fuel.text = "Remaining Fuel: " + probe.GetComponent<ProbeControl>().fuel;
        Phase.text = "Current Phase: " + instr.getPhase();
        Gen.text = "Generation: " + pop.generation;

        // Set impact velocity only if probe has hit the ground
        float impactVelocity = instr.impactVelocity;
        if (impactVelocity == 0f)
            Impact.text = "Impact Velocity: Not yet landed";
        else
            Impact.text = "Impact Velocity: " + impactVelocity;
    }
}
