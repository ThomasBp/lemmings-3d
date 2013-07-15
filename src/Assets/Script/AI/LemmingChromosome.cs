using UnityEngine;
using System.Collections;
using System.Collections.Generic;

	// Class for test purpose only
public class LemmingChromosome : Chromosome{

	
	public LemmingChromosome()
	{
		fitness = 0;
		geneList = new List<Gene>();

		LemmingGene gene = new LemmingGene(Lemming.ACTION.BLOCK,30);
		geneList.Add(gene);

		gene = new LemmingGene(Lemming.ACTION.BLOCK,40);
		geneList.Add(gene);
	}
}
