using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;


	// TODO : find a better approximation of the speed of the lemming
	// TODO : find the holes and save them on the map
	// TODO : implements the holes of the map in the chromosome
	// TODO : "play again"  button / "Save chromosome button" / "Replay chromosome" button	
//TODO : if a wall or the end door is out the map, take that into account



// Main class for the genetic algorithm
public class AI : MonoBehaviour {
	
	// Parameters of the algorithms ; Chromosome parameters can be found in the Chromosome class
	
	// Number of chromosome in a population
	private int nbMemberMax = 100;
	
	// Maximum number of populations
	private int nbPopulationMax = 400;
	
	// Number of "best chromosomes" we keep when we evolve from one population to another
	private int elitism = 20;
	
	
	// Current population
	private List<Chromosome> members;
	
	// New population
	private List<Chromosome> newMembers;
	
	// Current index of population
	private int nbPopulation = 0;
	
	// If true, a solution has been found
	private bool isFinished = false;
	
	// The best current chromosome
	public static Chromosome bestChrom;
	
	// Total of the fitness values of the current population
	private double totalFitness;
	
	// Fitness value of the goal
	public static int fitnessGoal;
	
	// Content logged at the end of the algorithm
	private string logContent;
	
	
	// Simulation of the map
	public static int[,] map;
	
	// X max on the simulated map
	public static int xMax;
	
	// Z max on the simulated map
	public static int zMax;
	
	// X coordonate of the start point in the simulated-map
	public static int xStart;

	// Z coordonate of the start point in the simulated-map
	public static int zStart;

	// X min coordonate of the end point in the simulated-map
	public static int xMinEnd;

	// X max coordonate of the end point in the simulated-map
	public static int xCenterEnd;
	
	// X max coordonate of the end point in the simulated-map
	public static int xMaxEnd;

	// Z min coordonate of the end point in the simulated-map
	public static int zMinEnd;

	// Z min coordonate of the end point in the simulated-map
	public static int zCenterEnd;
	
	// Z max coordonate of the end point in the simulated-map
	public static int zMaxEnd;
	
	// Half size of the lemming collider in the simulated-map
	public static int lemmingHalfSize;
	
	// Size of the lemming collider in the simulated-map
	public static int lemmingSize;
	
	// Size of a bridge in the simulated-map
	public static int bridgeSize;
	
	
	public void Start()
	{
		InitializeSimulatedMap();
		
		// If we want the AI to start automatically, we can decomment this
	//	BeginAI();
	}
	
	
	

