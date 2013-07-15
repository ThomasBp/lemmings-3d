using UnityEngine;
using System.Collections;

[RequireComponent (typeof (CharacterController))]

[AddComponentMenu("Character/Lemming")]

public class NewLemming : MonoBehaviour {
	
	Vector3 velocity = new Vector3(0F, -1F, 1F);
	
	void Update()
	{
		CharacterController characterController = GetComponent<CharacterController>();
		characterController.Move(velocity*Time.deltaTime);
	}
	
	void OnControllerColliderHit(ControllerColliderHit hit)
	{
		Debug.Log("hit");
		if(hit.normal.y>0.5F)
			Debug.Log("grounded");
	}
	
}
