using UnityEngine;
using System.Collections;


using System;
using System.Collections.Generic;
using System.Text;

public class GeneComparer : IComparer<Gene> {
  public int Compare(Gene g1, Gene g2)
  {
     int returnValue = -1;
     if (g1 != null && g2 != null)
     {
        returnValue = g1.GetGeneTime().CompareTo(g2.GetGeneTime());
     }

     return returnValue;
  }
}