using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NeuralNetwork : MonoBehaviour {
    public static class Util
    {
        private static System.Random rnd = new System.Random();
        public static double GetRandom()
        {
            return rnd.NextDouble();
        }
    }

    // Represents weights between nodes
    public class Weight
    {
        public double value;
        public Weight(double val)
        {
            this.value = val;
        }
        // For XmlSerialize use only
        public Weight() { }
    }

    // Applies a bias to each node
    public class Bias
    {
        public double value;
        public Bias(double val)
        {
            this.value = val;
        }
        // For XmlSerialize use only
        public Bias() { }
    }

    // Base unit of the network
    public class Node
    {
        public List<Weight> weights;
        public Bias bias;
        public double value;
        
        public Node(Bias b)
        {
            this.bias = b;
            this.weights = new List<Weight>();
        }
        // For XmlSerialize use only
        public Node() { }
    }

    // Class containing one layer of nodes
    public class Layer
    {
        public List<Node> nodes;
        public int nodeCount
        {
            get
            {
                return nodes.Count;
            }
        }
        public Layer(int numNodes)
        {
            nodes = new List<Node>(numNodes);
        }
        // For XmlSerialize use only
        public Layer() { }
    }

    // Start defining the network itself
    public class Network
    {
        // For XmlSerialize use only
        public Network() { }
        public List<Layer> layers { get; set; }
        public int numLayers
        {
            get
            {
                return layers.Count;
            }
        }

        // Returns a representation of the network state
        public List<List<double>> getNetworkState()
        {
            List<List<double>> networkState;
            networkState = new List<List<double>>();
            networkState.Add(new List<double>());
            networkState.Add(new List<double>());

            return networkState;
        }

        // Activation function
        public double TanH(double x)
        {
            if (x < -45.0) return -1.0f;
            else if (x > 45.0) return 1.0f;
            else return System.Math.Tanh(x);
        }

        // Takes in list of layer sizes. First is inputs, last is outputs
        // Second and third parameters expose info for genetic algorithm to evolve
        public Network(int[] newLayers, List<Weight> allWeights, List<Bias> allBias)
        {
            this.layers = new List<Layer>();
            // Create the desired number of layers
            for (int i = 0; i < newLayers.Length; i++)
            {
                Layer newLayer = new global::NeuralNetwork.Layer(newLayers[i]);
                this.layers.Add(newLayer);

                // Populate this layer with nodes
                for (int o = 0; o < newLayers[i]; o++)
                {
                    Bias newBias = new Bias(Util.GetRandom() * (2) - 1);

                    Node newNode = new Node(newBias);
                    newLayer.nodes.Add(newNode);
                    allBias.Add(newBias);
                }
                newLayer.nodes.ForEach((node) =>
                {
                    // Input nodes
                    if (i == 0)
                    {
                        node.bias.value = 0;
                    }
                    // All other nodes
                    else
                    {
                        // Assign random weights
                        for (int c = 0; c < newLayers[i - 1]; c++)
                        {
                            Weight newWeight = new Weight(Util.GetRandom() * (2) - 1);
                            node.weights.Add(newWeight);
                            allWeights.Add(newWeight);
                        }
                    }
                });
            }
        }

        // This variation is accessed by ProbeControl, calls internal one
        public float[] update(float[] inputs)
        {
            List<double> inputList = new List<double>(inputs.Length);
            for (int i = 0; i < inputs.Length; i++)
            {
                inputList.Add((double)inputs[i]);
            }
            return this.update(inputList);
        }

        // Takes in list of values for input layer.
        // Runs the network to produce an output
        private float[] update(List<double> input)
        {
            // Ensure inputs are correct
            if (input.Count != this.layers[0].nodeCount)
            {
                Debug.Log("Incorrect number of inputs to network");
                return null;
            }
            // Iterate through each layer
            for (int i = 0; i < layers.Count; i++)
            {
                Layer layer = layers[i];

                // Iterate through each node in the layer
                for (int o = 0; o < layer.nodes.Count; o++)
                {
                    Node node = layer.nodes[o];

                    // Input layer?
                    if (i == 0)
                    {
                        node.value = input[o];
                    }
                    else
                    {
                        node.value = 0;
                        for (int c = 0; c < this.layers[i - 1].nodes.Count; c++)
                        {
                            node.value = node.value + this.layers[i - 1].nodes[c].value * node.weights[c].value;
                            node.value = TanH(node.value + node.bias.value);
                        }
                    }
                }
            }
            // Prepare the output values to be returned
            Layer lastLayer = this.layers[this.layers.Count - 1];
            float[] output = new float[lastLayer.nodes.Count];
            for (int v = 0; v < lastLayer.nodes.Count; v++)
            {
                output[v] = (float) lastLayer.nodes[v].value;
            }
            return output;
        }
    }

	void Start () { }
	
	void Update () { }
}
