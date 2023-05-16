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

    public static readonly float howManySecondsToTrack = 6;

    public void StartRewindTimeBySeconds(float seconds)
    {
        CheckReachingOutOfBounds(seconds);

        rewindSeconds = seconds;
        TrackingStateCall?.Invoke(false);
        IsBeingRewinded = true;
    }


    public void SetTimeSecondsInRewind(float seconds)
    {
        CheckReachingOutOfBounds(seconds);
        rewindSeconds = seconds;
    }

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
            Debug.LogError("Not enough stored tracked value!");
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
            UIManagerSingleton.instance.SetTimeBar(HowManySecondsAvailableForRewind - rewindSeconds / (float)HowManySecondsAvailableForRewind);
            RewindTimeCall?.Invoke(rewindSeconds);
        }
        else if (HowManySecondsAvailableForRewind != howManySecondsToTrack)
        {
            UIManagerSingleton.instance.SetTimeBar(HowManySecondsAvailableForRewind / (float)howManySecondsToTrack);
            HowManySecondsAvailableForRewind += Time.fixedDeltaTime;

            if (HowManySecondsAvailableForRewind > howManySecondsToTrack)
                HowManySecondsAvailableForRewind = howManySecondsToTrack;
        }
    }

    protected override void InternalInit()
    {
    }

    protected override void InternalOnDestroy()
    {
    }
}
