using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]	 //Check for MeshFilter at build
[RequireComponent(typeof(MeshRenderer))] //Check for MeshRenderer at build
[RequireComponent(typeof(MeshCollider))] //Check for MeshCollider at build

public class TileMap : MonoBehaviour {
	//==========================================================================
	//Variables
	//==========================================================================
	public int size_x;			//Max x-tiles
	public int size_y; 		//Max z-tiles
	public float tileSize = 1.0f;	//Floating tile size based on unity units	
	public Texture2D terrainTile;
	public int tileResolution;
	//==========================================================================
	//Function
	//==========================================================================
	void Start () {
		BuildMesh ();
	}
	//==============================================================================================
	//Can use texture to test the map generation code
	//==============================================================================================

	Color[][] ChopUpTiles(){
		int tilePerRows = terrainTile.width / tileResolution;
		int tileRows = terrainTile.height / tileResolution;

		Color[][]tiles = new Color[tilePerRows * tileRows][];
		/*
		for (int y = 0; y < tileRows; y++) {
			for (int x = 0; x < tilePerRows; x++) {
				tiles[y * tilePerRows + x] = terrainTile.GetPixels(x * tileResolution, y * tileResolution, tileResolution, tileResolution);
			}
		}
*/
		//for (int y = tileRows - 1; y >= 0; y--) {
		//gotta go upside down
		for (int y = 0; y < tileRows; y++) {
			for (int x = 0; x < tilePerRows; x++) {
				tiles[(tileRows - 1 - y) * tilePerRows + x] = terrainTile.GetPixels(x * tileResolution, y * tileResolution, tileResolution, tileResolution);
			}
		}

		return tiles;
	}


	void BuildTexture(){
		TDMap map = new TDMap (size_x, size_y);
		map.GenerateForestAsWhole ();

		int textureWidth = size_x * tileResolution;
		int textureHeight = size_y * tileResolution;
		Texture2D texture = new Texture2D(textureWidth, textureHeight);

		Color[][] tiles = ChopUpTiles();
		Color[] paint;
		for (int y = 0; y < size_y; y++) {
			for (int x = 0; x < size_x; x++) {
				paint = tiles[map.GetTileAt(x, y)];
				texture.SetPixels ( x * tileResolution, y * tileResolution, tileResolution, tileResolution, paint);
			}
		}

		texture.filterMode = FilterMode.Point;
		texture.wrapMode = TextureWrapMode.Clamp;
		texture.Apply ();
		
		MeshRenderer meshRenderer = GetComponent<MeshRenderer> ();
		//Debug.Log (meshRenderer.sharedMaterials[0]);
		meshRenderer.sharedMaterials[0].mainTexture = texture;
			//materials[0].mainTexture = texture;	

		Debug.Log("Texture Complete!");
	}

	public void BuildMesh () {

		int tileNum = size_x * size_y;	//Number of tiles
		int triangleNum = tileNum * 2;	//Number of triangles, 2 triangles makes a tile

		int vertSize_x = size_x + 1;
		int vertSize_y = size_y + 1;
		int verticeNum = vertSize_x * vertSize_y;

		//Generate mesh data
		Vector3[] vertices = new Vector3[verticeNum];
		Vector3[] normals = new Vector3[verticeNum];
		Vector2[] uv = new Vector2[verticeNum];	

		int[] triangles = new int[ triangleNum * 3 ];	//Points to a series of vertices

		//initiate tile one by one
		for(int y = 0; y < vertSize_y; y++){
			for (int x = 0; x < vertSize_x; x++) {
				//Set Vertices
				vertices[y * vertSize_x + x] = new Vector3(x * tileSize, y * tileSize, 0);
				//Normal line will point up
				normals[y * vertSize_x + x] = Vector3.right;
				//Set UV
				uv[y * vertSize_x + x] = new Vector2((float)x / size_x, (float)y / size_y);
			}
		}
		Debug.Log("Verts complete!");

		//Affects the maping of tiles
		for(int y = 0; y < size_y; y++){
			for (int x = 0; x < size_x; x++) {
				int squareIndex = y * size_x + x;
				int triangleOffset = squareIndex * 6;
				//First Triangle	
				triangles [triangleOffset + 0] = y * vertSize_x + x +				0;
				triangles [triangleOffset + 1] = y * vertSize_x + x + vertSize_x  + 0;	
				triangles [triangleOffset + 2] = y * vertSize_x + x + vertSize_x  + 1;	
				//Second Triangle	
				triangles [triangleOffset + 3] = y * vertSize_x + x + 				0;
				triangles [triangleOffset + 4] = y * vertSize_x + x + vertSize_x  + 1;
				triangles [triangleOffset + 5] = y * vertSize_x + x + 				1;
			}
		}
		Debug.Log("Triangles complete!");

		//Create a Mesh and populate with data
		Mesh mesh = new Mesh ();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.normals = normals;
		mesh.uv = uv;
		//Assign mesh to filter/renderer/collider
		MeshFilter meshFilter = GetComponent<MeshFilter> ();
		MeshCollider meshCollider = GetComponent<MeshCollider>();


		meshFilter.mesh = mesh;
		meshCollider.sharedMesh = mesh;
		Debug.Log("Mesh complete!");

		BuildTexture ();
	}
}
