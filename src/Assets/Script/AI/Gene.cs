using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Gene {
	
	// The list of possible actions for the genes
	public static List<Lemming.ACTION> possibleActions;
	public static bool hasBridge = true;
	public static bool hasBlock = true;

	
	// The maximum time for the genes
	public static int maxTime = 500;
	
	// The maximum mutation change : a mutation will increase or decrease the time of the gene by maximum this amount
	public static int mutationTimeIntervalMax = 30;
	
	
	// The action the lemming will do
	private Lemming.ACTION action;
	
	// The time when the lemming will use the action
	private int time;
	
	
	public Gene()
	{
		// If it is the first gene we call, we initialize the possible actions
		if (possibleActions == null)
		{
			possibleActions = new List<Lemming.ACTION>();
			
			if (hasBlock)
				possibleActions.Add(Lemming.ACTION.BLOCK);
			
			if (hasBridge)
				possibleActions.Add(Lemming.ACTION.BUILD);
		}
	}
	
	
	public Gene(Gene copy)
	{
		time = copy.time;
		action = copy.action;
	}
	
	
	// Generate a random gene : random time and random action
	public void GenerateRandom()
	{
		time = (int)Random.Range(1,Gene.maxTime);
		
		int randomAction = (int)Random.Range(0,possibleActions.Count);		
		action = possibleActions[randomAction];
	}
	
	
	// Mutate the gene
	public void Mutation()
	{	
		// No negative time allowed, obviously
		int minTime = time-mutationTimeIntervalMax;
		if (minTime < 0)
			minTime = 0;
		
		// No time higher than the maximum time
		int maxTime = time+mutationTimeIntervalMax;
		if (maxTime > Gene.maxTime)
			maxTime = Gene.maxTime;
		
		time = (int)Random.Range(minTime,maxTime);
	}
	
	
	
	/*
	 * Getter / Setter
	 */
	
	public int GetGeneTime()
	{
		return time;
	}
	public Lemming.ACTION GetGeneAction()
	{
		return action;
	}
	
	public void SetGeneTime(int t)
	{
		time = t;
	}
	public void SetGeneAction(Lemming.ACTION act)
	{
		action = act;
	}
	
	
	// Tostring method
	public override string ToString()
    {
		string s = "("+time.ToString()+",";	
		switch(action)
		{
			case Lemming.ACTION.MINE:
				s+="MINE";
				break;
			case Lemming.ACTION.EXPLODE:
				s+="EXPLODE";
				break;
			case Lemming.ACTION.DIG:
				s+="DIG";
				break;
			case Lemming.ACTION.BUILD:
				s+="BUILD";
				break;
			case Lemming.ACTION.BLOCK:
				s+="BLOCK";
				break;
			case Lemming.ACTION.FLOAT:
				s+="FLOAT";
				break;
			case Lemming.ACTION.CLIMB:
				s+="CLIMB";
				break;
			case Lemming.ACTION.BASH:
				s+="BASH";
				break;
			default:
				return "";
		}		
		return s+")";// c.ToString();
    }
}