	// Initialize the simulated map, the start coordonates and the end coordonates
	private void InitializeSimulatedMap()
	{		
		Debug.Log("Simulated Map : Initialization complete.");
		Vector3 terrainSize = Terrain.activeTerrain.terrainData.size;		
				
		// We calculate the distance walked by a lemming in the given interval
		float distanceWalkedByTick = Lemming.WALK_SPEED * RunLemmingChromosome.interval;
		
		// I don't know why, but there is an slight difference between the simulated speed and the real speed
//		distanceWalkedByTick *=< 1.19f;
//		distanceWalkedByTick /= 0.83f;
		
		float normalizedRatio = 1f / distanceWalkedByTick;
		
		// The maximum coordonates of the simulated map
		xMax = (int) (normalizedRatio * terrainSize.x);
		zMax = (int) (normalizedRatio * terrainSize.z);

		
		// we create the simulated map
		map = new int[xMax,zMax];
		for(int x=0; x < xMax;x++)
		{
			for(int z = 0; z < zMax;z++)
			{
				map[x,z] = 0;		
			}
		}
		
		//we add the blocks on the simulated map
		GameObject[] blocs = GameObject.FindGameObjectsWithTag("block");
		float xSizeBloc;
		float zSizeBloc;
		int xMinBloc;
		int xMaxBloc;
		int zMinBloc;
		int zMaxBloc;
		
		foreach(GameObject bloc in blocs)
		{
			// We calculate the coordonates of the blocs in the simulated map
			xSizeBloc = bloc.collider.bounds.size.x * normalizedRatio;
			zSizeBloc = bloc.collider.bounds.size.z * normalizedRatio;
			
			xMinBloc = (int) (bloc.transform.position.x * normalizedRatio - (xSizeBloc / 2));
			xMaxBloc = (int) (bloc.transform.position.x * normalizedRatio + (xSizeBloc / 2));
			
			zMinBloc = (int) (bloc.transform.position.z * normalizedRatio - (zSizeBloc / 2));
			zMaxBloc = (int) (bloc.transform.position.z * normalizedRatio + (zSizeBloc / 2));
			
			// We add the end door on the simulated map
			for(int x=xMinBloc; x <= xMaxBloc;x++)
			{
				for(int z = zMinBloc; z <= zMaxBloc;z++)
				{
					map[x,z] = 1; // 1 == bloc
				}
			}
		}
		

		// We calculate the position of the start door on the simulated map
		GameObject start = (GameObject) GameObject.Find("start");
		xStart = Mathf.RoundToInt(start.transform.position.x * normalizedRatio);
		zStart = Mathf.RoundToInt(start.transform.position.z * normalizedRatio);
		
		
		// We calculate the coordonates of the end door on the simulated map
		GameObject end = GameObject.FindGameObjectWithTag("end");
		
		float xSizeEnd = end.collider.bounds.size.x * normalizedRatio;
		float zSizeEnd = end.collider.bounds.size.z * normalizedRatio;
		
		xMinEnd = (int) (end.transform.position.x * normalizedRatio - (xSizeEnd / 2));
		xMaxEnd = (int) (end.transform.position.x * normalizedRatio + (xSizeEnd / 2));
		xCenterEnd = (xMinEnd+xMaxEnd)/2;
		
		zMinEnd = (int) (end.transform.position.z * normalizedRatio - (zSizeEnd / 2));
		zMaxEnd = (int) (end.transform.position.z * normalizedRatio + (zSizeEnd / 2));
		zCenterEnd = (zMinEnd+zMaxEnd)/2;		
		
		// We add the end door on the simulated map
		for(int x=xMinEnd; x <= xMaxEnd;x++)
		{
			for(int z = zMinEnd; z <= zMaxEnd;z++)
			{
				map[x,z] = 9; // 9 == end
			}
		}
		
		// The simulated lemming size
		// the lemming size == 1 in the "normal" map
		// TODO : calculate the real lemming size... just in case the "1" value change one day
		lemmingSize = (int)(normalizedRatio);
		lemmingHalfSize = (int)(lemmingSize/2);
		
		// Largest possible distance on the map : the diagonal of the map
		fitnessGoal = (int)Mathf.Sqrt(xMax*xMax + zMax*zMax);
		
		
		// The bridge simulated size
		bridgeSize = (int) (5f * Lemming.BRICK_Z * normalizedRatio);		
	}
	
	
	
	
	
