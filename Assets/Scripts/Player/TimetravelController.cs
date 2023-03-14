using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerController))]
public class TimetravelController : MonoBehaviour
{
    [SerializeField] public Vector2Int offset;

    [SerializeField] public MapData pastMap;
    [SerializeField] public MapData futureMap;


    private TimePhase currentTime;
    PlayerInput inputActions;
    #region -Awake/OnEnable/OnDisable -
    private void OnEnable()
    {
        inputActions = new PlayerInput();
        inputActions.Gameplay.TimeTravel.performed += e => Timetravel();

        inputActions.Enable();
    }
    private void OnDisable()
    {
        inputActions = new PlayerInput();
        inputActions.Gameplay.TimeTravel.performed -= e => Timetravel();

        inputActions.Disable();
    }
    #endregion


    private void Start()
    {
        currentTime = TimePhase.Past;
    }

    public void Timetravel()
    {
        Vector2Int newPosition = new Vector2Int();
        Vector2Int currentPosition = new Vector2Int((int)transform.position.x, (int)transform.position.y);
        switch (currentTime)
        {
            case TimePhase.Past:
                newPosition =  GetNearestCoordinates(futureMap, currentPosition+offset);
                currentTime = TimePhase.Future;
                break;
            case TimePhase.Future:
                newPosition = GetNearestCoordinates(pastMap, currentPosition - offset);
                currentTime = TimePhase.Past;
                break;
        }
        transform.position = new Vector3(newPosition.x, newPosition.y) + new Vector3(0.5f,0.5f);
    }

    public Vector2Int GetNearestCoordinates(MapData mapData, Vector2Int targetPoint)
    {
        HashSet<Vector2Int> allFloorTiles = new HashSet<Vector2Int>();

        // Add all the floor tiles from each room
        foreach (Room room in mapData.Rooms)
        {
            allFloorTiles.UnionWith(room.FloorTiles);
        }

        // Add all the floor tiles from each path
        foreach (Path path in mapData.Paths)
        {
            allFloorTiles.UnionWith(path.FloorTiles);
        }
        List<Vector2Int> coordinates = new List<Vector2Int>(allFloorTiles);

        Dictionary<Vector2Int, float> distances = new Dictionary<Vector2Int, float>();

        // Calculate distances between each coordinate and the target point
        foreach (Vector2Int coordinate in coordinates)
        {
            float distance = Vector2Int.Distance(coordinate, targetPoint);
            distances.Add(coordinate, distance);
        }

        // Sort coordinates by distance
        var sortedDistances = distances.OrderBy(x => x.Value);

        return sortedDistances.Select(v => v.Key).First();
    }
}

public enum TimePhase
{
    Past,
    Future
}