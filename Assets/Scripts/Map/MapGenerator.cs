using UnityEngine;
using System.Collections;

public class MapGenerator : MonoBehaviour {
	//Constants derived from TDTile
	private const int GRASS = (int)TDTile.tileType.GRASS;
	private const int TREE = (int)TDTile.tileType.TREE;
	
	//==========================================================================
	//Variables
	//==========================================================================
	private int _size_x;
	private int _size_y;
	private int[,] _mapData;

	//==========================================================================
	//Function
	//==========================================================================
	//Chunk Method
	public int[,] GenerateByChunks(int size_x, int size_y){
		int[,] chunkData;
		chunkData = new int[MAP_CONSTANT.CHUNK_WIDTH, MAP_CONSTANT.CHUNK_HEIGHT];
		_size_x = size_x;
		_size_y = size_y;
		_mapData = new int[_size_x, _size_y];
		FillMap (GRASS);
		SpiralChunkGeneration (chunkData);
		return _mapData;
	}
	//Spiral around the map by chunks and develop within those chunks
	private void SpiralChunkGeneration(int[,] area){
		//map_x and map_y represent the dimension of grids
		int map_x = _size_x / MAP_CONSTANT.CHUNK_WIDTH;
		int map_y = _size_y / MAP_CONSTANT.CHUNK_HEIGHT;
		int increment = 0;
		bool space = true;
		//Expand from there by density and build it spirally
		do
		{
			//Top for(int x = increment; x <= area.GetLength(0) - 1 - increment; x++)
			for (int x = increment; x < map_x - 1 - increment; x++)
				GenerateChunk(area, x * MAP_CONSTANT.CHUNK_WIDTH, increment * MAP_CONSTANT.CHUNK_HEIGHT);
			//Right for (int y = increment + 1; y <= area.GetLength(1) - 1 - increment - 1; y++)
			for (int y = increment; y < map_y - 1 - increment; y++)
				GenerateChunk(area, (map_x - 1 - increment) * MAP_CONSTANT.CHUNK_WIDTH, y * MAP_CONSTANT.CHUNK_HEIGHT);
			//Bottom for (int x = area.GetLength(0) - 1 - increment; x >= increment; x--)
			for (int x = map_x - 1 - increment; x >= increment; x--)
				GenerateChunk(area, x * MAP_CONSTANT.CHUNK_WIDTH, (map_y - 1 - increment) * MAP_CONSTANT.CHUNK_HEIGHT);
			//Left for (int y = area.GetLength(1) - 1 - increment - 1; y >= increment + 1; y--)
			for (int y = map_y - 1 - increment - 1; y >= increment + 1; y--)
				GenerateChunk(area, 	increment * MAP_CONSTANT.CHUNK_WIDTH, y * MAP_CONSTANT.CHUNK_HEIGHT);
			//Increment
			if (2 * increment > map_x || 2 * increment > map_y)
				space = false;
			else
				increment++;
		} while (space);
	}
	//Individual chunk is developed starting from the top-left
	private void GenerateChunk(int[,] area, int x_coord, int y_coord){
		//Start with the outer edge
		for (int y = y_coord; y < y_coord + MAP_CONSTANT.CHUNK_HEIGHT; y++) {
			for (int x = x_coord; x < x_coord + MAP_CONSTANT.CHUNK_WIDTH; x++) {
				//An egde WILL be a tree
				if(x == 0 || x == _size_x - 1 ||
				   y == 0 || y == _size_y - 1 )
					_mapData[x, y] = TREE;
				//Set the chunk to be a part of the map data
				area[x - x_coord, y - y_coord] = _mapData[x, y];
			}
		}

		SpiralGeneration (area, 45, 0); 

		Fill_Isolated (area);

		//Set the chunk back into the map data
		for (int y = y_coord; y < y_coord + MAP_CONSTANT.CHUNK_HEIGHT; y++) 
			for (int x = x_coord; x < x_coord + MAP_CONSTANT.CHUNK_WIDTH; x++) 
				_mapData[x, y] = area[x - x_coord, y - y_coord];
	}
	//Fill the map of a single type of tile
	private void FillMap(int type){
		for (int y = 0; y < _size_y; y++){
			for (int x = 0; x < _size_x; x++)
				_mapData[x, y] = type;
		}
	}
	//Spiral generation starting from the top-left and then generation clockwise
	private void SpiralGeneration(int[,] area, int kFactor, int cFactor){
		int increment = 1;
		bool space = true;
		//Expand from there by density and build it spirally
		do
		{
			//Top
			for (int x = increment; x < area.GetLength(0) - 1 - increment; x++)
				GenerateTileByRelation(area, TREE, x, increment, kFactor, cFactor);
			//Right
			for (int y = increment; y < area.GetLength(1) - 1 - increment; y++)
				GenerateTileByRelation(area, TREE, area.GetLength(0) - 1 - increment, y, kFactor, cFactor);
			//Bottom
			for (int x = area.GetLength(0) - 1 - increment; x >= increment; x--)
				GenerateTileByRelation(area, TREE, x, area.GetLength(1) - 1 - increment, kFactor, cFactor);
			//Left
			for (int y = area.GetLength(1) - 1 - increment - 1; y >= increment + 1; y--)
				GenerateTileByRelation(area, TREE, increment, y, kFactor, cFactor);
			//Stop or increment
			if (2 * increment > area.GetLength(0) || 2 * increment > area.GetLength(1))
				space = false;
			else
				increment++;
		} while (space);
	}


