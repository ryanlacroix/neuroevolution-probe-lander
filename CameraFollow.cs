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

	// Use this for initialization
	void Start () {
        probeOffset = new Vector3(0, 10, -3);
        pop = GameObject.Find("EvolveController").GetComponent<GeneticAlgorithm>().pop;
	}
	
	// Update is called once per frame
	void Update () {
        Debug.DrawLine(transform.position - probeOffset, target.GetComponent<Transform>().position, Color.green);
        transform.position = probe.GetComponent<Transform>().position + probeOffset;
        writeUI();
    }

    void writeUI()
    {
        ProbeInstr instr = probe.GetComponent<ProbeInstr>();
        Alt.text = "Altitude: " + instr.getAltitude();
        Vel.text = "Velocity: " + instr.getSpeed();
        Dist.text = "Target Distance: " + instr.getDistanceToTarget();
        Fuel.text = "Remaining Fuel: " + probe.GetComponent<ProbeControl>().fuel;
        Gen.text = "Generation: " + pop.generation;
    }
}
