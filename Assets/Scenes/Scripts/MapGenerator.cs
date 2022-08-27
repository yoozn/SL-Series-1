using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public Transform tilePrefab;
    public Transform obstaclePrefab;
    public Vector2 mapSize;

    [Range(0,1)]
    public float outlinePercent;

    [Range(0,1)]
    public float obstaclePercent;

    List<Coord> allTileCoords;
    Queue<Coord> shuffledTileCoords;

    public int seed = 10;
    Coord mapCentre;

    private void Start()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {
        //list of all coords (x and y values for all tiles)
        allTileCoords = new List<Coord>();

        //nested for loop to match each tile to a coord
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                //add the coord of the tile matching x and y to the list of all coords
                allTileCoords.Add(new Coord (x, y));
            }
        }
        // Create a new queue from shuffled tiles (pass allTileCoords into ShuffleArray method)
        shuffledTileCoords = new Queue<Coord>(Utility.ShuffleArray(allTileCoords.ToArray(), seed));

        mapCentre = new Coord((int)mapSize.x / 2, (int)mapSize.y / 2);

        //Name of parent is "Generated Map"
        string holderName = "Generated Map";
        //if game object in hierarchy with name "Generated Map" already exists, destroy it, and all of it's children
        if (transform.Find(holderName))
        {
            //Used since this method may be called from while game is running
            DestroyImmediate(transform.Find(holderName).gameObject);
        }

        //Create a new game object 'mapHolder' with name "Generated Map"
        Transform mapHolder = new GameObject(holderName).transform;
        //Set mapHolder's parent to be the 'map' in hierarchy
        mapHolder.parent = transform;

        //Nested for loops to loop through size of x * y values ( grid size )
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                
                Vector3 tilePosition = CoordToPosition(x, y);
                //Instantiate tiles with the tile prefab, the tiles position, and rotated to be flat.
                Transform newTile = Instantiate(tilePrefab, tilePosition, Quaternion.Euler(Vector3.right * 90)) as Transform;
                //Set scale according to outline. 1 - outlinePercent so that it measures the outline scale, not the tile scale.
                newTile.localScale = Vector3.one * (1 - outlinePercent);
                //Set the parent of the tile to be mapHolder object
                newTile.parent = mapHolder;
            }
        }

        bool[,] obstacleMap = new bool[(int)mapSize.x, (int)mapSize.y];

        int obstacleCount = (int)( mapSize.x * mapSize.y * obstaclePercent );
        int currentObstacleCount = 0;
        for (int i = 0; i < obstacleCount; i++)
        {
            //Get the next random coord from the shuffled queue of coords.
            Coord randomCoord = GetRandomCoord();
            obstacleMap[randomCoord.x, randomCoord.y] = true;
            currentObstacleCount++;
            if (randomCoord != mapCentre && MapIsFullyAccessible(obstacleMap, currentObstacleCount))
            {
                //Set a variable for the obstacles position to the x and y values of this random coord.
                Vector3 obstaclePosition = CoordToPosition(randomCoord.x, randomCoord.y);
                //Instantiate a newObstacle and set it's transform to the same value as the assigned random coord.
                Transform newObstacle = Instantiate(obstaclePrefab, obstaclePosition + Vector3.up * .5f, Quaternion.identity) as Transform;
                //Set the parent as mapHolder so it will be deleted instead of instantiated every frame.
                newObstacle.parent = mapHolder;
            }
            else
            {
                obstacleMap[randomCoord.x, randomCoord.y] = false;
                currentObstacleCount--;
            }
            
        }
    }

    bool MapIsFullyAccessible(bool[,] obstacleMap, int currentObstacleCount) 
    {
        bool[,] mapFlags = new bool[obstacleMap.GetLength(0), obstacleMap.GetLength(1)];
        Queue<Coord> queue = new Queue<Coord>();
        queue.Enqueue(mapCentre);
        mapFlags[mapCentre.x, mapCentre.y] = true;

        int accessibleTileCount = 1;

        while (queue.Count > 0)
        {
            Coord tile = queue.Dequeue();

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    int neighborX = tile.x + x;
                    int neighborY = tile.y + y;
                    if (x == 0 || y == 0)
                    {
                        if (neighborX >= 0 && neighborX < obstacleMap.GetLength(0) && neighborY >= 0 && neighborY < obstacleMap.GetLength(1))
                        {
                            if (!mapFlags[neighborX, neighborY] && !obstacleMap[neighborX, neighborY])
                            {
                                mapFlags[neighborX, neighborY] = true;
                                queue.Enqueue(new Coord(neighborX, neighborY));
                                accessibleTileCount++;
                            }
                        }
                    }
                }
            }
        }
        int targetAccessibleTileCount = (int)(mapSize.x * mapSize.y - currentObstacleCount);
        return targetAccessibleTileCount == accessibleTileCount;
    }

    Vector3 CoordToPosition(int x, int y)
    {
        //tile position of each iteration of loop. Set to be centered. First tile is at the negative of the lowest x value, divided by two (since negative and positive values of loop)
        // add .5f since each tile is 1 * 1 unit, and the position is measured from center of the tile.
        //Add x so that the tile is moved over by one unit each new iteration. repeat for the y values.
        return new Vector3(-mapSize.x / 2 + 0.5f + x, 0, -mapSize.y / 2 + 0.5f + y);
    }

    public Coord GetRandomCoord()
    {
        //Get the first value from the queue of random shuffled coords
        Coord randomCoord = shuffledTileCoords.Dequeue();
        //Put the randomCoord value to the back of the queue
        shuffledTileCoords.Enqueue(randomCoord);
        //return the value of coords
        return randomCoord;
    }

    //custom struct, that holds two values, x and y
    public struct Coord
    {
        public int x;
        public int y;

        public Coord(int _x, int _y)
        {
            //method to set x and y
            x = _x;
            y = _y;
        }

        public static bool operator ==(Coord c1, Coord c2)
        {
            return c1.x == c2.x && c1.y == c2.y;
        }
        public static bool operator !=(Coord c1, Coord c2)
        {
            return !(c1 == c2);
        }
    }
}
