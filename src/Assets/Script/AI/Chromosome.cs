using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class Chromosome{

	// The mutation rate
	protected static float mutationRate = 0.6f;
	
	// The rate of a deletion of a gene : if there is a mutation, there is a chance that the gene will be deleted
	protected static float mutationDeleteGeneRate = 0.35f;
	
	// The rate of a creation of a gene : if there is a mutation, there is a chance that a new random gene will be created
	protected static float mutationCreateGeneRate = 0.1f;
	
	// The crossover rate
	protected static float crossoverRate = 0.7f;
		
	// The minimum number of gene
	protected static int minChromSize = 1;
	
	// The maximum number of gene
	protected static int maxChromSize = -1;	
	
	
	
	// The genes list of the chromosome
	public List<Gene> geneList;
	
	// The fitness of the chromosome
	// Default is 0
	protected int fitness;
	
	// The size (the number of gene) of the chromosome
	protected int size;
	
	// The current map
	private int[,] map;

	// The simulated lemming
	private SimulatedLemming lem;

	// If the current chromosome is wrong (the lemmings died, etc..), we stop the evaluation of the chromosome
	private bool stop = false;

	// If a lemming succeeded (arrived to the end)
	private bool success = false;
	
	
	
	// Constructor
	public Chromosome()
	{
		// if the maximum number of genes is not defined, it will be the number of lemmings
		if (maxChromSize <= 0)
		{
			maxChromSize = 	StartDoor.numberOfLemmings;
		}
		
		// Random number of gene for the current chromosome
		size = Random.Range(1,maxChromSize);
		
		fitness = 0;
		geneList = new List<Gene>();
		for (int i=0; i < size; i++)
		{
			Gene gene = new Gene();
			geneList.Add(gene);
		}
	}
	
		
	// Generate random chromosome
	public void GenerateRandomGeneList()
	{
		foreach(Gene gene in geneList)
		{
			gene.GenerateRandom();
		}
		
		// We sort the genes by time ASC
		geneList.Sort(new GeneComparer());
	}
	
	
	
	// With a crossover probability cross over the parents to form a new offspring (children). 
	// If no crossover was performed, offspring is an exact copy of parents.
	public static void Crossover(Chromosome chrom1, Chromosome chrom2,out Chromosome child1, out Chromosome child2)
	{				
		double crossoverProbability = Random.Range(0f,1f);
		
		// No crossover
		if (crossoverProbability > Chromosome.crossoverRate)
		{
			child1 = chrom1.Clone();
			child2 = chrom2.Clone();
			return;
		}
		
		// Crossover
		child1 = new Chromosome();
		child2 = new Chromosome();
						
		// We set the size of the new chromosomes
		int chrom1Size = chrom1.GetChromosomeSize();
		int chrom2Size = chrom2.GetChromosomeSize();
		child1.SetChromosomeSize(chrom2Size);
		child2.SetChromosomeSize(chrom1Size);
		
		int lengthDifference = chrom1Size - chrom2Size;
		
		int maxRange = chrom1Size;
		int minRange = maxRange;
		if (maxRange < chrom2Size)
		{
			maxRange = chrom2Size;
		}
		else
		{
			minRange = chrom2Size;
		}
		
		// We do the crossover operation
		int pos = Random.Range(0,minRange-1);
		for(int i = 0 ; i < minRange ; i++)
		{
			if (i < pos)
			{
				child1.geneList[i] = (new Gene(chrom1.geneList[i]));
				child2.geneList[i] = (new Gene(chrom2.geneList[i]));
			}
			else
			{
				child1.geneList[i] = (new Gene(chrom2.geneList[i]));
				child2.geneList[i] = (new Gene(chrom1.geneList[i]));
			}
		}
		
		
		// If the parents chromosomes did not have the same length, we add the remaining genes
		// Chrom1.length > chrom2.length
		if (lengthDifference > 0)
		{
			for(int i = minRange; i < chrom1Size; i++)
			{
				child2.geneList[i] = (new Gene(chrom1.geneList[i]));
			}
		}
		// Chrom1.length < chrom2.length
		else if (lengthDifference < 0)
		{
			for(int i = minRange; i < chrom2Size; i++)
			{
				child1.geneList[i] = (new Gene(chrom2.geneList[i]));
			}
		}
		
		// We sort the new chromosomes by time
		child1.GetListGene().Sort(new GeneComparer());
		child2.GetListGene().Sort(new GeneComparer());		
			
//		Debug.Log("************ Start Cross");
//		Debug.Log("Parent1 :"+chrom1);
//		Debug.Log("Parent2 :"+chrom2);
//		Debug.Log("Child1 :"+child1);
//		Debug.Log("Child2 :"+child2);		
//		Debug.Log("************ End Cross *********");
	}

	

	
	// Random mutation of a gene
	public void Mutation()
	{
//		Debug.Log("Before mutation:"+this+ " ** size:"+size);
		
		
		float mutationProbability = Random.Range(0f,1f);
		
		// There is no mutation, we stop here
		if (mutationProbability > Chromosome.mutationRate)
			return;
		
		// There is a mutation
		
		// We chose a random gene to mutate
		int randomPosition = Random.Range(0,size-1);
		
		// Probability to delete or to create a gene
		float mutationCreateOrDeleteGene = Random.Range(0f,1f);
		
		// Delete of a random gene
		// Note : we can't delete a gene if the gene is the only one of the chromosome
		if (size > 1 && mutationCreateOrDeleteGene < Chromosome.mutationDeleteGeneRate)
		{
			geneList.RemoveAt(randomPosition);
			
			// Don't forget to decrease the size of the chromosome
			size--;
		}
		// The "basic" mutation of a gene
		else
		{
			geneList[randomPosition].Mutation();
		}
		
		
		// Creation of a new random gene
		// Note : we can't create a new gene if the chromosome has already its maximum size
		if (size < (StartDoor.numberOfLemmings - 1) && mutationCreateOrDeleteGene < (1 - Chromosome.mutationCreateGeneRate))
		{
			Gene newGene = new Gene();
			newGene.GenerateRandom();
			geneList.Add(newGene);
			size++;
		}
			
		// We sort the chromosome by time
		geneList.Sort(new GeneComparer());
		
//		Debug.Log("After mutation:"+this);
	}

	
	
	
	
	
	// compute the fitness of the current chromosome
	// It is the most important function of the genetic algorithm : it is here we will know if a chromosome is good or bad for our problem
	public int Fitness()
	{		
		fitness = 0;
		
		// We clone the simulated map for the chromosome : we will use it here to evaluate the result of the chromosome on the map
		map = (int[,])AI.map.Clone();
	
		// The shortest distance between a lemming and the end door
		// By default, it's the start / end distance
		int shortestDistance = (int)Mathf.Sqrt(Mathf.Pow(AI.xStart - AI.xCenterEnd,2) + Mathf.Pow(AI.zStart - AI.zCenterEnd,2));
		int currentDistance = shortestDistance;
		
		// Interval between 2 lemmings
		int intervalLemmingCreation = (int) (StartDoor.speedProduct / RunLemmingChromosome.interval);
							
		
		// Foreach lemming, we simulate their path on the map
		for (int i = 0; i < StartDoor.numberOfLemmings && !success && !stop; i++)
		{			
			// For the 1st lemming, the 1st step is 0
			// For the 2nd lemming, the 1st step is "intervalLemmingCreation"
			// For the 2nd lemming, the 1st step is "intervalLemmingCreation" * 2
			// etc
			// Note : there is a small delay at the beginning, between the simulation and the reality, so we add +4
			int currentStep = 2 + intervalLemmingCreation * i;
			
			
			// We create the lemming we will move on the map
			lem = new SimulatedLemming(AI.xStart,AI.zStart,0);
			
			
			// For each step, if no lemming found the end door
			while(currentStep < Gene.maxTime && !success)
			{		
				
				// We move the lemming
				// 0 = up
				// 1 = right
				// 2 = down
				// 3 = left
				MoveLemming(lem);
				
				
				// If a lemming died,  chances are all the other will die too.
				// We stop the loop here, this chromosome is useless
				if (stop)
				{
//					Debug.Log("Stop : lemmings died : step num "+currentStep);
					break;
				}		
				
				
				// test if the current cell contains a blocker
				// or any cell in the lemming collider distance
				MeetABlocker(lem);
							
				
				// if the lemming find the end door, it is a success
				if (   lem.x > AI.xMinEnd && lem.x < AI.xMaxEnd 
				    && lem.z > AI.zMinEnd && lem.z < AI.zMaxEnd)
				{
					success = true;
				}
				
				currentDistance = (int)Mathf.Sqrt(Mathf.Pow(lem.x - AI.xCenterEnd,2) + Mathf.Pow(lem.z - AI.zCenterEnd,2));
				
				if (shortestDistance > currentDistance)
				{
					shortestDistance = currentDistance;
				}
				
				
				// if the current step == the gene time, we execute the gene's action
				if (i < size && geneList[i].GetGeneTime() == currentStep)
				{
					// If the action is a "blocking" action on the lemming (explosion, blocker, etc)
					// then we don't need to continue, the lemming will not move anymore
					int actionReturnCode = ExecuteGeneAction(lem, geneList[i].GetGeneAction());
					if (actionReturnCode == 1)
					{
						currentStep = Gene.maxTime;	
					}
					else if (actionReturnCode == 2)
					{
						stop = true;
						break;
					}
					
				}
				currentStep++;
			}
		}

		// The fitness is the distance between the closest point we found and the end
		fitness = AI.fitnessGoal - shortestDistance;
		
		
//		Debug.Log("Chrom : "+this.ToString()+" = "+shortestDistance);	
	
		// Debug
//		RunLemmingChromosome run = GameObject.FindObjectOfType(typeof(RunLemmingChromosome)) as RunLemmingChromosome;
//		run.chrom = this;
//		run.start = true;

//		PrintMap();	
		
		return fitness;
	}
	
	
	
	// We move the lemming
	// 0 = up
	// 1 = right
	// 2 = down
	// 3 = left	
	private void MoveLemming(SimulatedLemming lem)
	{
		switch(lem.direction)
		{
		case 0:
//			Debug.Log("walk up");
			// If the lemming walk out of the map, he becomes useless : we end it here
			if (lem.z == AI.zMax)
			{
				stop = true;
				break;
			}
			
			lem.z++;
			break;
			
		case 1:
//			Debug.Log("walk right");
			// If the lemming walk out of the map, he becomes useless : we end it here
			if (lem.z == AI.xMax)
			{
				stop = true;
				break;
			}
			
			lem.x++;
			break;
			
		case 2:
//			Debug.Log("walk down");
			// If the lemming walk out of the map, he becomes useless : we end it here
			if (lem.z == 0)
			{
				stop = true;
				break;
			}
			
			lem.z--;
			break;
			
		case 3:
//			Debug.Log("walk left");
			// If the lemming walk out of the map, he becomes useless : we end it here
			if (lem.x == 0)
			{
				stop = true;
				break;
			}
			lem.x--;
			break;
			
		default:
			Debug.Log("Error lemming direction !!");
			break;
		}
	}
	
	
	
	// Test if the lemming met blocker
	// We test only the collider limits in front of the lemming :
	//  - we don't need to test anything "inside" the collider
	//  - we don't need to test the sides or the back of the collider
	private bool MeetABlocker(SimulatedLemming lem)
	{
		return MeetABlocker(lem, 0);
	}
	
	
	// The additionalDistance parameter is used to test if there is a blocker in the additionalDistance in front of the lemming
	// It is used when the lemming build a bridge
	private bool MeetABlocker(SimulatedLemming lem, int additionalDistance)
	{	
		// coordonates of the lemming collider
		int xmin;
		int xmax;
		int zmin;
		int zmax;
		
		// Coordonates of the map
		int xMinMap;
		int xMaxMap;
		int zMinMap;
		int zMaxMap;
		
		switch(lem.direction)
		{
		// UP
		case 0:
			xmin = lem.x - AI.lemmingHalfSize;
			xmax = lem.x + AI.lemmingHalfSize;
			for (int x = xmin; x <= xmax; x++)
			{
				zMaxMap = lem.z+AI.lemmingHalfSize+1+additionalDistance;
				for (int z = lem.z+AI.lemmingHalfSize+1 ; z <= zMaxMap ; z++)
				{
					// We don't go outside of the map
					if (z >= AI.zMax)
					{
						return false;
					}
					
					// We met an other collider here (wall or blocker)
					if (map[x,z] == 1)
					{
						lem.direction = (lem.direction+1) % 4;
						return true;
					}
				}
			}
			break;

		// Right
		case 1:
			zmin = lem.z - AI.lemmingHalfSize;
			zmax = lem.z + AI.lemmingHalfSize;
			for (int z = zmin; z <= zmax; z++)
			{
				xMaxMap = lem.x+AI.lemmingHalfSize+1+additionalDistance;
				for (int x = lem.x+AI.lemmingHalfSize+1 ; x <= xMaxMap ; x++)
				{
					// We don't go outside of the map
					if (x >= AI.xMax)
					{
						return false;
					}				
					
					// We met an other collider here (wall or blocker)
					if (map[x,z] == 1)
					{
						lem.direction = (lem.direction+1) % 4;
						return true;
					}	
				}
			}
			break;			
			
		// Down
		case 2:
			xmin = lem.x - AI.lemmingHalfSize;
			xmax = lem.x + AI.lemmingHalfSize;
			for (int x = xmin; x <= xmax; x++)
			{
				zMinMap = lem.z - AI.lemmingHalfSize - 1 - additionalDistance;
				for (int z = lem.z-AI.lemmingHalfSize-1 ; z >= zMinMap ; z--)
				{
					// We don't go outside of the map
					if (z < 0)
					{
						return false;
					}		
					
					// We met an other collider here (wall or blocker)
					if (map[x,z] == 1)
					{
						lem.direction = (lem.direction+1) % 4;
						return true;
					}
				}
			}
			break;
			
		// Left
		case 3:
			zmin = lem.z - AI.lemmingHalfSize;
			zmax = lem.z + AI.lemmingHalfSize;
			for (int z = zmin; z < zmax; z++)
			{
				xMinMap = lem.x-AI.lemmingHalfSize-1-additionalDistance;
				
				for (int x = lem.x-AI.lemmingHalfSize-1 ; x >= xMinMap ; x--)
				{
					// We don't go outside of the map
					if (x < 0)
					{
						return false;
					}				
					
					// We met an other collider here (wall or blocker)
					if (map[x,z] == 1)
					{
						lem.direction = (lem.direction+1) % 4;
						return true;
					}	
				}
			}
			break;			

		}
		return false;
	}

	

	
	
	// Execute the action in paramater on the simulated lemming
	// Return true if it is a blocking action (explosion, blocker, etc)
	// false otherwise
	private int ExecuteGeneAction(SimulatedLemming lem, Lemming.ACTION action)
	{
		// 0 == the action does not stop the lemming
		// 1 == the action does stop the lemming
		// 2 == the action kill the lemming
		int isABlockingAction = 0;
		int xMin;
		int zMin;
		int xMax;
		int zMax;
		
		switch(action)
		{

		// We create a blocker on the map
		// To simplify the simulation, we create a square instead of a circle
		case Lemming.ACTION.BLOCK:
			
			// We test the limits of the lemming collider, to be sure to be on the map
			xMin = lem.x - AI.lemmingHalfSize;
			zMin = lem.z - AI.lemmingHalfSize;
			
			if (xMin < 0)
			{
				xMin = 0;
			}
			if (zMin < 0)
			{
				zMin = 0;
			}
			
			xMax = lem.x + AI.lemmingHalfSize;
			zMax = lem.z + AI.lemmingHalfSize;
			
			if (xMax >= AI.xMax)
			{
				xMax = AI.xMax;
			}
			if (zMax >= AI.zMax)
			{
				zMax = AI.zMax;	
			}
						
			for (int i = xMin;i < xMax;i++)
			{
				for (int j = zMin;j < zMax;j++)
				{
					map[i,j] = 1;
				}
			}
			isABlockingAction = 1;
			break;
			// End of Block Action
			
			
			
			
		// We create a bridge on the map
		// WARNING : Right now, there is no 3 dimensions so we just assume that if the bridge is at the right distance of a wall or a blocker,
		// then it will go on the other side
		case Lemming.ACTION.BUILD:
			
			// If there is no blocker in front of the lemming (and close to it), we build a bridge
			if (!MeetABlocker(lem,(int)(AI.bridgeSize * 0.35f)))
			{
				switch(lem.direction)
				{
				// UP
				case 0:
					// TODO : calculate the width of a bridge
					xMin = lem.x - AI.lemmingHalfSize;
					xMax = lem.x + AI.lemmingHalfSize;
					zMax = lem.z + AI.bridgeSize;
					if (zMax > AI.zMax)
					{
						isABlockingAction = 2;
						break;
					}
					
					for (int x = xMin; x <= xMax; x++)
					{
						for (int z = lem.z ; z < zMax;z++)
						{						
							map[x,z] = 2;
						}
					}
					break;
		
				// Right
				case 1:
					// TODO : calculate the width of a bridge
					zMin = lem.z - AI.lemmingHalfSize;
					zMax = lem.z + AI.lemmingHalfSize;
					xMax = lem.x + AI.bridgeSize;
					if (xMax > AI.xMax)
					{
						isABlockingAction = 2;
						break;
					}
					
					for (int x = lem.x; x < xMax; x++)
					{
						for (int z = zMin ; z <= zMax;z++)
						{						
							map[x,z] = 2;
						}
					}
					break;			
					
				// Down
				case 2:
					// TODO : calculate the width of a bridge
					xMin = lem.x - AI.lemmingHalfSize;
					xMax = lem.x + AI.lemmingHalfSize;
					zMin = lem.z - AI.bridgeSize;
					if (zMin < 0)
					{
						isABlockingAction = 2;
						break;
					}
					
					for (int x = xMin; x <= xMax; x++)
					{
						for (int z = lem.z ; z > zMin;z--)
						{						
							map[x,z] = 2;
						}
					}
					break;
					
				// Left
				case 3:
					// TODO : calculate the width of a bridge
					zMin = lem.z - AI.lemmingHalfSize;
					zMax = lem.z + AI.lemmingHalfSize;
					xMin = lem.x - AI.bridgeSize;
					if (xMin < 0)
					{
						isABlockingAction = 2;
						break;
					}
					
					for (int x = lem.x; x > xMin; x--)
					{
						for (int z = zMin ; z <= zMax;z++)
						{						
							map[x,z] = 2;
						}
					}
					break;	
				}
			}
			break;
			// End of build action
			
			
		default:
			Debug.Log("Unknown action");
			break;
		}
		
		return isABlockingAction;
	}
	
	
	
	
	
	
	// Debug purpose : we print the map on a file
	public void PrintMap()
	{
		string s = "";
		for(int x = 0; x<AI.xMax;x++)
		{
			for(int z = 0; z<AI.zMax;z++)
			{
				s += map[x,z].ToString();
			}
			s += "\n";
		}
		
		// Log content in a file			
		TextWriter tw = new StreamWriter("Debug_map.txt");

		// write a line of text to the file
        tw.Write(s);

        // close the stream
        tw.Close();
	}
	
	
	
	
	
	
	// toString method
	public override string ToString() 
	{
		string s = "";
		int count = 0;
		foreach(Gene gene in geneList)
		{
			if (count != 0)
			{
				s += " - ";
			}
			s += gene.ToString();
			count++;
		}
		return s;
	}
	
	

	// To clone a chromosome
	public Chromosome Clone()
	{
		Chromosome chrom = new Chromosome();
		int count = geneList.Count;
		chrom.SetChromosomeSize(count);
		
		for (int i = 0; i < count; i++)
		{
			chrom.geneList[i] = new Gene(geneList[i]);
		}
		return chrom;
	}
	
	
	
	// When we set the size of the chromosome, we add or remove genes
	public void SetChromosomeSize(int s)
	{
		size = s;
		
		// Debug.Log("before :"+this+" ; size:"+size+" ; count:"+geneList.Count);
		// rebuild the genes'list if needed
		int count = geneList.Count;
		if (count < size)
		{
			for (int i=count; i < size; i++)
			{
				Gene gene = new Gene();
				geneList.Add(gene);
			}
		}
		else if (count > size)
		{
			for (int i=count-1; i >= size; i--)
			{
				geneList.RemoveAt(i);
			}
		}
		// Debug.Log("after :"+this+" ; size:"+size+" ; count:"+geneList.Count);
	}	
	
	
	
	/*
	 * Getters / Setters
	 */
	
	public int GetFitness()
	{
		return fitness;	
	}
	public int GetChromosomeSize()
	{
		return size;
	}	
	public static float GetCrossOverRate()
	{
		return Chromosome.crossoverRate;	
	}
	public static float GetMutationRate()
	{
		return Chromosome.mutationRate;	
	}
	public List<Gene> GetListGene()
	{
		return geneList;	
	}
	public void SetGene(Gene gene, int index)
	{
		geneList[index] = new Gene(gene);	
	}
	public bool GetSuccess()
	{
		return success;	
	}
}
