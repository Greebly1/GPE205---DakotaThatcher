using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

/// <summary>
/// This is a pretty monolithic script, I have lots of plans for refactoring but I probably won't refactor this unless we do more with generation\
/// Instead of creating an empty 2D array then flling it with random tiles,
/// 
/// This generator creates a 2D array containing a single start/spwan tile,
/// Then uses custom genpath objects to create a random direction,
/// The genpaths expand the 2D map array if it needs to be expanded
/// The genpaths move into this new coordinate and place a random tile prefab into this index
/// When a genpath creates a new tile it opens its door in the correct location
/// Additionally, if a genpath moves into a tile that isn't empty it will open its door instead of replacing it with a new tile
/// 
/// This continues until the genpath placed enough tiles.
/// Finally the last tile placed by the first genpath is always an endTile
/// 
/// Now the 2D tile array is filled with tiles (i created a tile struct to encapsulate prefab information with the coordinates and door information)
/// The mapgenerator instantiates these prefabs using the data stored in them to;
/// 1 - Instantiate the tile in the correct world space position (using the x and y coordinate stored in the tile)
/// 2 - Open the correct doors (using a doorstate list stored inside the tile)
/// 
/// 
/// Using this system it is possible to create multiple special generation paths that branch off one another.
/// This system ensures all tiles are reachable and doesn't instantiate level geometry in places the player can't reach
/// Its possible to create custom rooms such as hidden rooms or locked rooms that the door must be destroyed to find
/// 
/// It's also possible to have custom genpaths that will 100% lead to certain rooms. So with enter the gungeon as an example, each dungeon can
/// generate with two chestroom genpaths, that each end with a chestroom.
/// We could also lock the end room behind a locked door and require the player to track down the key from the special genpath that ends in a key room.
/// </summary>



public class MapGenerator : MonoBehaviour
{
    public List<List<Tile>> map;

    public GameObject startPrefab;
    public GameObject tilePrefab;
    public GameObject endPrefab;
    public GameObject emptyTilePrefab;

    public int X_Min;
    public int Y_Min;

    private void Awake()
    {
        map = new List<List<Tile>>();
        List<Tile> column = new List<Tile>();
        map.Add(column);

        Tile startTile = new Tile(startPrefab, 0, 0);

        map[0].Add(startTile);
        X_Min = 0;
        Y_Min = 0;
        
        genPath generation = new genPath(this, new Vector2Int(0,0));

        generation.runPathGen();

        InstantiateMap();
    }

    // Start is called before the first frame update
    void Start()
    {
        //logMap();
    }

    // Instantiates each tile in the 2D map array ONLY EXECUTE THIS METHOD ONCE
    public void InstantiateMap()
    {
        foreach(var column in map)
        {
            foreach(Tile _tile in column)
            {
                if (_tile._prefab != null) { 
                    GameObject temp = Instantiate(_tile._prefab, _tile.getTilePosition(), Quaternion.identity);
                    temp.GetComponent<Room>().setDoors(_tile.doors);
                }
            }
        }
    }



    #region map methods

    /// <summary>
    /// generate a column filled with as many empty tiles as the map's vertical range
    /// </summary>
    public List<Tile> Column(int x)
    {
        List<Tile> _column = new List<Tile>();
        int y = Y_Min;
        foreach (var row in map[X_ToIndex(0)])
        {
            _column.Add(empty(x,y));
            y++;
        }
        return _column;
    }

    public void  tryAddColumn(int x)
    {
        if (!isvalidX(x))
        {
            List<Tile> column = Column(x);
            if (x > X_Max())
            {
                map.Add(column);
                //Debug.Log("Added a new column");
            } else if (x < X_Min)
            {
                map.Insert(0, column);
                X_Min--;
                //Debug.Log("inserted a new column");
            } else
            {
                Debug.Log("Somehow tryAddColumn failed with x of: " + x);
                Debug.Log("X max is: " + X_Max());
                Debug.Log("X min is " + X_Min);
            }
        }
    }

