using System;
using UnityEngine;

public class RewindManager : MonoSingleton<RewindManager>
{
    [SerializeField] public static Action<float> RewindTimeCall;
    [SerializeField] public static Action<bool> TrackingStateCall;
    [SerializeField] public static Action<float> RestoreBuffers;


    public float HowManySecondsAvailableForRewind;
    public bool IsBeingRewinded = false;

    float rewindSeconds = 0;

    /// <summary>
    /// Variable defining how much into the past should be tracked, after set limit is hit, old values will be overwritten in circular buffer
    /// </summary>
    public static readonly float howManySecondsToTrack = 6;

    /// <summary>
    /// Call this method if you want to start rewinding time with ability to preview snapshots. After done rewinding, StopRewindTimeBySeconds() must be called!!!. To update snapshot preview between, call method SetTimeSecondsInRewind()
    /// </summary>
    /// <param name="seconds">Parameter defining how many seconds before should the rewind preview rewind to (Parameter must be >=0)</param>
    /// <returns></returns>
    public void StartRewindTimeBySeconds(float seconds)
    {
        CheckReachingOutOfBounds(seconds);

        rewindSeconds = seconds;
        TrackingStateCall?.Invoke(false);
        IsBeingRewinded = true;
    }

    /// <summary>
    /// Call this method to update rewind preview while rewind is active (StartRewindTimeBySeconds() method was called before)
    /// </summary>
    /// <param name="seconds">Parameter defining how many seconds should the rewind preview move to (Parameter must be >=0)</param>
    public void SetTimeSecondsInRewind(float seconds)
    {
        CheckReachingOutOfBounds(seconds);
        rewindSeconds = seconds;
    }
    /// <summary>
    /// Call this method to stop previewing rewind state and effectively set current values to the rewind state
    /// </summary>
    public void StopRewindTimeBySeconds()
    {
        HowManySecondsAvailableForRewind -= rewindSeconds;
        IsBeingRewinded = false;
        RestoreBuffers?.Invoke(rewindSeconds);
        TrackingStateCall?.Invoke(true);
    }
    private void CheckReachingOutOfBounds(float seconds)
    {
        if (seconds > HowManySecondsAvailableForRewind)
        {
            Debug.LogError("Not enough stored tracked value!!! Reaching on wrong index. Called rewind should be less than HowManySecondsAvailableForRewind property");
            return;
        }
        if (seconds < 0)
        {
            Debug.LogError("Parameter in StartRewindTimeBySeconds() must have positive value!!!");
            return;
        }
    }
    private void OnEnable()
    {
        HowManySecondsAvailableForRewind = 0;
    }

    private void FixedUpdate()
    {
        if (IsBeingRewinded)
        {
            RewindTimeCall?.Invoke(rewindSeconds);
        }
        else if (HowManySecondsAvailableForRewind != howManySecondsToTrack)
        {
            HowManySecondsAvailableForRewind += Time.fixedDeltaTime;

            if (HowManySecondsAvailableForRewind > howManySecondsToTrack)
                HowManySecondsAvailableForRewind = howManySecondsToTrack;
        }
    }
}
