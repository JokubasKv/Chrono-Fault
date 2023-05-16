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

    [SerializeField] MapData mapData;

    public void PlaceAgents()
    {
        if (mapData == null)
            return;

        minRoomEnemyCount = LevelsManager.instance.MinEnemies;
        maxRoomEnemyCount = LevelsManager.instance.MaxEnemies;

        for (int i = 0; i < mapData.Rooms.Count; i++)
        {
            switch (mapData.Rooms[i].RoomType)
            {
                case RoomTypes.Starting:
                    Debug.Log("Bruh");
                    if (playerPrefab)
                    {
                        GameObject player = GameObject.FindGameObjectWithTag("Player");
                        if (player == null)
                        {
                            player = Instantiate(playerPrefab);
                        }
                        player.transform.localPosition = mapData.Rooms[i].RoomCenterPos + Vector2.one * 0.5f;
                    }
                    break;

                case RoomTypes.Normal:
                    Room room = mapData.Rooms[i];
                    TileGraph roomGraph = new TileGraph(room.FloorTiles);

                    HashSet<Vector2Int> roomFloor = new HashSet<Vector2Int>(room.FloorTiles);
                    roomFloor.IntersectWith(mapData.Paths.SelectMany(floors => floors.FloorTiles).ToHashSet());

                    Dictionary<Vector2Int, Vector2Int> roomMap = roomGraph.GetReachableTilesBFS(roomFloor.First(), room.PropPositions);
                    room.PositionsAccessibleFromPath = roomMap.Keys.OrderBy(x => Random.Range(0f, 6f)).ToList();

                    PlaceEnemies(room, Random.Range(minRoomEnemyCount,maxRoomEnemyCount));
                    
                    break;
            }
        }
    }

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

        Queue<Vector2Int> tilesToVisit = new Queue<Vector2Int>();
        tilesToVisit.Enqueue(startPos);

        HashSet<Vector2Int> visitedTiles = new HashSet<Vector2Int>();
        visitedTiles.Add(startPos);

        Dictionary<Vector2Int, int> graphPositionWeight = new Dictionary<Vector2Int, int>();
        graphPositionWeight.Add(startPos, 0);

        while (tilesToVisit.Count > 0)
        {
            Vector2Int node = tilesToVisit.Dequeue();
            List<Vector2Int> neighbours = graph[node];
            int currentWeight = graphPositionWeight[node];

            foreach (Vector2Int neighbourPosition in neighbours)
            {
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
    public Dictionary<Vector2Int, Vector2Int> GetReachableTilesBFS(Vector2Int startPos, HashSet<Vector2Int> occupiedTiles)
    {
        Queue<Vector2Int> nodesToVisit = new Queue<Vector2Int>();
        nodesToVisit.Enqueue(startPos);

        HashSet<Vector2Int> visitedNodes = new HashSet<Vector2Int>
        {
            startPos
        };

        //The dictionary that we will return 
        Dictionary<Vector2Int, Vector2Int> map = new Dictionary<Vector2Int, Vector2Int>
        {
            { startPos, startPos }
        };


        while (nodesToVisit.Count > 0)
        {
            Vector2Int node = nodesToVisit.Dequeue();
            List<Vector2Int> neighbours = graph[node];

            //loop through each neighbour position
            foreach (Vector2Int neighbourPosition in neighbours)
            {
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