    public void tryAddRow(int y)
    {
        if (!isvalidY(y))
        {
            if ( y > Y_Max())
            {
                addRow(y);
                //Debug.Log("Added a new row");
            } else if ( y < Y_Min)
            {
                insertRow(y);
                Y_Min--;
                //Debug.Log("Inserted a new row");
            } else
            {
                Debug.Log("Somehow tryAddRow failed with y of: " + y);
                Debug.Log("y max is: " + Y_Max());
                Debug.Log("y min is " + Y_Min);
            }
        }
    }

    public void insertRow(int y)
    {
        int x = X_Min;
        foreach(var column in map)
        {
            column.Insert(0, empty(x,y));
        }
    }
     
    public void addRow(int y)
    {
        int x = X_Min;
        foreach (var column in map)
        {
            column.Add(empty(x, y));
        }
    }

    //Whether a given x coordinate exists in the current range of the map
    public bool isvalidX(int x)
    {
        
        if (X_Min <= x && x <= X_Max())
        {
            //Debug.Log("X of: " + x + " is valid");
            return true;
        }
        //Debug.Log("X of: " + x + " is not valid");
        return false;
    }

    //Whether a given y coordinate exists in the current range of the map
    public bool isvalidY(int y)
    {
        if(Y_Min <= y && y <= Y_Max())
        {
           //Debug.Log("Y of: " + y + " is valid");
            return true;
        }
        //Debug.Log("Y of: " + y + " is not valid");
        return false;
    }

    //prints the map to the log for debugging
    public void logMap()
    {
        Debug.Log("Logging the map");
        foreach ( var column in map )
        {
            foreach ( Tile tile in column)
            {
                Debug.Log("(" + tile._x +", " + tile._y +") is a " + tile._prefab);
            }
        }
    }

    #endregion

    public Tile empty(int x, int y) { return new Tile(emptyTilePrefab, x, y); }


    #region macros
    //I know these aren't actually macros but I am using them to make my code more readable
    private int X_Max() { return map.Count + X_Min - 1; }
        private int Y_Max() { return map[X_ToIndex(0)].Count + Y_Min - 1; }
        private int X_Range() { return map.Count; }
        private int Y_Range() { return map[X_ToIndex(0)].Count; }

        // I am using integers to represent cardinal directions. 1 = UP, 2 = RIGHT, 3 = DOWN, 4 = LEFT
        // I know it would be more intuitive to use enums for this, however this is how I planned it out on a
        // Whiteboard so this is how I implemented it.
        static public Vector2Int UP() { return new Vector2Int(0, -1); }
        static public Vector2Int DOWN() { return new Vector2Int(0, 1);  }
        static public Vector2Int LEFT() { return new Vector2Int(-1, 0); }
        static public Vector2Int RIGHT() { return new Vector2Int(1, 0); }
        static public Vector2Int dirFromInt(int direction) {
            switch(direction){
                case 1: return UP();
                case 2: return RIGHT();
                case 3: return DOWN();
                case 4: return LEFT();
            }
            Debug.LogWarning("Direction from int failed to get a direction, int passed: " + direction);
            return UP();
        }
        static public int intFromDir(Vector2Int direction)
        {
            int x = direction.x;
            int y = direction.y;
            switch (x)
            {
                case -1: return 4;
                case 0:
                    switch (y)
                    {
                        case -1: return 3;
                        case 1: return 1;
                    }
                    Debug.LogWarning("IntFromDir failed, x = " + x + " y = " + y);
                    return 1;
                case 1: return 2;
            }
            Debug.LogWarning("IntFromDir failed, x = " + x + " y = " + y);
            return 1;
        }

        static public int flipDirection (int direction)
    {
        switch (direction)
        {
            case 1: return 3;
            case 2: return 4;
            case 3: return 1;
            case 4: return 2;
        }
        Debug.LogWarning("Flipdirection failed with direction " + direction);
        return 1;
    }