	private void GenerateTileByRelation( int[,] area, int type, int x_coord, int y_coord, int kFactor, int cFactor)
	{
		//(Modifier x Neighboring Trees) / 100 
		if (((kFactor * Detect_Direct_Border(area, type, x_coord, y_coord)) + cFactor) > (int)Random.Range(0,100) &&
		    x_coord > 0 && x_coord < area.GetLength (0) - 1 && y_coord > 0 && y_coord < area.GetLength (1) - 1)
			area[x_coord, y_coord] = type;
	}
	
	private void Fill_Isolated(int[,] area)
	{
		for (int x = 1; x < area.GetLength(0) - 1; x++)
		{
			for (int y = 1; y < area.GetLength(1) - 1; y++)
			{
				if (Detect_Direct_Border(area, TREE, x, y) >= 3)
					area[x, y] = TREE;
			}
		}
		for (int x = area.GetLength(0) - 2; x > 0; x--)
		{
			for (int y = area.GetLength(1) - 2; y > 0; y--)
			{
				if (Detect_Direct_Border(area, TREE, x, y) >= 3)
					area[x, y] = TREE;
			}
		}
	}
	
	private int Detect_Direct_Border(int[,] area, int target, int x_coord, int y_coord)
	{
		int count = 0;
		//Direct border
		//if (x_coord > 0 && x_coord < area.GetLength (0) - 1 && y_coord > 0 && y_coord < area.GetLength (1) - 1) {
			if (area [x_coord - 1, y_coord + 0] == target)
				count++;
			if (area [x_coord + 0, y_coord - 1] == target)
				count++;
			if (area [x_coord + 1, y_coord + 0] == target)
				count++;
			if (area [x_coord + 0, y_coord + 1] == target)
				count++;
		//}
		/* Corners
		if (map[x_coord - 1, y_coord - 1] == target)
			count++;
		if (map[x_coord + 1, y_coord - 1] == target)
			count++;
		if (map[x_coord - 1, y_coord + 1] == target)
			count++;
		if (map[x_coord + 1, y_coord + 1] == target)
			count++;
			*/
		return count;
	}
	//Original Method
	public int[,] GenerateMapData(int size_x, int size_y){
		_size_x = size_x;
		_size_y = size_y;
		_mapData = new int[_size_x, _size_y];
		FillMap(GRASS);
		Generate_Outer_Forest();
		Fill_Isolated(_mapData);
		return _mapData;
	}
	//Original method generation
	private void Generate_Outer_Forest()
	{
		//Start with outer edge
		for (int x = 0; x < _size_x; x++)
		{
			_mapData[x, 0] = TREE;
			_mapData[x, _size_y - 1] = TREE;
			for (int y = 0; y < _size_y; y++)
			{
				_mapData[0, y] = TREE;
				_mapData[_size_x - 1, y] = TREE;
			}
		}
		//Expand from there by density and build it spirally
		SpiralGeneration (_mapData, 40, 0);
	}
}//End of MapGenerator
