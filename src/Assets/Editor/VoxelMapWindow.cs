using UnityEngine;
using UnityEditor;

public class VoxelMapWindow : EditorWindow
{
	// My window
	//VmapWindow _window;
	
	// The map of this window
	VoxelMap _map;
	
	// Groupe
	bool _groupDimention;
	bool _groupPaint;
	bool _groupHeightMap;
	
	// Paint
	private bool    _paintEnable = false;
	//private float   _paintDistance = 10F;
	private Vector3 _paintPosition;
	private float   _paintSize = 1F;
	private VoxelMap.OBJ _obj = VoxelMap.OBJ.SPHERE;
	private VoxelMap.SFX _sfx = VoxelMap.SFX.ADD;
	
	// HeightMap
	Texture2D _heightmap;
	float _height;
	
    // Add menu named "VoxelMap" to the Window menu
    [MenuItem ("Window/VoxelMapWindow")]
    static void Init () {
		
        // Get existing open window or if none, make a new one:
        //VmapWindow _window = (VmapWindow)EditorWindow.GetWindow(typeof(VmapWindow));
		
    }
    
    void OnGUI () {
		
        GUILayout.Label ("VoxelMap", EditorStyles.boldLabel);
		_map = (VoxelMap) EditorGUILayout.ObjectField("map", _map, typeof(VoxelMap), true);
		
		// No map no process
		if(_map==null)
			return;
		
		// Button to turn on or off the map
		_map.Enable = EditorGUILayout.Toggle("Start Builder:", _map.Enable);
		
		if(!_map.Enable)
			return;
		
		// Dimention of map, multiple of 20 ("RES" size of VCube)
		_groupDimention = EditorGUILayout.BeginToggleGroup("Dimention", _groupDimention);
		if(_groupDimention){
			EditorGUILayout.BeginHorizontal();
			_map.Width  = EditorGUILayout.IntField(_map.Width);
			_map.Height = EditorGUILayout.IntField(_map.Height);
			_map.Depth  = EditorGUILayout.IntField(_map.Depth);
			EditorGUILayout.EndHorizontal();
		}
		EditorGUILayout.EndToggleGroup();
		
		// Paint
		_groupPaint = EditorGUILayout.BeginToggleGroup("Paint", _groupPaint);
		if(_groupPaint){
			_paintEnable = EditorGUILayout.Toggle("Start paint", _paintEnable);
			GUILayout.Label("Paint size");
			_paintSize = GUILayout.HorizontalSlider(_paintSize, 0, 20);
			
			if(EditorGUILayout.Toggle("Sphere", _obj==VoxelMap.OBJ.SPHERE))
				_obj = VoxelMap.OBJ.SPHERE;
			if(EditorGUILayout.Toggle("Cube", _obj==VoxelMap.OBJ.CUBE))
				_obj = VoxelMap.OBJ.CUBE;
			if(EditorGUILayout.Toggle("Random", _obj==VoxelMap.OBJ.RANDOM))
				_obj = VoxelMap.OBJ.RANDOM;
			
			if(EditorGUILayout.Toggle("Add", _sfx==VoxelMap.SFX.ADD))
				_sfx = VoxelMap.SFX.ADD;
			if(EditorGUILayout.Toggle("Sub", _sfx==VoxelMap.SFX.SUB))
				_sfx = VoxelMap.SFX.SUB;
			
			// Destroy brush object
			if(_paintEnable==false&&_brush!=null)
				GameObject.DestroyImmediate(_brush);
			// Create brush object
			if(_paintEnable==true&&_brush==null)
				_brush = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		}
		EditorGUILayout.EndToggleGroup();
		
		// Beta Height Map
		_groupHeightMap = EditorGUILayout.BeginToggleGroup("Height Map", _groupHeightMap);
		if(_groupHeightMap){
			_heightmap = (Texture2D) EditorGUILayout.ObjectField("HeightMap", _heightmap, typeof(Texture2D), true);
			float height = EditorGUILayout.Slider("Height", _height, 0, _map.Height);
			if(height!=_height){
				_height = height;
				_map.HeightMap(_heightmap, _height);
			}
		}
		EditorGUILayout.EndToggleGroup();
		
		
		EditorGUILayout.Toggle("Toggle", true);
		EditorGUILayout.Slider("Slider", 0, -3, 3);
		
		// Rebuild 5 VCube in map list, if the map is enable
		for(int i=0; i<5; i++)
			_map.Update();
		
		/*
            myString = EditorGUILayout.TextField ("Text Field", myString);
        
		
		
        groupEnabled = EditorGUILayout.BeginToggleGroup ("Optional Settings", groupEnabled);
            myBool = EditorGUILayout.Toggle ("Toggle", myBool);
            myFloat = EditorGUILayout.Slider ("Slider", myFloat, -3, 3);
        EditorGUILayout.EndToggleGroup ();*/
    }
	