	// The initial call to launch the genetic algorithm
	public void BeginAI () {
		Debug.Log("begin AI");
		logContent = "Start...";
		
		// Initilizing the population
		members = new List<Chromosome>();
		
		// First population == random
		for(int i=0;i<nbMemberMax;i++)
		{
			Chromosome chrom = new Chromosome();
			chrom.GenerateRandomGeneList();
			members.Add(chrom);
		}
		
		newMembers = new List<Chromosome>();
				
		isFinished = false;
		nbPopulation = 0;		
				
		// Main loop of the genetic algorithm
		while (nbPopulation < nbPopulationMax && isFinished == false)
		{
			// Evaluate the fitness f(x) of each chromosome x in the population	
			FitnessPopulation();
			
			// Sort the population by fitness value
			SortPopulationByFitness();

			// Log the result
			LogCurrentPop();
			
			//  Create a new population by repeating following steps until the new population is complete
			NewPopulation();
			
			// Use new generated population for a further run of algorithm
			ReplacePopulation();
			
			// If the end condition is satisfied, stop, and return the best solution in current population
//			TestPopulation();
			nbPopulation++;
		}	
		
		
//		int fitnessPrecision = (int) (AI.fitnessGoal * 0.01f);
//		if (bestChrom.GetFitness() >= (AI.fitnessGoal-fitnessPrecision))

//		When we found a solution, we run it on the game
		if (isFinished)
		{
			RunLemmingChromosome.Run(AI.bestChrom);
			isFinished = true;
			Debug.Log("Solution found! Pop "+nbPopulation+" * "+bestChrom.ToString()+" * mutation Rate = "+Chromosome.GetMutationRate()+" - crossoverRate = "+Chromosome.GetCrossOverRate());
		}
		else
		{
			Debug.Log("No solution found");	
		}

		bestChrom.PrintMap();
		
		// We write the log in a file -- Warning : only do this at the end, not during the algorithm ! 
		// Otherwise, it really slows down everything by A LOT !
		TextWriter tw = new StreamWriter("IA_log.txt");

		// write a line of text to the file
        tw.Write(logContent);

        // close the stream
        tw.Close();
	}
	
		
	
	
	
	
	// Generate random population of n chromosomes (suitable solutions for the problem)
	public void GenerateRandomPopulation()
	{
		foreach(Chromosome chrom in members)
		{
			chrom.GenerateRandomGeneList();
		}
	}
	
	
	
	
	// Evaluate the fitness f(x) of each chromosome x in the population	
	public void FitnessPopulation()
	{
		totalFitness = 0;
		int currentFitness;

		int max = -1;
		int min = -1;
		
		// for each chromosome
		foreach(Chromosome chrom in members)
		{
			currentFitness = chrom.Fitness();

			// Total fitness of the current population
			totalFitness += currentFitness;
			
			if (max < currentFitness || max == -1)
				max = currentFitness;
			
			if (min > currentFitness || min == -1)
				min = currentFitness;
			
			if (null == bestChrom || currentFitness > bestChrom.GetFitness())
				bestChrom = chrom;
			
			//TODO : delete this part when the evaluation function will take the number of lemmings
			// and the time as part of the evaluation
			// Right now, it is only about the lemmings arrive to the end or not
			if (chrom.GetSuccess())
			{
				isFinished = true;
				bestChrom = chrom;
			}
		}
		
//		Debug.Log("Total fitness = "+totalFitness+" -- Max = "+max+" -- Min = "+min);
	}
	
	
	// Simple log function
	public void Log(string s)
	{
		logContent += s + "\n";
	}
	
	
	// Log the current population
	private void LogCurrentPop()
	{
		// Log the best chromosomes of the current population
		Log("POPULATION Num "+nbPopulation);
		for (int i = (nbMemberMax-elitism); i < nbMemberMax ; i++)
//		for (int i = 0; i < nbMemberMax ; i++)
		{
			Log("chrom "+i+" * score "+members[i].GetFitness()+" ** "+members[i].ToString());
		}
		Log("\n");
	}
	
	
	
	// Sort the population by fitness asc
	private void SortPopulationByFitness()
	{
		members.Sort(delegate(Chromosome c1, Chromosome c2) { 
			return c1.GetFitness().CompareTo(c2.GetFitness()); 
		});
				
//		Debug.Log(members[nbMemberMax-1].GetFitness());
	}
	
	
	
