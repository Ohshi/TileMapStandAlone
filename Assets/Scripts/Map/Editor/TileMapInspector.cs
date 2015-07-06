using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(TileMap))]
public class TileMapInspector : Editor {
	//==========================================================================
	//Constants
	//==========================================================================

	public override void OnInspectorGUI() {
		DrawDefaultInspector();
		if(GUILayout.Button("Generate Small Map")) {
			TileMap tileMap = (TileMap)target;
			tileMap.size_x = MAP_CONSTANT.SML_WIDTH * MAP_CONSTANT.CHUNK_WIDTH;
			tileMap.size_y = MAP_CONSTANT.SML_HEIGHT * MAP_CONSTANT.CHUNK_HEIGHT;
			tileMap.BuildMesh();

		}if(GUILayout.Button("Generate Medium Map")) {
			TileMap tileMap = (TileMap)target;
			tileMap.size_x = MAP_CONSTANT.LRG_WIDTH * MAP_CONSTANT.CHUNK_WIDTH;
			tileMap.size_y = MAP_CONSTANT.SML_HEIGHT * MAP_CONSTANT.CHUNK_HEIGHT;
			tileMap.BuildMesh();

		}if(GUILayout.Button("Generate Large Map")) {
			TileMap tileMap = (TileMap)target;
			tileMap.size_x = MAP_CONSTANT.LRG_WIDTH * MAP_CONSTANT.CHUNK_WIDTH;
			tileMap.size_y = MAP_CONSTANT.LRG_HEIGHT * MAP_CONSTANT.CHUNK_HEIGHT;
			tileMap.BuildMesh();
		}

		if(GUILayout.Button("Regenerate Map")) {
			TileMap tileMap = (TileMap)target;
			tileMap.BuildMesh();
		}
	}
}
