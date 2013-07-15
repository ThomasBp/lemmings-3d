using UnityEngine;
using System.Collections;

public class LemmingGene : Gene {
	
	public int time;
	public Lemming.ACTION action;
	
	public LemmingGene(Lemming.ACTION act, int t)
	{
		action = act;
		time = t;
	}
}
