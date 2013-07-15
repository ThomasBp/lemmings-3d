using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VTerrain;

namespace VTerrain
{
	public class VPoint
	{
		public float val;
		public Color tex;
		
		public VPoint(float val, Color tex)
		{
			this.val = val;
			this.tex = tex;
		}
	}
}