	//  Create a new population by repeating following steps until the new population is complete
	public void NewPopulation()
	{		
		// For each chromosome not in the "elitism" part of the population
		for(int i=0; i<(nbMemberMax - elitism)/2; i++)
		{		
			// Select two parent chromosomes from a population according to their fitness (the better fitness, the bigger chance to be selected)
			int pidx1 = -1;
			int pidx2 = -1;
			
			// The do / while loop could be deleted if needed
			// It is here to avoid doing a crossover with the same chromosome twice
			do {
				pidx1 = RouletteSelection();
				pidx2 = RouletteSelection();	
			} while (pidx1 == pidx2);
			
			Chromosome chrom1 = members[pidx1];
			Chromosome chrom2 = members[pidx2];
			
			Chromosome child1 = null;
			Chromosome child2 = null;
			
			// With a crossover probability cross over the parents to form a new offspring (children). 
			// If no crossover was performed, offspring is an exact copy of parents.
			Chromosome.Crossover(chrom1,chrom2, out child1, out child2);
			
			// With a mutation probability mutate new offspring at each locus (position in chromosome).
			child1.Mutation();
			child2.Mutation();
			
			// Place new offspring in a new population
			Accepting(child1);
			Accepting(child2);
		}
	}
	
	
	
	// Selection of a chromosome with the roulette selection
	/*
		1. [Sum] Calculate sum of all chromosome fitnesses in population - sum S.
		2. [Select] Generate random number from interval (0,S) - r.
		3. [Loop] Go through the population and sum fitnesses from 0 - sum s. When the sum s is greater than r, stop and return the chromosome where you are.

		Of course, step 1 is performed only once for each population.
	 */
	// Note : there are several other types of selection ; I don't really know if the roulette selection is the best choice here or not
	public int RouletteSelection()
	{
		double randomFitness = Random.Range(0f,(float)totalFitness);
		int currentTotalFitness = 0;
			
		for (int i = 0;i < nbMemberMax;i++)
		{
			currentTotalFitness += members[i].GetFitness();
			
			if (currentTotalFitness >= randomFitness)
			{
				return i;
			}
		}
		
		Debug.Log("ERROR ROULETTE ! - Debug : "+currentTotalFitness +" <= "+randomFitness+ " - totalfitness = "+totalFitness);
		return -1;
	}
	
	
	
	// Place new offspring in a new population
	public void Accepting(Chromosome chrom)
	{
		
		// We compare the genes times, and if 2 times are too close from each other, we remove the chromosome
		// The reason is the simulation is not precise enough when it deals with short distances
		int previousTime = 1;
		foreach(Gene g in chrom.geneList)
		{
			if (g.GetGeneTime() - previousTime < 5 )
			{
				return;	
			}
			previousTime = g.GetGeneTime();
		}
		newMembers.Add(chrom);
	}
	
	
	
	// Use new generated population for a further run of algorithm
	public void ReplacePopulation()
	{
		for (int i = 0; i < newMembers.Count ; i++)
		{
			members[i] = newMembers[i];
		}
		newMembers.RemoveRange(0,newMembers.Count);
	}
	
	
	
	// Graphical interface
	void OnGUI()
	{

		// Relaunch the genetic algorithm when clicking on the button	
        if (GUI.Button(new Rect(700, 10, 100, 30), "Start AI"))
		{	
			StartDoor start = GetComponent<StartDoor>();
			start.Reset();
			
			RunLemmingChromosome.ResetRun();
			BeginAI();
		}
		
		// Allow to relaunch the current chromosome
		if (isFinished)
		{
			if (GUI.Button(new Rect(630, 50, 170, 30), "Relaunch this chromosome"))
			{	
				RunLemmingChromosome.Run(AI.bestChrom);
				StartDoor start = GetComponent<StartDoor>();
				start.Reset();
			}
		}
		
		Gene.hasBlock = GUI.Toggle(new Rect(520, 10, 170, 30), Gene.hasBlock, "Block");
		Gene.hasBridge = GUI.Toggle(new Rect(520, 50, 170, 30), Gene.hasBridge, "Bridge");
		
		if (GUI.Button(new Rect(10, Screen.height - 40, 100, 30), "Menu"))
		{	
			Application.LoadLevel("AI_menu");
		}
			
		if (isFinished)
		{
			GUI.Box(new Rect(120, Screen.height - 80, 350, 30), "Best chromosome found in population "+ nbPopulation );
			GUI.Box(new Rect(120, Screen.height - 40, Screen.width - 130, 30), bestChrom.ToString());
		}
	}
}
