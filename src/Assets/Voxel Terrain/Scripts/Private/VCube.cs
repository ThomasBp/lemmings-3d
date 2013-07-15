using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VMap;

namespace VMap
{
	public class VCube
	{
		
		public GameObject _object;
		private Mesh _mesh;
		
		//private Matrix4x4 _worldToLocal;
		//private float[,,] _map;
		//
		//private List<Vector3> _vertices;
		//private List<int> _triangles;
		
		private Bounds _bounds;
		private int _minX, _minY, _minZ, _maxX, _maxY, _maxZ;
		private VoxelMap _map;
		
		public Bounds bounds{get{return _bounds;}}
		
		// Create new mesh zone
		public VCube(Bounds bounds, VoxelMap map)
		{
			/* Limit to this zone */
			
			_minX = (int) bounds.min.x;
			_minY = (int) bounds.min.y;
			_minZ = (int) bounds.min.z;
			_maxX = (int) bounds.max.x;
			_maxY = (int) bounds.max.y;
			_maxZ = (int) bounds.max.z;
			_bounds = bounds;
			
			// Save reference to map parent
			_map = map;
			
			/* Instanciate new game object */
			
			_object = new GameObject("VCube");
			_object.transform.parent = _map.transform;		// map parent
			
			// Add renderer with map materials
			_object.AddComponent<MeshRenderer>().materials = _map.renderer.materials;
			
			_object.AddComponent<MeshCollider>();					// Add collider
			_mesh = new Mesh();
			_object.AddComponent<MeshFilter>().sharedMesh = _mesh;	// Add mesh
			_mesh.bounds = _bounds;									// Add bounds
			
		}
		
		public void ReBuild()
		{
			int w = _map._width;
			int h = _map._height;
			int d = _map._depth;
			
			Vector3[] vertices;
			int[] triangles;
			Vector2[] uvY;
			Vector2[] uvZ;
			Color[] colors;
			
			// Delete old vertices and triangles mesh
			_mesh.Clear();
			
			VRender.MarchingCubesRender(_map._map,
			                            _minX, _minY, _minZ,
			                            _maxX, _maxY, _maxZ,
			                            out vertices,
			                            out triangles);
			
			//uvX = new Vector2[vertices.Length];
			uvY = new Vector2[vertices.Length];
			uvZ = new Vector2[vertices.Length];
			colors = new Color[vertices.Length];
			
			// Generate uv array
			for(int i=1; i<vertices.Length; i++)
			{
				// Planar with global position
				Vector3 v = vertices[i];
				
				// For top planar
				uvY[i] = new Vector2(v.x/w, v.z/d);
				
				// For side planar
				uvZ[i] = new Vector2(v.x/w, v.y/h);
				
				// Index of texture
				colors[i] = _map._clr[(int)v.x, (int)v.y, (int)v.z];
				
			}
			
			// Apply mesh
			_mesh.vertices = vertices;
			_mesh.triangles = triangles;
			
			_mesh.uv  = uvY;
			_mesh.uv2 = uvZ;
			_mesh.colors = colors;
			
			// May be write my normal after
			_mesh.RecalculateNormals();
			
		}
		
		// Rebuild collider after paint
		public void ReBuildCollider()
		{
			_object.GetComponent<MeshCollider>().sharedMesh = null;
			_object.GetComponent<MeshCollider>().sharedMesh = _mesh;	
		}
		
	}
}