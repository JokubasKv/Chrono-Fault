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

    protected void TrackTransform()
    {
        TransformValues valuesToWrite;
        valuesToWrite.position = transform.position;
        valuesToWrite.rotation = transform.rotation;
        valuesToWrite.scale = transform.localScale;
        trackedTransformValues.WriteLastValue(valuesToWrite);
    }

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


    protected abstract void Track();
    protected abstract void Rewind(float seconds);

}