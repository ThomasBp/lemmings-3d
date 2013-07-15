using UnityEngine;
using System.Collections;

public class RunLemmingChromosome : MonoBehaviour {
	
	// The interval used between 2 ticks
	// A "tick" is the time unit used in a gene
	public static float interval = 0.1f;
	
	// The chromosome we will run
	public Chromosome chrom;
	
	// The current tick
	private int currentTick = -1;
	
	// The actual unity time between 2 ticks
	private float resetTime = 0;
		
	// If set to false, nothing run
	public bool start = false;
	
	// The current lemming used for an action
	private int currentLemming = 0;
	
	
	// Use this for initialization
	void Start () {
		chrom = new Chromosome();
	}
	
	
	// Update is called once per frame
//	void FixedUpdate () 
	void Update () 
	{
		if (!start)
			return;
		
		resetTime += Time.fixedDeltaTime;
		
		if (resetTime < interval)
		{
			return;
		}

		
		resetTime = 0;
		
		// Debug only
//		if (Lemming.isFalling)
//			return;
		
		currentTick++;
		
		if (chrom.geneList.Count == 0)
			return;
		
		if (currentLemming >= Lemming.lemmings.Count || Lemming.lemmings[currentLemming] == null)
		{
//			Debug.Log("Current : "+currentLemming+" - lemming count :"+Lemming.lemmings.Count);
			return;
		}
			
		
		Gene currentGene = ((Gene)chrom.geneList[0]);
		
		if (currentTick >= currentGene.GetGeneTime() && currentLemming < Lemming.lemmings.Count)
		{
			Lemming.ACTION action = currentGene.GetGeneAction();
			
			if (action != Lemming.ACTION.DEFAULT)
			{
				chrom.geneList.Remove(currentGene);
				
				switch(action)
				{
					case Lemming.ACTION.MINE:
					case Lemming.ACTION.EXPLODE:
					case Lemming.ACTION.DIG:
					case Lemming.ACTION.BUILD:
					case Lemming.ACTION.BLOCK:
						Lemming.lemmings[currentLemming].onUpdate = action;
						break;
					case Lemming.ACTION.FLOAT:
						Lemming.lemmings[currentLemming].onFall = action;
						break;
					case Lemming.ACTION.CLIMB:
					case Lemming.ACTION.BASH:
						Lemming.lemmings[currentLemming].onBlock = action;
						break;
				}
				
				switch(action)
				{
				case Lemming.ACTION.EXPLODE:
				case Lemming.ACTION.BLOCK:
					currentLemming++;
					break;
				}				
			}			
		}
	}
	
	public static void ResetRun()
	{
		RunLemmingChromosome run = GameObject.FindObjectOfType(typeof(RunLemmingChromosome)) as RunLemmingChromosome;
		run.Reset();
	}
	
	public static void Run(Chromosome chrom)
	{
		RunLemmingChromosome run = GameObject.FindObjectOfType(typeof(RunLemmingChromosome)) as RunLemmingChromosome;
		run.chrom = chrom.Clone();
		run.start = true;
		run.Reset();
	}
	
	
	public void Reset()
	{
		currentTick = -1;
		resetTime = 0;
		currentLemming = 0;
		
		GameObject[] bridges = GameObject.FindGameObjectsWithTag("bridge");
		foreach(GameObject bridge in bridges)
		{
			Destroy(bridge);	
		}
		
		Lemming.lemmings.Clear();
		Gene.possibleActions = null;
	}
	
	
	void OnGUI () {		
		// Make a background box
		GUI.Box (new Rect(10,50,200,30), "Current tick : "+currentTick);
		
		if (chrom != null && chrom.geneList != null && chrom.geneList.Count == 0)
			return;
		
		if (currentLemming >= Lemming.lemmings.Count 
		    || Lemming.lemmings[currentLemming] == null)
		{
			return;
		}
			
		
		Gene currentGene = ((Gene)chrom.geneList[0]);
		GUI.Box (new Rect(10,10,200,30), "Next Gene : "+currentGene);
	}
}

	