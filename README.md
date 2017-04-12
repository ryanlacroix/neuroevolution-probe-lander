NeuroEvolved Probe Lander
===========================
Uses a genetic algorithm to evolve the weights and biases of a neural network. The network controls a probe as it comes in for its final stage of landing. The genetic algorithm represents each probe's genome using lists of doubles. Neural networks are constructed at the beginning of each generation based on crossover and mutations of the previous generation's genomes. Rather than having full control over every actuator on the probe (which proved to have too steep a learning curve for the network to grasp), networks have access to different pre-defined maneuvres. These include firing main retro thrusters, turning toward the target landing location, turning away from target landing location, turning 90 degrees above the target (for cruising), and aiming main thrusters towards the ground. Any of the actions can be carried out at the same time to different degrees, giving the network the ability to make new maneuvres by combining predefined ones. Each maneuvre can be carried out at any strength the network wishes.

The software is fully ready for use and runs well. Training, however, is a whole different matter. Because the training is not based on any predefined data and control is continuous, the network's performance is rated based on fitness functions which attempt to sum up the *value* of a probe's performance on a given flight. These fitness functions are still a work in progress as they are tricky to get just right. More information can be found in the Learning Tasks section.

This project is written in C# for the Unity engine, however the neuroevolution-specific scripts should be easily portable to any environment.

Learning tasks
--------------
The network learns best training it one task at a time, each building upon the last. Each task is learned using a fitness algorithm based on the probe's flight and landing (or impact), and as such it is important not only to change this algorithm to reflect the new task, but also to some degree factor in the performance on the previous task. This ensures that the network does not unlearn it's previous behaviours. Evolving the network using an evolutionary algorithm seems (at least in my experience) to be somewhat better at this than the traditional back-propogation technique, which is known for quickly forgetting previously learned tasks. The program can be stopped at any time to create new fitness functions, as the state of the population is saved at the end of every generation. This allows each new task to be built on the genome of the previous population.

Default configuration
---------------------
+ __Genetic Algorithm__
  * The genetic algorithm has a default of 50 individuals, mutation rate of 10%, corssover rate of 95% and elitism count of 10. After a fair bit of trial and error I have found these values to evolve the probes most effectively.
  * These values can be modified in the `GenAlgo` constructor found in the `Start()` function of `GeneticAlgorithm.cs`

+ __Neural Network__
  * Neural networks are initiliazed with one hidden layer of 10 nodes, 2 inputs and 4 outputs. Different configurations have not yet been tested properly, and so this configuration is likely unoptimal. Any size network can be achieved by changing the `Network` constructor parameters within the `Individual` constructor implementations found within the `GeneticAlgorithm.cs` file. `Network` structure is defined using a list of `int`s, where the first and last values are the input and output layers respectively.

Note
----
Runs in the Unity 5 environment. For the sake of space, this repository does not include any assets other than the scripts. The object/asset structure within the project is not particularly complicated and can easily be rebuilt by following the code and using primitives as object instances.