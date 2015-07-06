public class TDTile {
	//Enumerator
	public enum tileType{
		WATER = 0, 
		DIRT, 
		GRASS, 
		TREE
	}
	//Constructor
	public TDTile(int type){ _tileType = (tileType)type; }

	//Get the tile type as int
	public int GetTileTypeAsInt() { return (int)_tileType; }

	//Set the tile using an int
	public void SetTileTypeByInt(int type) { _tileType = (tileType)type; }

	public void SetTileType(TDTile type) { _tileType = type._tileType; }

	//Members
	private tileType _tileType;
}
