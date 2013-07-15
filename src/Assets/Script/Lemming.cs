using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Lemming : MonoBehaviour {
	// For debug only 
//	public static bool isFalling = false;
	
	
	public static float _deltaTime = 0.02f;
	
	// all lemmings
	public static List<Lemming> lemmings = new List<Lemming>();
	
	// Delegate function for product action (its like interface with one method)
	private delegate void Product();
	
	// Const and enum
	public const float FALL_DURATION_FOR_DEATH = 0.5F;	// second
	public const float FALL_SPEED = 10F;
	public const float FLOAT_SPEED = 5F;
	public const float CLIMB_SPEED = 2F;
	public const float WALK_SPEED = 2F;
	public const float BUILD_SPEED = 8F; // many bricks by second
	public const float WALL_DISTANCE = 0.4F;
	public const float CLIMB_DISTANCE = 0.3F; // distance for climb between lemming and wall
	public const float BLOCK_DISTANCE = 0.4F;
	public const float BRICK_X = 0.6F;
	public const float BRICK_Y = 0.2F;
	public const float BRICK_Z = 0.4F;
	
	const int LEMMING_LAYER = ~(1 << 8); // Because the lemming see himself with a simple ray
	
	public enum ACTION {
		
		DEFAULT,	// walk, turn right and fall
		
		// onUpdate (call once by frame)
		MINE,		// dig down forward TODO
		EXPLODE,	// explode and dig around TODO
		DIG,		// dig down TODO
		BUILD,		// up and forward TODO
		BLOCK,		// change the other lemming direction
		
		// onFall
		FLOAT,		// with umbrella
		
		// onBlock (when is not a bounce surface or other lemming)
		CLIMB,		// up
		BASH		// dig forward
		
	}
	
	// Public variable
	public ACTION onUpdate = ACTION.DEFAULT;
	public ACTION onBlock = ACTION.DEFAULT;
	public ACTION onFall = ACTION.DEFAULT;
	
	// Private variable
	private Vector3 _straight;			// Direction straight of lemming
	private Vector3 _head;				// Head position in world space
	private Vector3 _feet;      		// Feet position in world space
	private RaycastHit _hit;			// The last hit (Property of collision point)
	private float _fallDuration = 0F;	// For product death if more than FALL_DURATION_FOR_DEATH
	private Product product = null;		// This function is call one by frame and override Update()
	private float build_time;
	private float build_number;
	
	private float _angle;
	
	private GameObject _object;
	
	//private bool _isClimbEnable = false;	// When the lemming climb
	//private bool _isBlockEnable = false;	// When the lemming block other lemming
	
	// Use this for initialization
	void Start () {
//	public void ManualStart () {		
		
		// Add lemming in a list
//		if(Lemming.lemmings == null)
//			Lemming.lemmings = new List<Lemming>();
		Lemming.lemmings.Add(this);
		
		// productStart
		transform.localScale = new Vector3(1F, 0F, 1F);
		product = new Product(ProductStart);
		_object = transform.GetChild(0).gameObject;
	}
	
	// Product the climb action
	private void ProductClimb()
	{
		/// Increment straight when the lemming fall
		/// $$$$$$$$$$$$$$$$$$$
		/// $$$$$$  ------>
		/// $$$$$$ | If the lemmings can increment up, the lemming fall ?
		/// $$$$$$ | 
		/// $$$$$$ | Increment up when the head or the feet see a wall
		/// $$$$$$ |
		
		/// TO DO : Lemming can increment up
		
		/// Increment straight when the lemming fall
		///  <-----
		///        |
		/// $$$$$$ |
		/// $$$$$$ | Increment up when the head or the feet see a wall
		/// $$$$$$ |
		/// $$$$$$ |
		
		
		
		// If wall on straight directory
		if(Physics.Raycast(_head, _straight, out _hit, 1F, LEMMING_LAYER) ||
		   Physics.Raycast(_feet, _straight, 1F, LEMMING_LAYER)){
			// For visual effect the lemming close the wall with a distance for climb
			if(_hit.distance>CLIMB_DISTANCE)
			{
				ProductWalk();
			}
			else
			{
				// Go to up
				transform.position += Vector3.up * CLIMB_SPEED * _deltaTime;
			}
			return;
		}
		
		// For visual effect the lemming close the wall with a distance for fall after climb
		Vector3 rayPos = _feet - (WALL_DISTANCE*_straight);
		
		// If the lemming levitate, look the down direction of lemming
		if(!Physics.Raycast(rayPos, Vector3.down, 0.1F, LEMMING_LAYER))
		{
			ProductWalk();
			return;
		}
		
		// Stop climb
		product = null;
		
	}

	// Product the walk action
	private void ProductWalk()
	{
		// Go to straight for walk
		transform.position += _straight * WALK_SPEED * _deltaTime;
	}
	
	// You must went to right now
	private void ProductBlock()
	{
		/// PROBLEM:
		/// If I dig in lower part of a blocked lemming, the lemming can be fly :)
		
		/// - if the lemming see a object in one meter
		/// - if the object is a lemming
		/// - if the rotation of object is invert of this
		if(Physics.Raycast(_head, _straight, out _hit, BLOCK_DISTANCE) &&
		   _hit.transform.tag == "lemming" &&
		   _hit.transform.rotation*Vector3.back == _straight
		   ){
			// You must went to right now!
			_hit.transform.Rotate(Vector3.up, 90F);
		}
	}
	
	private void ProductBuild()
	{
		build_time += _deltaTime * BUILD_SPEED;
		if(build_time>build_number)
		{
			/// build 
			///              __ y  /|\
			///   $$$$$$$$$$$      \|/ <- Lemming
			///   $$$$$+------- y/2 |
			///   $$$$$|$$$$$     ---  <- Feet
			///   |    |     o <- origin
			///   z   z/2
			/// - Brick   = orign + z/2 + y/2
			/// - Lemming = orign + z/2 + y
			GameObject brick = GameObject.CreatePrimitive(PrimitiveType.Cube);
			brick.tag = "bridge";
			
			//Destroy(brick.collider);
			//GameObject brick = (GameObject) Instantiate(GameObject.CreatePrimitive(PrimitiveType.Cube), Vector3.zero, transform.rotation);
			Vector3 orign = transform.position;
			Vector3 z = _straight * BRICK_Z;
			Vector3 y = Vector3.up * BRICK_Y;
			transform.position = orign + z/2 + y;
			brick.transform.position = orign + z/2 + y/2;
			brick.transform.localScale = new Vector3(BRICK_X, BRICK_Y, BRICK_Z);
			
			build_number++;
			
			// end ifs
			// - no brick
			// - wall in straight
			// - ceiling
			if(build_number > 10 ||
			   Physics.Raycast(_head, _straight, WALL_DISTANCE, LEMMING_LAYER) ||
			   Physics.Raycast(_head, Vector3.up, WALL_DISTANCE, LEMMING_LAYER))
			{
				Grounded();
				onUpdate = ACTION.DEFAULT;
				product = null;
				return;
			}
		}
	}
	
	// Delete lemming (particle and size)
	private void ProductDeath()
	{
		// the lemming must to go up the ground
		if(Physics.Raycast(_head, Vector3.down, out _hit, 2F, LEMMING_LAYER))
		{
			transform.position = Vector3.Lerp(transform.position, _hit.point, 0.4F);
		}
		// narrowed
		transform.localScale += Vector3.down * _deltaTime * 40F;
		// if end of narrowed
		if(transform.localScale.y<0)
		{
			IHM.IWillDie();
			// Delete lemming
			Destroy(gameObject);
		}
	}
	
	// Start effect when the lemming is add on scene
	private void ProductStart()
	{
		// Step1 : fall
		transform.position += Vector3.down * FALL_SPEED * _deltaTime;
		
		// Step2 : size
		if(transform.localScale.y < 1F)
		{
			transform.localScale += Vector3.up * FALL_SPEED/2 * _deltaTime;
			return;
		}
		
		// Step3 : end
		transform.localScale = new Vector3(1F, 1F, 1F);
		product = null;
	}
	
	// When the lemming must to go up the ground
	private void Grounded()
	{
		if(Physics.Raycast(_head, Vector3.down, out _hit, 2F, LEMMING_LAYER))
		{
			transform.position = Vector3.Lerp(transform.position, _hit.point, 0.4F);
		}
	}
	
	// When the lemming is blocking by the end door
	private void ProductEnd()
	{
		transform.localScale -= new Vector3(1F,1F,1F) * 5 * _deltaTime;
		transform.position += (_straight*WALL_DISTANCE + Vector3.up) * 5 * _deltaTime;
		if(transform.localScale.x<0)
		{
			IHM.IFindExit();
			Destroy(gameObject);
		}
	}
	
	// Call when the lemming fall
	private void OnFall()
	{
		// debug only
//		isFalling = true;
		
		switch(onFall){
				
		// you fall if you have not an umbrella
		case ACTION.DEFAULT :
			
			_fallDuration += _deltaTime;
			transform.position += Vector3.down * FALL_SPEED * _deltaTime;
			break;
			
		// you float if you have an umbrella
		case ACTION.FLOAT :
			
			_fallDuration=0F;
			transform.position += Vector3.down * FLOAT_SPEED * _deltaTime;
			break;
			
		default : Debug.Log("onFall cannot be equal to " + onFall); break;
			
		}
	}
	
	// Test if the lemming death
	private void OnAfterFall()
	{
		// The lemming death when the fall speed is too big
		if(_fallDuration>FALL_DURATION_FOR_DEATH)
		{
			product = new Product(ProductDeath);
		}
	}
	
	// Call when the lemming is blocked
	private void OnBlock()
	{
		/// If the wall can be dig or climb
		/// - We need to test if the wall is perpendiculate to lemming straight
		
		// Get outlect for straight of lemming on a wall
		Vector3 outlect = Vector3.Reflect(_straight, _hit.normal);
		
		// Update outlect to 90°
		outlect.y = 0F;
		if(Mathf.Abs(outlect.x)>Mathf.Abs(outlect.z))
		{
			outlect.x = outlect.x>0F ? 1F : -1F;
			outlect.z = 0F;
		}
		else
		{
			outlect.x = 0F;
			outlect.z = outlect.z>0F ? 1F : -1F;
		}
		
//		Debug.Log("angle = "+Vector3.Angle(_straight, outlect));
		
		// if angle = 180
		if(Mathf.Round(Vector3.Angle(_straight, outlect)/90F)==2F)
		{
			// Switch the action for block
			switch(onBlock){
		
			case ACTION.CLIMB :
				// is call in the first step of Update();
				product = new Product(ProductClimb);
				return; // Stop here
			case ACTION.DEFAULT : // Go to lemming right
				// Update to rotate to +90° but I think is not a good
				transform.Rotate(Vector3.up, 90F);
				break;
			// Go to dig forward
			case ACTION.BASH :
				break;
			default : Debug.Log("onBlock cannot be equal to " + onBlock); break;
			}
		}
		else
		{
			//product = new Product(ProductRotation);
			// Rotate to +-90°
			transform.rotation = Quaternion.LookRotation(outlect);
			transform.position -= _straight * 0.1F; // For solve a problem
		}
	}
	
	// Call after all call function
	private void OnUpdate()
	{
		switch(onUpdate){
			
		case ACTION.DEFAULT : // Walk on straight
				
			ProductWalk();
			break;
			
		case ACTION.MINE :
			break;
			
		case ACTION.EXPLODE :
			break;
			
		case ACTION.DIG :
			break;
			
		case ACTION.BUILD :
			build_time = 0;
			build_number = 0;
			Grounded();
			product = new Product(ProductBuild);
			break;
			
		case ACTION.BLOCK :
			IHM.IWillDie();
			// Rotate the lemming to 180°
			transform.Rotate(Vector3.up, 180);
			product = new Product(ProductBlock);
			break;
				
		default : Debug.Log("onUpdate cannot be equal to " + onUpdate); break;
			
		}	
	}
	
	// Update is called once per frame
	void FixedUpdate()
//	public void ManualUpdate()		
	{
		_straight = transform.rotation * Vector3.forward;
		_head = transform.position + Vector3.up;
		_feet = transform.position;
		
		/// First priority if the lemming product an action
		/// We need to override Update for
		/// - "climb" because no ground but we need the lemming no fall
		/// - "block" because fixe position
		if(product != null)
		{
			product();
			return;
		}
		
		// The lemming close the wall with a distance for fall
		Vector3 rayPos = _head - (WALL_DISTANCE*_straight);
		
		Debug.DrawRay(rayPos, Vector3.down, Color.red);
		
		/// Second priority if the lemming fall
		/// - When the lemming falls, what utility if there is a wall or not in front of him?
		/// - The lemming fall if there is no ground in lower part of him, with distance < 1.2F
		if(!Physics.Raycast(rayPos, Vector3.down, out _hit, 1.3F, LEMMING_LAYER))
		{
			OnFall();
			return; // end of update function	
		}
		
		Debug.DrawRay(_head, Vector3.down, Color.blue);
		
		// the lemming must to go up the ground
		Grounded();
		
		if(_fallDuration>0F){
			OnAfterFall();
			_fallDuration=0F;
		}
		
		/// We need to start the ray back the lemming
		///         ___      ___
		///        |$$$     |$$$
		/// ------>|$$$   ->|$$$
		///   /\   |$$$     /\$$    Like this the lemming can be enter in the wall
		///   \/   |$$$ =>  \/$$
		///   /-   |$$$     /-$$
		///   \_   |$$$     \_$$
		
		Debug.DrawLine(rayPos, rayPos+_straight*1F, Color.green);
		
		/// Third priority if the lemming is blocked
		/// - if the lemming is blocked
		/// - and is not a other lemming
		if(Physics.Raycast(rayPos, _straight, out _hit, WALL_DISTANCE*2F, LEMMING_LAYER))
		{
			if(_hit.transform.tag=="end")
			{
				product = new Product(ProductEnd);
//				Lemming.lemmings.Remove(this);
				return;
			}
			OnBlock();
			return; // end of update function
		}
		
		/// Other step the update function
		/// - When no fall or no block
		OnUpdate();
	}
	
	// Get action
	void OnMouseEnter() {
		_object.renderer.material.color = new Color(1F, 0.5F, 0F);
	}
	
	void OnMouseExit() {
		_object.renderer.material.color = new Color(1F, 1F, 1F);
	}
	
	void OnMouseUp(){
		switch(IHM.action)
		{
		case ACTION.MINE:
		case ACTION.EXPLODE:
		case ACTION.DIG:
		case ACTION.BUILD:
		case ACTION.BLOCK:
			onUpdate = IHM.action;
			return;
		case ACTION.FLOAT:
			onFall = IHM.action;
			return;
		case ACTION.CLIMB:
		case ACTION.BASH:
			onBlock = IHM.action;
			return;
		}
	} 
	
}
