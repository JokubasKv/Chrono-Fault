using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerController))]
public class TimetravelController : MonoBehaviour
{
    Vector2Int offset;
    MapData pastMap;
    MapData futureMap;


    private TimePhase currentTime =TimePhase.Past;
    PlayerInput inputActions;
    #region -Awake/OnEnable/OnDisable -
    private void Awake()
    {
        inputActions = new PlayerInput();
    }
    private void OnEnable()
    {
        
        inputActions.Gameplay.TimeTravel.performed += e => Timetravel();

        inputActions.Enable();
    }
    private void OnDisable()
    {
        inputActions.Gameplay.TimeTravel.performed -= e => Timetravel();

        inputActions.Disable();
    }
    #endregion

    public void Timetravel()
    {
        DualMapData dualMapData = FindObjectOfType<DualMapData>();
        offset = dualMapData.offset;
        pastMap = dualMapData.PastMapData;
        futureMap = dualMapData.FutureMapData;

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
        StartCoroutine(MovePlayer(newPosition, 0.5f));

    }

    IEnumerator MovePlayer(Vector2Int newPosition, float delayTime)
    {
        UIManagerSingleton.Instance.TimeTravelFlashOnce(delayTime * 2);
        yield return new WaitForSeconds(delayTime);
        transform.position = new Vector3(newPosition.x, newPosition.y) + new Vector3(0.5f, 0.5f);

    }

    public Vector2Int GetNearestCoordinates(MapData mapData, Vector2Int targetPoint)
    {
        List<Vector2Int> coordinates = new List<Vector2Int>(mapData.AllFloorTiles);
        //Debug.Log(string.Join(", ", coordinates.Select(v => v.ToString())));

        Dictionary<Vector2Int, float> distances = new Dictionary<Vector2Int, float>();


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