using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class AgentPlacer : MonoBehaviour
{
    [SerializeField]
    private GameObject enemyPrefab, playerPrefab;


    [SerializeField] public int minRoomEnemyCount;
    [SerializeField] public int maxRoomEnemyCount;

    [SerializeField] MapData dungeonData;

    [SerializeField]
    private bool showGizmo = false;

    public void PlaceAgents()
    {
        if (dungeonData == null)
            return;

        minRoomEnemyCount = LevelsManager.Instance.MinEnemies;
        maxRoomEnemyCount = LevelsManager.Instance.MaxEnemies;
        //Loop for each room
        for (int i = 0; i < dungeonData.Rooms.Count; i++)
        {
            switch (dungeonData.Rooms[i].RoomType)
            {
                case RoomTypes.Starting:
                    if (playerPrefab)
                    {
                        GameObject player = GameObject.FindGameObjectWithTag("Player");
                        if (player == null)
                        {
                            player = Instantiate(playerPrefab);
                        }
                        player.transform.localPosition = dungeonData.Rooms[i].RoomCenterPos + Vector2.one * 0.5f;
                    }
                    break;

                case RoomTypes.Normal:
                    //TO place eneies we need to analyze the room tiles to find those accesible from the path
                    Room room = dungeonData.Rooms[i];
                    TileGraph roomGraph = new TileGraph(room.FloorTiles);

                    //Find the Path inside this specific room
                    HashSet<Vector2Int> roomFloor = new HashSet<Vector2Int>(room.FloorTiles);
                    //Find the tiles belonging to both the path and the room
                    roomFloor.IntersectWith(dungeonData.Paths.SelectMany(floors => floors.FloorTiles).ToHashSet());

                    //Run the BFS to find all the tiles in the room accessible from the path
                    Dictionary<Vector2Int, Vector2Int> roomMap = roomGraph.GetReachableTilesBFS(roomFloor.First(), room.PropPositions);

                    //Positions that we can reach + path == positions where we can place enemies
                    room.PositionsAccessibleFromPath = roomMap.Keys.OrderBy(x => Random.Range(0f, 6f)).ToList();

                    PlaceEnemies(room, Random.Range(minRoomEnemyCount,maxRoomEnemyCount));
                    
                    break;
            }
        }
    }

    /// <summary>
    /// Places enemies in the positions accessible from the path
    /// </summary>
    /// <param name="room"></param>
    /// <param name="enemysCount"></param>
    private void PlaceEnemies(Room room, int enemysCount)
    {
        for (int k = 0; k < enemysCount; k++)
        {
            if (room.PositionsAccessibleFromPath.Count <= k)
            {
                return;
            }
            GameObject enemy = Instantiate(enemyPrefab);
            enemy.transform.localPosition = (Vector2)room.PositionsAccessibleFromPath[k] + Vector2.one * 0.5f;
            room.EnemiesInTheRoom.Add(enemy);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (dungeonData == null || showGizmo == false)
            return;
        foreach (Room room in dungeonData.Rooms)
        {
            Color color = Color.green;
            color.a = 0.3f;
            Gizmos.color = color;

            foreach (Vector2Int pos in room.PositionsAccessibleFromPath)
            {
                Gizmos.DrawCube((Vector2)pos + Vector2.one * 0.5f, Vector2.one);
            }
        }
    }
}

public class TileGraph
{
    public static List<Vector2Int> fourDirections = new()
    {
        Vector2Int.up,
        Vector2Int.right,
        Vector2Int.down,
        Vector2Int.left
    };

    Dictionary<Vector2Int, List<Vector2Int>> graph = new Dictionary<Vector2Int, List<Vector2Int>>();
    

    public TileGraph(HashSet<Vector2Int> tileFloor)
    {
        foreach (Vector2Int pos in tileFloor)
        {
            List<Vector2Int> neighbours = new List<Vector2Int>();
            foreach (Vector2Int direction in fourDirections)
            {
                Vector2Int newPos = pos + direction;
                if (tileFloor.Contains(newPos))
                {
                    neighbours.Add(newPos);
                }
            }
            graph.Add(pos, neighbours);
        }
    }


    public Dictionary<Vector2Int, int> GetWeightedBFS(Vector2Int startPos, HashSet<Vector2Int> occupiedTiles)
    {
        //BFS related variuables
        Queue<Vector2Int> tilesToVisit = new Queue<Vector2Int>();
        tilesToVisit.Enqueue(startPos);

        HashSet<Vector2Int> visitedTiles = new HashSet<Vector2Int>();
        visitedTiles.Add(startPos);

        Dictionary<Vector2Int, int> graphPositionWeight = new Dictionary<Vector2Int, int>();
        graphPositionWeight.Add(startPos, 0);

        while (tilesToVisit.Count > 0)
        {
            //get the data about specific position
            Vector2Int node = tilesToVisit.Dequeue();
            List<Vector2Int> neighbours = graph[node];
            int currentWeight = graphPositionWeight[node];

            //loop through each neighbour position
            foreach (Vector2Int neighbourPosition in neighbours)
            {
                //add the neighbour position to our map if it is valid
                if (!visitedTiles.Contains(neighbourPosition)  &&
                    !occupiedTiles.Contains(neighbourPosition))
                {
                    tilesToVisit.Enqueue(neighbourPosition);
                    visitedTiles.Add(neighbourPosition);
                    graphPositionWeight[neighbourPosition] = currentWeight+1;
                }
            }
        }

        return graphPositionWeight;
    }
    /// <summary>
    /// Creates a map of reachable tiles in our dungeon.
    /// </summary>
    /// <param name="startPos">Door position or tile position on the path between rooms inside this room</param>
    /// <param name="occupiedTiles"></param>
    /// <returns></returns>
    public Dictionary<Vector2Int, Vector2Int> GetReachableTilesBFS(Vector2Int startPos, HashSet<Vector2Int> occupiedTiles)
    {
        //BFS related variuables
        Queue<Vector2Int> nodesToVisit = new Queue<Vector2Int>();
        nodesToVisit.Enqueue(startPos);

        HashSet<Vector2Int> visitedNodes = new HashSet<Vector2Int>();
        visitedNodes.Add(startPos);

        //The dictionary that we will return 
        Dictionary<Vector2Int, Vector2Int> map = new Dictionary<Vector2Int, Vector2Int>();
        map.Add(startPos, startPos);


        while (nodesToVisit.Count > 0)
        {
            //get the data about specific position
            Vector2Int node = nodesToVisit.Dequeue();
            List<Vector2Int> neighbours = graph[node];

            //loop through each neighbour position
            foreach (Vector2Int neighbourPosition in neighbours)
            {
                //add the neighbour position to our map if it is valid
                if (visitedNodes.Contains(neighbourPosition) == false &&
                    occupiedTiles.Contains(neighbourPosition) == false)
                {
                    nodesToVisit.Enqueue(neighbourPosition);
                    visitedNodes.Add(neighbourPosition);
                    map[neighbourPosition] = node;
                }
            }
        }

        return map;
    }
}
