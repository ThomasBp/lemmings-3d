using UnityEngine;
using System.Collections;

public class AIGrid : MonoBehaviour {
	
	public int[][] grid;
	private int sizeX = 20;
	private int sizeY = 20;
	
	// Use this for initialization
	void Start () {
		
		grid = new int[sizeX][];
		
		for (int i = 0; i < sizeX ; i++)
		{
			grid[i] = new int[sizeY];
			for (int j = 0; j < sizeY ; j++)
			{
				grid[i][j] = 0;
			}
		}		
	}
	
	// Update is called once per frame
	void Update () {
	
		for (int i = 0; i < sizeX ; i++)
		{
			for (int j = 0; j < sizeY ; j++)
			{
				Debug.DrawLine(new Vector3(i,0,j), new Vector3(i,0.01f,j),Color.red);
			}
		}
	}
}
