using UnityEngine;

public abstract class RewindAbstract : MonoBehaviour
{
    RewindManager rewindManager;
    public bool IsTracking { get; set; } = false;

    Rigidbody2D rb2d;
    AudioSource audioSource;


    protected void Awake()
    {

        rewindManager = FindObjectOfType<RewindManager>();
        if (rewindManager != null)
        {
            rb2d = GetComponent<Rigidbody2D>();
            audioSource = GetComponent<AudioSource>();

            IsTracking = true;
        }
        else
        {
            Debug.LogError("No Time Manager");
        }

        trackedTransformValues = new CircularArray<TransformValues>();
        trackedVelocities = new CircularArray<VelocityValues>();
        trackedAudioTimes = new CircularArray<AudioTrackedData>();
    }

    protected void FixedUpdate()
    {
        if (IsTracking)
            Track();
    }

    #region Transform

    CircularArray<TransformValues> trackedTransformValues;
    public struct TransformValues
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;
    }

    /// <summary>
    /// Call this method in Track() if you want to track object Position and Rotation
    /// </summary>
    protected void TrackTransform()
    {
        TransformValues valuesToWrite;
        valuesToWrite.position = transform.position;
        valuesToWrite.rotation = transform.rotation;
        valuesToWrite.scale = transform.localScale;
        trackedTransformValues.WriteLastValue(valuesToWrite);
    }
    /// <summary>
    /// Call this method in GetSnapshotFromSavedValues() to restore Position and Rotation
    /// </summary>
    protected void RestoreTransform(float seconds)
    {
        TransformValues valuesToRead = trackedTransformValues.ReadFromBuffer(seconds);
        transform.SetPositionAndRotation(valuesToRead.position, valuesToRead.rotation);
        transform.localScale = valuesToRead.scale;
    }
    #endregion

    #region Velocity
    public struct VelocityValues
    {
        public Vector3 velocity;
        public Vector3 angularVelocity;
    }
    CircularArray<VelocityValues> trackedVelocities;
    /// <summary>
    /// Call this method in Track() if you want to track velocity of Rigidbody
    /// </summary>
    protected void TrackVelocity()
    {
        if (rb2d != null)
        {
            VelocityValues valuesToWrite;
            valuesToWrite.velocity = rb2d.velocity;
            valuesToWrite.angularVelocity = new Vector3(rb2d.angularVelocity, 0, 0);
            trackedVelocities.WriteLastValue(valuesToWrite);
        }
        else
        {
            Debug.LogError("Cannot find Rigidbody on the object, while TrackVelocity() is being called!!!");
        }
    }
    /// <summary>
    /// Call this method in GetSnapshotFromSavedValues() to velocity of Rigidbody
    /// </summary>
    protected void RestoreVelocity(float seconds)
    {
        VelocityValues valuesToRead = trackedVelocities.ReadFromBuffer(seconds);
        rb2d.velocity = valuesToRead.velocity;
        rb2d.angularVelocity = valuesToRead.angularVelocity.x;
    }
    #endregion

    #region Audio
    CircularArray<AudioTrackedData> trackedAudioTimes;
    public struct AudioTrackedData
    {
        public float time;
        public bool isPlaying;
        public bool isEnabled;
    }
    /// <summary>
    /// Call this method in Track() if you want to track Audio
    /// </summary>
    protected void TrackAudio()
    {
        if (audioSource == null)
        {
            Debug.LogError("Cannot find AudioSource on the object, while TrackAudio() is being called!!!");
            return;
        }

        audioSource.volume = 1;
        AudioTrackedData dataToWrite;
        dataToWrite.time = audioSource.time;
        dataToWrite.isEnabled = audioSource.enabled;
        dataToWrite.isPlaying = audioSource.isPlaying;

        trackedAudioTimes.WriteLastValue(dataToWrite);
    }
    /// <summary>
    /// Call this method in GetSnapshotFromSavedValues() to restore Audio
    /// </summary>
    protected void RestoreAudio(float seconds)
    {
        AudioTrackedData readValues = trackedAudioTimes.ReadFromBuffer(seconds);
        audioSource.enabled = readValues.isEnabled;
        if (readValues.isPlaying)
        {
            audioSource.time = readValues.time;
            audioSource.volume = 0;

            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
        else if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
    #endregion



    private void OnTrackingChange(bool val)
    {
        IsTracking = val;
    }
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

    /// <summary>
    /// Main method where all tracking is filled, lets choose here what will be tracked for specific object
    /// </summary>
    protected abstract void Track();


    /// <summary>
    /// Main method where all rewinding is filled, lets choose here what will be rewinded for specific object
    /// </summary>
    /// <param name="seconds">Parameter defining how many seconds we want to rewind back</param>
    protected abstract void Rewind(float seconds);

}