        // There are two types of xy value pairs.
        // Coordinates which represent a possibly negative coordinate on a coordinate plane
        // Indexes which represent a position in the 2D map array
        public int X_ToIndex(int x) { return x + Mathf.Abs(X_Min); }
        public int X_ToCoords(int x) { return x - Mathf.Abs(X_Min); }
        public int Y_ToIndex(int y) { return y + Mathf.Abs(Y_Min); }
        public int Y_ToCoords(int y) { return y - Mathf.Abs(Y_Min); }
        public Vector2Int coordsToIndex(Vector2Int coords)
        {
            int xIndex = X_ToIndex( coords.x);
            int yIndex = Y_ToIndex( coords.y);
            return new Vector2Int(xIndex, yIndex);
        }
        public Vector2Int indexToCoords(Vector2Int index)
        {
            int xIndex = X_ToCoords(index.x);
            int yIndex = Y_ToCoords(index.y);
            return new Vector2Int(xIndex, yIndex);
        }
    #endregion
}

public class genPath
{
    MapGenerator _generator;
    Vector2Int coordinates;
    int lastDirection;
    int maxRandomChecks = 5;
    bool inNewTile = false;

    public genPath (MapGenerator generator, Vector2Int startCoordinates)
    {
        coordinates = startCoordinates;
        _generator = generator;
    }

    private Vector2Int randomDirection()
    {
        int randDir = Random.Range(1, 5);
        for (int randomGenerations = 1; randDir == lastDirection && randomGenerations < maxRandomChecks; randomGenerations++)
        {
            randDir = Random.Range(1, 5);
        }

        if (randDir == lastDirection)
        {
            if (randDir == 1) { randDir++; } else { randDir--; }
        }

        return MapGenerator.dirFromInt(randDir);
     }

    private void move(Vector2Int direction)
    {
        _generator.map[_generator.X_ToIndex(coordinates.x)][_generator.Y_ToIndex(coordinates.y)].setDoorState(MapGenerator.intFromDir(direction), DoorState.open);

        
        coordinates  += direction;
        lastDirection = MapGenerator.intFromDir(direction);

        _generator.tryAddColumn(coordinates.x);
        _generator.tryAddRow(coordinates.y);

        //_generator.logMap();
        Vector2Int mapIndex = _generator.coordsToIndex(coordinates);


        if (_generator.map[mapIndex.x][mapIndex.y]._prefab == _generator.emptyTilePrefab)
        {
            newTile(mapIndex);
            _generator.map[_generator.X_ToIndex(coordinates.x)][_generator.Y_ToIndex(coordinates.y)].setDoorState(MapGenerator.flipDirection(MapGenerator.intFromDir(direction)), DoorState.open);
        }
        else
        {
            _generator.map[_generator.X_ToIndex(coordinates.x)][_generator.Y_ToIndex(coordinates.y)].setDoorState(MapGenerator.flipDirection(MapGenerator.intFromDir(direction)), DoorState.open);

            inNewTile = false;
        }
    }

    private void newTile(Vector2Int index)
    {
        Tile basetile = new Tile(_generator.tilePrefab, coordinates.x, coordinates.y);
        _generator.map[index.x][index.y] = basetile;
        inNewTile = true;
    }

    public void runPathGen()
    {
        //yea this is pretty ugly but it works and I'm running out of time. I'll most likely refactor this later
        move(randomDirection());
        move(randomDirection());
        move(randomDirection());
        move(randomDirection());
        move(randomDirection());
        move(randomDirection());
        move(randomDirection());
        move(randomDirection());
        move(randomDirection());
        move(randomDirection());
        move(randomDirection());
        move(randomDirection());
        move(randomDirection());
        move(randomDirection());
        move(randomDirection());
        move(randomDirection());
    }
}

/// <summary>
/// A tile contains, a prefab for a gameobject with a room component.
/// It also contains a x and y coordinate.
/// and it contains directional information for which doors to open. The array index+1 represents a cardinal direction
/// </summary>
public struct Tile
{
    public GameObject _prefab { get; }
    public int _x { get; }
    public int _y {  get; }
    public DoorState[] doors;

    public Tile (GameObject prefab, int x, int y)
    {
        _prefab = prefab;
        _x = x;
        _y = y;
        doors = new DoorState[4];
    }

    public Vector3 getTilePosition() { return new Vector3(_x * 50 ,0,_y * 50 ); }
    public void setDoorState(int doorDirection, DoorState state)
    {
        doors[doorDirection-1] = state;
    }
}

/// <summary>
/// A state that a door can be in
/// </summary>
public enum DoorState { closed, open, secret }