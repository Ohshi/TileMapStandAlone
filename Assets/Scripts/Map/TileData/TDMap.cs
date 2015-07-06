using UnityEngine;	//Needed for Random at line ~141

public struct Coord{
	int x_coord;
	int y_coord;
}

public class TDMap {
	//Constants derived from TDTile
	//private const int WATER = (int)TDTile.tileType.WATER;
	//private const int DIRT = (int)TDTile.tileType.DIRT;
	//private const int GRASS = (int)TDTile.tileType.GRASS;
	//private const int TREE = (int)TDTile.tileType.TREE;
	//==========================================================================
	//Constants, Variable are located at the bottom of this script
	//==========================================================================
	private const int WATER 		= 0;
	private const int EMPTY		 	= 1;
	private const int GRASS 		= 2;
	private const int TREE  		= 3;
	private const int LAVA  		= 4;
	private const int TLDIRT	 	= 5;
	private const int TMDIRT 		= 6;
	private const int TRDIRT		= 7;
	private const int WALL	 		= 8;
	private const int LMDIRT 		= 9;
	private const int CMDIRT 		= 10;
	private const int RMDIRT 		= 11;
	private const int CHECK 		= 12;
	private const int BLDIRT 		= 13;
	private const int BMDIRT 		= 14;
	private const int BRDIRT 		= 15;
	//==========================================================================
	//Constructor, Get, and Set Functions
	//==========================================================================
	//Constructer
	public TDMap(int size_x, int size_y){
		_size_x = size_x;
		_size_y = size_y;
		_mapData = new int[_size_x, _size_y];
		//For checking purposes, set all tiles to empty incase something misses
		for (int y = 0; y < _size_y; y++)
			for (int x = 0; x < _size_x; x++)
				_mapData [x, y] = 1;
	}
	//Gets an individual tile
	public int GetTileAt(int x_coord, int y_coord){
		if (x_coord < 0 || x_coord >= _size_x || y_coord < 0 || y_coord >= _size_y)
			return 0;
		else
			return _mapData[x_coord, y_coord];
	}
	//Gets the map data as int for analysis 
	public int[,] GetMapDataAsInt(){
		int[,] mapData = new int[_size_x, _size_y];
		for (int y = 0; y < _size_y; y++)
			for (int x = 0; x < _size_x; x++)
				mapData [x, y] = _mapData [x, y];
		return mapData;
	}
	//Sets the map data with an array of int
	public void SetMapData(int[,] mapData){
		_size_x = mapData.GetLength (0);
		_size_y = mapData.GetLength (1);
		_mapData = new int[_size_x, _size_y];
		for (int y = 0; y < _size_y; y++)
			for (int x = 0; x < _size_x; x++)
				_mapData[x, y] = mapData[x, y];
	}
	//==========================================================================
	//Insert different types of maps here
	//==========================================================================
	public void GenerateForestByChunks(){
		int[,] chunkData;
		chunkData = new int[MAP_CONSTANT.CHUNK_WIDTH, MAP_CONSTANT.CHUNK_HEIGHT];
		RingGenerator (_mapData, TREE, 0);
		FillMap (GRASS);
		GenerateChunkInSpiral (chunkData, TREE, 40, 0);
	}
	public void GenerateForestAsWhole(){
		FillMap (GRASS);
		RingGenerator (_mapData, TREE, 0);
		SpiralGeneration (_mapData, TREE, 40, 0);
		ReserveForDoodad (_mapData, GRASS, 'd', 2 * 2 + 2 * 2, 2, 2);
		ReserveForDoodad (_mapData, GRASS, 'd', 2 * 3 + 2 * 3, 3, 3);
		Fill_Isolated (_mapData, TREE);
		
	}
	/*
	//Chunk Method
	public void GenerateByChunks(){
		int[,] chunkData;
		chunkData = new int[MAP_CONSTANT.CHUNK_WIDTH, MAP_CONSTANT.CHUNK_HEIGHT];
		FillMap (GRASS);
		SpiralChunkGeneration (chunkData);
	}
	*/
	//==========================================================================
	//Functions taken from MapGenerator; General Tool Functions
	//==========================================================================
	//Fill the map of a single type of tile
	private void FillMap(int fill){
		//For every point in the map,
		for (int y = 0; y < _size_y; y++){
			for (int x = 0; x < _size_x; x++)
				//that point will equal to the designated fill.
				_mapData[x, y] = fill;
		}
	}
	//Ring generator starting from the top-left and then generates clockwise
	private void RingGenerator(int[,] area, int fill, int increment){
		//This function will use an incrementing system so that we can navagate the map
		//Expand from there by density and build it spirally
		//Top
		for (int x = increment; x < area.GetLength(0) - increment; x++) {
			_mapData [x, increment] = fill;
			_mapData [x, area.GetLength(1) - 1 - increment] = fill;
		}
		//Right
		for (int y = increment; y < area.GetLength(1) - increment; y++) {
			_mapData [increment, y] = fill;
			_mapData [area.GetLength (0) - 1 - increment, y] = fill;
		}
	}
	//Spiral generation starting from the top-left and then generation clockwise
	private void SpiralGeneration(int[,] area, int fill, int kFactor, int cFactor){
		//This function will use an incrementing system so that we can navagate the map
		int increment = 1;
		bool space = true;
		//Expand from there by density and build it spirally
		do
		{
			//Top
			for (int x = increment; x < area.GetLength(0) - 1 - increment; x++)
				GenerateTileByRelation(area, fill, 
				                       x, 			//x-coord
				                       increment, 	//y-coord
				                       kFactor, cFactor);
			//Right
			for (int y = increment; y < area.GetLength(1) - 1 - increment; y++)
				GenerateTileByRelation(area, fill, 
				                       area.GetLength(0) - 1 - increment,	//x-coord
				                       y, 									//y-coord
				                       kFactor, cFactor);
			//Bottom
			for (int x = area.GetLength(0) - 1 - increment; x >= increment; x--)
				GenerateTileByRelation(area, fill, 
				                       x, 									//x-coord
				                       area.GetLength(1) - 1 - increment, 	//y-coord
				                       kFactor, cFactor);
			//Left
			for (int y = area.GetLength(1) - 1 - increment - 1; y >= increment + 1; y--)
				GenerateTileByRelation(area, fill, 
				                       increment,	//x-coord
				                       y, 			//y-coord
				                       kFactor, cFactor);
			//Increment or stop?
			if (2 * increment > area.GetLength(0) || 2 * increment > area.GetLength(1))
				space = false;
			else
				increment++;
		} while (space);
	}
	//Based on the tiles around the selected point, a fill will take its place
	private void GenerateTileByRelation( int[,] area, int fill, int x_coord, int y_coord, int kFactor, int cFactor){
		//(Modifier x Neighboring Trees) / 100 
		if (((kFactor * DetectSimilarBorderTiles(area, fill, x_coord, y_coord)) + cFactor) > (int)Random.Range(0,100) &&
		    x_coord > 0 && x_coord < area.GetLength (0) - 1 && y_coord > 0 && y_coord < area.GetLength (1) - 1)
			area[x_coord, y_coord] = fill;
	}
	//Reserved a plot of land for example a larger tree or a building or a bandit camp
	private void ReserveForDoodad(int[,] area, int clear, char fill, int accpetedClearance, int size_x, int size_y){
		int x_coord = 0;
		int y_coord = 0;
		int perimeter = 2 * size_x + 2 * size_y;
		bool clearance = false;
		if(clear <= perimeter){
			do{
				x_coord = (int)Random.Range (0, _mapData.GetLength(0) - size_x);
				y_coord = (int)Random.Range (0, _mapData.GetLength(1) - size_y);
				if(ScanEdge(area, clear, size_x, size_y, x_coord, y_coord) >= accpetedClearance)
					clearance = true;
			}while(!clearance);
			if(clearance){
				//Check
				PlaceDoodadHolder (area, size_x, size_y, x_coord, y_coord);
				switch(fill){
				case 'd':
					_mapData[x_coord, y_coord] = BLDIRT;
					_mapData[x_coord + size_x - 1, y_coord] = BRDIRT;
					_mapData[x_coord, y_coord + size_y - 1] = TLDIRT;
					_mapData[x_coord + size_x - 1, y_coord + size_y - 1] = TRDIRT;
					for (int y = y_coord + 1; y < y_coord + size_y - 1; y++){
						area[x_coord, y] = LMDIRT;
						area[x_coord + size_x - 1 , y] = RMDIRT;
					}
					for (int x = x_coord + 1; x < x_coord + size_x - 1; x++){
						area[x, y_coord] = BMDIRT;
						area[x, y_coord + size_y - 1] = TMDIRT;
					}
					for (int y = y_coord + 1; y < y_coord + size_y - 1; y++)
						for (int x = x_coord +1; x < x_coord + size_x - 1; x++)
							area[x, y] = CMDIRT;
					break;
				default:
					break;
					
				}
			}
		}
	}
	//Reserved a plot of land for example a larger tree or a building or a bandit camp
	private void PlaceDoodadHolder(int[,] area, int size_x, int size_y, int x_coord, int y_coord){
		for (int y = y_coord; y < y_coord + size_y; y++)
			for (int x = x_coord; x < x_coord + size_x; x++)
				area[x, y] = EMPTY;
	}
	//Fills in points with 3 or more bording tiles
	//An attempt to remove unreachable area
	private void Fill_Isolated(int[,] area, int fill)
	{
		for (int x = 1; x < area.GetLength(0) - 1; x++)
			for (int y = 1; y < area.GetLength(1) - 1; y++)
				if (DetectSimilarBorderTiles(area, fill, x, y) >= 3)
					area[x, y] = fill;
		
		for (int x = area.GetLength(0) - 2; x > 0; x--)
			for (int y = area.GetLength(1) - 2; y > 0; y--)
				if (DetectSimilarBorderTiles(area, fill, x, y) >= 3)
					area[x, y] = fill;
	}
	//Detect if there is a similar tile next to the bordering tile
	private int DetectSimilarBorderTiles(int[,] area, int fill, int x_coord, int y_coord){
		int count = 0;
		//Check if we are at the edge of the map, since taht would break the program
		if (x_coord > 0 && x_coord < area.GetLength (0) - 1 && y_coord > 0 && y_coord < area.GetLength (1) - 1) {
			//Directly touching the point
			if (area [x_coord - 1, y_coord + 0] == fill)
				count++;
			if (area [x_coord + 0, y_coord - 1] == fill)
				count++;
			if (area [x_coord + 1, y_coord + 0] == fill)
				count++;
			if (area [x_coord + 0, y_coord + 1] == fill)
				count++;
			/* 
			//Touching at the border
			if (map[x_coord - 1, y_coord - 1] == fill)
				count++;
			if (map[x_coord + 1, y_coord - 1] == fill)
				count++;
			if (map[x_coord - 1, y_coord + 1] == fill)
				count++;
			if (map[x_coord + 1, y_coord + 1] == fill)
				count++;
			*/
		}
		return count;
	}
	//Scan the edges to see how clear it is
	private int ScanEdge(int[,] area, int clear, int size_x, int size_y, int x_coord, int y_coord){
		int count = 0;
		for (int x = x_coord; x < x_coord + size_x; x++) {
			if (y_coord - 1 > 0)
				if(_mapData [x, y_coord - 1] == clear)
					count++;
			if (y_coord + size_y + 1 < area.GetLength (1) - 1)
				if(_mapData [x, y_coord + size_y + 1] == clear)
					count++;
		}
		//Right
		for (int y = y_coord; y < y_coord + size_y; y++) {
			if (x_coord - 1 > 0)
				if(_mapData [x_coord - 1, y] == clear)
					count++;
			if (x_coord + size_x + 1 < area.GetLength (0) - 1)
				if(_mapData [x_coord + size_x + 1, y_coord + 1] == clear)
					count++;
		}
		return count;
	}
	
	
	
	
	//==========================================================================
	//Specialized Tool Functions
	//==========================================================================
	//Spiral around the map by chunks and develop within those chunks
	private void GenerateChunkInSpiral(int[,] area, int fill, int kFactor, int cFactor){
		//map_x and map_y represent the dimension of grids
		int map_x = _size_x / MAP_CONSTANT.CHUNK_WIDTH;
		int map_y = _size_y / MAP_CONSTANT.CHUNK_HEIGHT;
		int increment = 0;
		bool space = true;
		//Expand from there by density and build it spirally
		do
		{
			//Top
			for (int x = increment; x < map_x - 1 - increment; x++)
				GenerateChunk(area, fill, 
				              x * MAP_CONSTANT.CHUNK_WIDTH, 			//x-coord
				              increment * MAP_CONSTANT.CHUNK_HEIGHT, 	//y-coord
				              kFactor, cFactor);
			//Right
			for (int y = increment; y < map_y - 1 - increment; y++)
				GenerateChunk(area, fill, 
				              (map_x - 1 - increment) * MAP_CONSTANT.CHUNK_WIDTH,	//x-coord
				              y * MAP_CONSTANT.CHUNK_HEIGHT, 						//y-coord
				              kFactor, cFactor);
			//Bottom
			for (int x = map_x - 1 - increment; x >= increment; x--)
				GenerateChunk(area, fill, 
				              x * MAP_CONSTANT.CHUNK_WIDTH, 						//x-coord
				              (map_y - 1 - increment) * MAP_CONSTANT.CHUNK_HEIGHT, 	//y-coord
				              kFactor, cFactor);
			//Left
			for (int y = map_y - 1 - increment - 1; y >= increment + 1; y--)
				GenerateChunk(area, fill, 
				              increment * MAP_CONSTANT.CHUNK_WIDTH,	//x-coord
				              y * MAP_CONSTANT.CHUNK_HEIGHT, 		//y-coord
				              kFactor, cFactor);
			//Increment or stop?
			if (2 * increment >= map_x || 2 * increment >= map_y)
				space = false;
			else
				increment++;
		} while (space);
	}
	//Individual chunk is developed starting from the top-left
	private void GenerateChunk(int[,] area, int fill, int x_coord, int y_coord, int kFactor, int cFactor){
		//Start with the outer edge
		for (int y = y_coord; y < y_coord + MAP_CONSTANT.CHUNK_HEIGHT; y++)
			for (int x = x_coord; x < x_coord + MAP_CONSTANT.CHUNK_WIDTH; x++)
				//Set the chunk to be a part of the map data
				area[x - x_coord, y - y_coord] = _mapData[x, y];
		//What are we going to do in this chuck?
		//A: Spiral generate the fill and then fill in the isolate areas
		SpiralGeneration (area, fill, kFactor, cFactor); 
		Fill_Isolated (area, fill);
		//Set the chunk back into the map data
		for (int y = y_coord; y < y_coord + MAP_CONSTANT.CHUNK_HEIGHT; y++) 
			for (int x = x_coord; x < x_coord + MAP_CONSTANT.CHUNK_WIDTH; x++) 
				_mapData[x, y] = area[x - x_coord, y - y_coord];
	}
	
	//==========================================================================
	//Variables
	//==========================================================================
	private int[,] _mapData;
	private int _size_x;
	private int _size_y;
}
