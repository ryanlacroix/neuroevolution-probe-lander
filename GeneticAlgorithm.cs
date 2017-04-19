using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GeneticAlgorithm : MonoBehaviour {
    public class GenAlgo
    {
        public int popSize;
        public double mutationRate;
        public double crossoverRate;
        public int elitismCount;
        private System.Random rand;
        private float fitnessThreshold1 = 0.4f;

        public GenAlgo(int size, double mut, double cross, int elite)
        {
            this.popSize = size;
            this.mutationRate = mut;
            this.crossoverRate = cross;
            this.elitismCount = elite;
            rand = new System.Random();
        }

        public Population makePopulation()
        {
            Population newPop = new Population(this.popSize);
            return newPop;
        }

        // Call this at the end of each trial
        public double calcFitness(Individual ind, GameObject pr, double populationFitness)
        {
            ProbeInstr instruments = pr.GetComponent<ProbeInstr>();
            double totalScoreVel;
            double totalScoreDist;
            totalScoreVel = instruments.impactVelocity;

            totalScoreDist = instruments.getDistanceToTarget();

            // Remove better than perfect scores -> within 10m is considered perfect
            // Encourage probes to focus on impact speed once target is found
            if (totalScoreDist < 20)
                totalScoreDist = 20;

            // Put a larger emphasis on lowering impact velocity
            // This should even it out a little better
            return System.Math.Min(((1 / totalScoreVel)*10 + (1 / totalScoreDist)*6), 1);
        }

        public void evalPopulation(Population pop)
        {
            double totalFitness = 0;
            double bestFitness = 0;
            for (int i = 0; i < pop.population.Count; i++)
            {
                totalFitness += pop.population[i].fitness;
                if (pop.population[i].fitness > bestFitness)
                    bestFitness = pop.population[i].fitness;
            }
                
            pop.popFitness = totalFitness;
            Debug.Log("Overall population fitness: " + pop.popFitness);
            Debug.Log("Best probe: " + bestFitness);
        }

        // Mutate the population based on random probabilities
        public Population mutatePopulation(Population pop)
        {
            pop.sortPopulation();
            // Iterate through individuals
            for (int i = 0; i < pop.popSize; i++)
            {
                Individual p1 = pop.population[i];

                // Determine if this individual should mutate
                if (i > elitismCount && this.rand.NextDouble() < mutationRate)
                {
                    // What percentage of the genes should mutate
                    double mutateAmount = this.rand.NextDouble();
                    // Build genome of child
                    List<NeuralNetwork.Weight> childWeights = new List<NeuralNetwork.Weight>();
                    List<NeuralNetwork.Bias> childBiases = new List<NeuralNetwork.Bias>();

                    for (int o = 0; o < p1.weights.Count; o++)
                    { // Build weights
                        if (rand.NextDouble() > mutateAmount)
                            childWeights.Add(new NeuralNetwork.Weight(NeuralNetwork.Util.GetRandom() * (2) - 1));
                        else
                            childWeights.Add(p1.weights[o]);
                    }
                    for (int o = 0; o < p1.biases.Count; o++)
                    { // Build biases
                        if (rand.NextDouble() > mutateAmount)
                            childBiases.Add(new NeuralNetwork.Bias(NeuralNetwork.Util.GetRandom() * (2) - 1));
                        else
                            childBiases.Add(p1.biases[o]);
                    }

                    pop.population[i] = new Individual(childWeights, childBiases);
                }
            }
            return pop;
        }

        // Selects a parent giving priority to fittest.
        private Individual selectParent(Population pop)
        {
            double popFitness = pop.popFitness;
            double rouletteWheel = (this.rand.NextDouble()) * popFitness;

            double spinWheel = 0;
            for (int i = 0; i < pop.popSize; i++)
            {
                spinWheel += pop.population[i].fitness;
                if (spinWheel > rouletteWheel)
                {
                    return pop.population[i];
                }
            }
            // In the event not a single one is selected
            return pop.population[pop.population.Count - 1];
        }

        public Population crossoverPopulation(Population pop)
        {
            // Should probably instead just call this in eval
            pop.sortPopulation();
            for (int i = 0; i < pop.popSize; i++)
            {
                Individual p1 = pop.population[i];

                // Determine if this individual should breed
                if (this.crossoverRate > this.rand.NextDouble() && i > elitismCount)
                {
                    // Build weights and biases for use in a new child
                    Individual p2;
                    p2 = selectParent(pop);
                    // Build genome of child
                    List<NeuralNetwork.Weight> childWeights = new List<NeuralNetwork.Weight>();
                    List<NeuralNetwork.Bias> childBiases = new List<NeuralNetwork.Bias>();
                    
                    for (int o = 0; o < p1.weights.Count; o++)
                    { // Build weights
                        if (rand.NextDouble() > 0.5)
                            childWeights.Add(p1.weights[o]);
                        else
                            childWeights.Add(p2.weights[o]);
                    }
                    for (int o = 0; o < p1.biases.Count; o++)
                    { // Build biases
                        if (rand.NextDouble() > 0.5)
                            childBiases.Add(p1.biases[o]);
                        else
                            childBiases.Add(p2.biases[o]);
                    }

                    pop.population[i] = new Individual(childWeights, childBiases);
                }
            }
            return pop;
        }
    }

    public class Population
    {
        public List<Individual> population;
        public double popFitness;
        public int popSize;
        public int generation;

        // For XmlSerialize use only
        public Population() { }

        public Population(int size)
        {
            this.population = new List<Individual>();
            // Fill the new population with random individuals
            for (int i = 0; i < size; i++)
                this.population.Add(new global::GeneticAlgorithm.Individual());
            this.popSize = size;
            this.popFitness = 0; // Determined by GenAlgo
            this.generation = 0;
        }

        // Return the fittest individual in this population
        // Possibly redundant now that sortPopulation() implememnted
        public int getFittest()
        {
            Individual fittest = this.population[0];
            int fittestIndex = 0;
            for (int i = 0; i < this.population.Count; i++)
            {
                if (this.population[i].fitness > fittest.fitness)
                {
                    fittest = this.population[i];
                    fittestIndex = i;
                }
            }
            return fittestIndex;
        }

        // Call this after evauating fitness
        // Sorts population based on fitness, best being at index 0
        // Not too concerned about runtime. This only happens once per generation
        public void sortPopulation()
        {
            Individual temp;
            for (int i = 0; i < this.popSize; i++)
            {
                for (int j = 0; j < this.popSize - 1; j++)
                {
                    if (this.population[j].fitness < this.population[j + 1].fitness)
                    {
                        temp = this.population[j];
                        this.population[j] = this.population[j + 1];
                        this.population[j + 1] = temp;
                    }
                }
            }
        }
    }

    public class Individual
    {
        public NeuralNetwork.Network network; 
        public List<NeuralNetwork.Weight> weights;
        public List<NeuralNetwork.Bias> biases;
        public double fitness;

        // Constructor for initial population
        public Individual()
        {
            weights = new List<NeuralNetwork.Weight>();
            biases = new List<NeuralNetwork.Bias>();
            this.network = new NeuralNetwork.Network(new int[] { 2, 10, 5 }, weights, biases);
            this.fitness = 0; // Must be set after evaluation
        }

        // Constructor for evolved populations
        public Individual(List<NeuralNetwork.Weight> w, List<NeuralNetwork.Bias> b)
        {
            weights = new List<NeuralNetwork.Weight>();
            biases = new List<NeuralNetwork.Bias>();
            this.network = new NeuralNetwork.Network(new int[] { 2, 10, 5 }, weights, biases);
            // Replace weights and biases with evolved ones
            for (int i = 0; i < weights.Count; i++)
                weights[i] = w[i];
            for (int i = 0; i < biases.Count; i++)
                biases[i] = b[i];
            this.fitness = 0; // Must be set after evaluation
        }
    }

    public GenAlgo gen;
    public Population pop;
    public GameObject probePrefab;
    public int generation;
    public int currInd;
    public bool running;
    private List<GameObject> probes;
    public Camera mainCam;
    private int fittestIndex;
    private StateSaver<Population> stateSaver;
    public float fitnessThreshold1;

	// Use this for initialization
	void Start () {
        stateSaver = new StateSaver<Population>(@"population.xml");
        // Create initial population
        gen = new global::GeneticAlgorithm.GenAlgo(50, 0.05, 0.95, 5);

        // Load last evolved population if one exists
        if (stateSaver.hasPriorSave())
            pop = stateSaver.load();
        else
            pop = gen.makePopulation();

        generation = 0;
        running = false;
        currInd = 0;
        probes = new List<GameObject>();
        fittestIndex = 0;
	}

    // Update is called once per frame
    void Update()
    {
        // Check if generaton has ended
        if (!running)
        {
            pop = gen.crossoverPopulation(pop);

            // Populate list with new generation
            for (int i = 0; i < pop.popSize; i++)
            {
                GameObject newProbe = (GameObject)Instantiate(probePrefab);
                ProbeControl pControl = newProbe.GetComponent<ProbeControl>();
                pControl.network = pop.population[i].network;
                probes.Add(newProbe);
            }
            Debug.Log("Completed generation " + pop.generation);
            generation++;
            pop.generation++;
            running = true;
            // Set camera to follow best individual in population
            mainCam.GetComponent<CameraFollow>().probe = probes[fittestIndex];
        }
        else // Generation is currently running
        {
            // Determine if all probes have landed
            bool allProbesLanded = true;
            for (int j = 0; j < pop.popSize; j++)
            {
                if (probes[j].transform.position.y > 3)
                {
                    allProbesLanded = false;
                    break;
                }
            }
            if (allProbesLanded == true)
            {
                // Save the population genomes to XML file
                stateSaver.save(pop);

                // Calculate generation fitness and destroy probes
                running = false;
                for (int i = 0; i < pop.popSize; i++)
                {
                    pop.population[i].fitness = gen.calcFitness(pop.population[i],
                        probes[i], pop.popFitness);
                    Destroy(probes[i]);
                }
                probes.Clear();
                gen.evalPopulation(pop);
                fittestIndex = pop.getFittest();
            }
                
        }
    }
}