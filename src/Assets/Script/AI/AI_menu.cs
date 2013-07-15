using UnityEngine;
using System.Collections;

public class AI_menu : MonoBehaviour {

	// Graphical interface
	void OnGUI()
	{
		int width = 200;
		int height = 30;
	
		GUI.Box(new Rect(Screen.width/2 - 150, 120,300,30),"Artificial intelligence for the Lemmings 3D game");
		
        if (GUI.Button(new Rect(Screen.width/2 - (width/2), 200, width, height), "Simple level"))
		{	
			Application.LoadLevel("AI_level1");
			Gene.hasBlock = true;
			Gene.hasBridge = true;			
		}
        if (GUI.Button(new Rect(Screen.width/2 - (width/2), 240, width, height), "Bridge level"))
		{	
			Application.LoadLevel("AI_level2");
			Gene.hasBlock = false;
			Gene.hasBridge = true;
		}
	    if (GUI.Button(new Rect(Screen.width/2 - (width/2), 280, width, height), "Block level"))
		{	
			Application.LoadLevel("AI_level5");
			Gene.hasBlock = true;
			Gene.hasBridge = false;
		}
        if (GUI.Button(new Rect(Screen.width/2 - (width/2), 320, width, height), "Mixed level"))
		{	
			Application.LoadLevel("AI_level3");
			Gene.hasBlock = true;
			Gene.hasBridge = true;
		}
        if (GUI.Button(new Rect(Screen.width/2 - (width/2), 360, width, height), "Hardest level"))
		{	
			Application.LoadLevel("AI_level4");
			Gene.hasBlock = true;
			Gene.hasBridge = true;
		}
		
		if (GUI.Button(new Rect(Screen.width/2 - (width/2), 400, width, height), "Exit"))
		{	
			Application.Quit();
		}
	}

}
