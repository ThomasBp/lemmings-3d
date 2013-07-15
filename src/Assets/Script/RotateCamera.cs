using UnityEngine;
using System.Collections;

public class RotateCamera : MonoBehaviour {
	
	public float MOUSE_SPEED_X = -3F;
	public float MOUSE_SPEED_Y = -1F;
	public float MAX_Y = 45;
	
	public Vector3 target = Vector3.zero;
	public Vector2 rotation = Vector3.zero;
	public float distance = 20F;
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButton(1))
		{
			float rotX = 2F * Input.mousePosition.x / Screen.width  - 1F;  // -1 <= rotX <= +1
			float rotY = 2F * Input.mousePosition.y / Screen.height - 1F; // -1 <= rotY <= +1
			
			rotX = Mathf.Pow(rotX, 5) * MOUSE_SPEED_X; // rotX^5 * MOUSE_SPEED_X
			rotY = Mathf.Pow(rotY, 5) * MOUSE_SPEED_Y; // rotY^5 * MOUSE_SPEED_Y
			
			rotX += rotation.x;
			rotY += rotation.y;
			rotY = rotY>MAX_Y?MAX_Y:(rotY<-MAX_Y?-MAX_Y:rotY);
			
			rotation = new Vector2(rotX, rotY);
			
			UpdateCamera();
		}
	}
	
	void UpdateCamera()
	{
		// Go to distance point
		transform.position = target + new Vector3(0F,0F,-distance);
		
		// Go to rotation
		transform.RotateAround(target, Vector3.left, rotation.y);
		transform.RotateAround(target, Vector3.up,   rotation.x);
		
		// LookAt center map
		transform.LookAt(target);
	}
	
	void OnDrawGizmosSelected()
	{
		UpdateCamera();
		
		Gizmos.DrawLine(transform.position, target);
	}
	
}
