using UnityEngine;

public class RewindPosition : RewindAbstract
{
    protected override void Rewind(float seconds)
    {
        RestoreTransform(seconds);
        RestoreVelocity(seconds);
    }

    protected override void Track()
    {
        TrackTransform();
        TrackVelocity();
    }

}