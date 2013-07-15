using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SimulatedLemming{
	public int x;
	public int z;
	
	/*
	 * 0 = up
	 * 1 = right
	 * 2 = down
	 * 3 = left
	 * 
			0
		  3 X 1
		    2 

	 */
	public int direction; 
	
	public SimulatedLemming(int newx, int newz, int newdir)
	{
		x = newx;
		z = newz;
		direction = newdir;
	}
}
