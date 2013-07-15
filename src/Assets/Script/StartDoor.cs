using UnityEngine;
using System.Collections;

public class StartDoor : MonoBehaviour {
	
	public Lemming LemmingObject;
	public static int numberOfLemmings = 10;
	public static float speedProduct = 0.5F;
	
	private int number;
	private float lastTime;
//	private float timer;
	
	private RunLemmingChromosome run;
	
	// Use this for initialization
	void Start () {
		number = 0;
		lastTime = Time.time+1F;
//		lastTime = 1f;
//		timer = 0f;
		name = "Start";
		run = GameObject.FindObjectOfType(typeof(RunLemmingChromosome)) as RunLemmingChromosome;
	}
	
	// Update is called once per frame
	void Update () {
		if (run.start == false)
		{
			return;
		}
		
		if(number<numberOfLemmings && lastTime<Time.time)
		{
			lastTime = Time.time+speedProduct;
			number++;
			Lemming lemming = Instantiate(LemmingObject) as Lemming;
			lemming.transform.position = transform.position;
		}
	}
	
	
	public void Reset()
	{
		number = 0;
		GameObject[] lems = GameObject.FindGameObjectsWithTag("lemming");
		foreach(GameObject lem in lems)
		{
			Destroy(lem);
		}
	}
	
	
//	public void ManualUpdate()
//	{
//		timer += Lemming._deltaTime;
//		if(number<numberOfLemmings && lastTime<timer)
//		{
//			lastTime = timer+speedProduct;
//			number++;
//			Lemming lemming = Instantiate(LemmingObject) as Lemming;
//			lemming.transform.position = transform.position;
//			lemming.ManualStart();
//			Debug.Log("New lemming id = "+number + " - total lem = "+Lemming.lemmings.Count);
//		}
//	}
	
	
	void OnDrawGizmos(){
		
		// The direction to the up of object
		//Vector3 direction = transform.rotation * Vector3.up;
		//Vector3 origin = transform.position;
		
		//RaycastHit hitBack;
		//RaycastHit hitDown;
		
		/*
		for(int i=0; i<nbRaycast; i++){
			if(Physics.Raycast(new Ray(origin, direction), out hit)){
				if(i<nbDisplay){
					Gizmos.color = new Color((float)i/nbDisplay,1F,0F,1F);
					Gizmos.DrawLine(origin, hit.point);
				}
				origin = hit.point;
				direction = Vector3.Reflect(direction, hit.normal);
			};
		}
		*/
		
	}
	
}
