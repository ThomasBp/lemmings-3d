using UnityEngine;
using System.Collections;

public class GridPosition : MonoBehaviour {
	
	void OnDrawGizmosSelected(){
		
		Vector3 pos = transform.position;
		pos.x = Mathf.Round(pos.x);
		pos.y = Mathf.Round(pos.y);
		pos.z = Mathf.Round(pos.z);
		transform.position = pos;
		float angle;
		Vector3 axe = Vector3.up;
		transform.rotation.ToAngleAxis(out angle, out axe);
		Debug.Log(angle+", "+axe);
		
		Debug.DrawRay(pos, axe);
		
		/*
		rot.x = (Mathf.Round(rot.x/22.5F)*22.5F)%360F;
		rot.y = (Mathf.Round(rot.y/22.5F)*22.5F)%360F;
		rot.z = (Mathf.Round(rot.z/22.5F)*22.5F)%360F;
		if(rot.x<0) rot.x = 360F+rot.x;
		if(rot.y<0) rot.y = 360F+rot.y;
		if(rot.z<0) rot.z = 360F+rot.z;
		transform.rotation = Quaternion.Euler(rot);
		*/
		
	}
	
}