	private GameObject _brush;
	
	public void Update()
	{
		if(_map==null)
			return;
		if(_paintEnable&&_brush!=null)
		{
			// Apply transform
			_brush.transform.localScale = new Vector3(_paintSize, _paintSize, _paintSize);
			
			// map alteration
			Transform t = _brush.transform;
			_map.Alteration(t.position, t.localScale, _obj, _sfx, Color.red);
		}
	}
	
}
//	
//	
//	public int toolbarInt = 0;
//    public string[] toolbarStrings = new string[] {"Toolbar1", "Toolbar2", "Toolbar3"};
//	
//	private bool    _paintEnable = false;
//	private float   _paintDistance = 10F;
//	private Vector3 _paintPosition;
//	private float   _paintSize = 1F; 
//	
//	public void OnSceneGUI()
//	{
//		VBrush brush = ((VBrush)target);
//		
//		Handles.BeginGUI();
//		
//		// Get event
//		Event e = Event.current;
//		if (e!=null)
//		{
//			// Get paintPosition
//			Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
//			object hit = HandleUtility.RaySnap(ray);
//			if(hit!=null)
//			{
//				if(_paintEnable == false)
//					_paintDistance = ((RaycastHit)hit).distance;
//			}
//			_paintPosition = ray.GetPoint(_paintDistance);
//			
//			brush.transform.position = _paintPosition;
//			brush.transform.localScale = new Vector3(1,1,1)*_paintSize;
//			
//			// Get paint position
//			
//			
//			switch(e.type)
//			{
//			case EventType.MouseDown : _paintEnable = true;  break;
//			case EventType.MouseUp :   _paintEnable = false; break;
//			}
//			
//			if(_paintEnable == true)
//				brush.Paint(_paintPosition, _paintSize, VBrush.SFX.ADD_SPHERE);
//		}
//		
//		//GUILayout.Box(paintDistance.ToString());
//		
//		Handles.EndGUI();
//		//GUILayout.BeginArea(target.ViewRect);
//		
//		
////		Event e = Event.current;
////		if (e!=null) switch(e.type)
////		{
////		case EventType.MouseDown :
////		case EventType.MouseUp :
////		case EventType.MouseMove :
////			
////			Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
////			RaycastHit hit = (RaycastHit)HandleUtility.RaySnap(ray);
////			
////			
////			//Display.Point(hit.point, Color.red);
////			
////			Event.current.Use();
////			
////			break;
////		}
//		
//		
//		
//		//wantsMouseMove = EditorGUILayout.Toggle("Receive Movement: ", wantsMouseMove);
//		//Gui.LabelField("Mouse Position: ", Event.current.mousePosition.ToString());
//		
//		
//		
//		//sceneview.camera.
//		
//		//selected = GUI.SelectionGrid(new Rect(25, 25, 250, 30), selected, new St
//		 //GUI.Button(new Rect(25, 25, 250, 30), "paint");
//		//layer = EditorGUI.LayerField(new Rect(25, 25, 250, 30), layer);
//		//toolbarInt = GUI.Toolbar(new Rect(25, 25, 250, 30), toolbarInt, toolbarStrings);
//        //GUILayout.Label("Base Settings", EditorStyles.boldLabel);
//		
//	}
//	
////	public override void OnInspectorGUI()
////	{
////	}
//}