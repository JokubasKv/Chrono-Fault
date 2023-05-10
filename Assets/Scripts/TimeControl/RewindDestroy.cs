using System;
using UnityEngine;
using UnityEngine.UI;
public class RewindDestroy : MonoSingleton<RewindDestroy>
{
    [SerializeField] CircularArray<GameObject> trackedDestroyedObjects;

    public bool IsTracking { get; set; } = false;

    protected void OnEnable()
    {
        RewindManager.RewindTimeCall += Rewind;
        RewindManager.TrackingStateCall += OnTrackingChange;
    }
    protected void OnDisable()
    {
        RewindManager.RewindTimeCall -= Rewind;
        RewindManager.TrackingStateCall -= OnTrackingChange;
    }

    private void OnTrackingChange(bool val)
    {
        IsTracking = val;
    }

    protected void FixedUpdate()
    {
        if (IsTracking)
            Track(null);
    }

    private void Start()
    {
        trackedDestroyedObjects = new CircularArray<GameObject>();     
    }

    public void Track( GameObject gameObject)
    {
        trackedDestroyedObjects.WriteLastValue(gameObject);
    }

    public void Rewind(float seconds)
    {
        GameObject gameObject = trackedDestroyedObjects.ReadFromBuffer(seconds);

        if (gameObject != null)
        {
            Debug.Log(gameObject.name);
            Instantiate(gameObject);
        }
    }
}