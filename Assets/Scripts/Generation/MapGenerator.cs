using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public List<List<GameObject>> map;

    public GameObject startPrefab;
    public GameObject tilePrefab;
    public GameObject emptyTilePrefab;

    public int X_Min;
    public int Y_Min;

    private void Awake()
    {
        map = new List<List<GameObject>>();
        map[0][0] = startPrefab;
        X_Min = 0;
        Y_Min = 0;
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public List<GameObject> Column(int x)
    {
        List<GameObject> _column = new List<GameObject>();
        for (int i = Y_Min; i < Y_Max(); i++)
        {
            _column.Add(emptyTilePrefab);
        }
        return _column;
    }

    public void  tryAddColumn(int x)
    {
        if (!isvalidX(x))
        {
            List<GameObject> column = Column(x);
            if (x > X_Max())
            {
                map.Add(column);
            } else if (x < X_Min)
            {
                map.Insert(0, column);
                X_Min--;
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
            } else if ( y < Y_Min)
            {
                insertRow(y);
                Y_Min--;
            }
        }
    }

    public void insertRow(int y)
    {
        for (int i = Y_Min; i < Y_Max(); i++)
        {
            map[i].Insert(0, emptyTilePrefab);
        }
    }
     
    public void addRow(int y)
    {
        for (int i = X_Min; i < X_Max(); i++)
        {
            map[i].Add(emptyTilePrefab);
        }
    }

    //Whether a given x coordinate exists in the current range of the map
    public bool isvalidX(int x)
    {
        if (X_Min < x || x < X_Max())
        {
            return true;
        }
        return false;
    }

    //Whether a given y coordinate exists in the current range of the map
     public bool isvalidY(int y)
    {
        if(Y_Min < y || y < Y_Max())
        {
            return true;
        }
        return false;
    }


    #region macros
        private int X_Max() { return map.Count + X_Min - 1; }
        private int Y_Max() { return map[X_ToIndex(0)].Count + Y_Min - 1; }
        private int X_Range() { return map.Count; }
        private int Y_Range() { return map[X_ToIndex(0)].Count; }
        static private Vector2Int UP() { return new Vector2Int(0, -1); }
        static private Vector2Int DOWN() { return new Vector2Int(0, 1);  }
        static private Vector2Int LEFT() { return new Vector2Int(-1, 0); }
        static private Vector2Int RIGHT() { return new Vector2Int(1, 0); }
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

public class genThread
{
    MapGenerator generator;
    Vector2Int coordinates;
    int lastDirection;
    int maxRandomChecks = 5;


    private Vector2Int randomDirection(Vector2Int currentDir)
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
        coordinates += direction;
        generator.tryAddColumn(coordinates.x);
        generator.tryAddRow(coordinates.y);

        if (generator.map[coordinates.x][coordinates.y] == generator.emptyTilePrefab)
        {
            generator.map[coordinates.x][coordinates.y] = generator.tilePrefab;
        }
    }
}