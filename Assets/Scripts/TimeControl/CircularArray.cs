using System.Collections.Generic;
using System;
using UnityEngine;

public class CircularArray<T>
{
    T[] dataArray;
    int bufferCurrentPosition = -1;
    int bufferCapacity;
    float howManyRecordsPerSecond;

    public CircularArray()
    {
        try
        {
            howManyRecordsPerSecond = Time.timeScale / Time.fixedDeltaTime;
            bufferCapacity = (int)(RewindManager.howManySecondsToTrack * howManyRecordsPerSecond);
            dataArray = new T[bufferCapacity];
            RewindManager.RestoreBuffers += OnBuffersRestore;
        }
        catch
        {
            Debug.LogError("Error");
        }
    }

    public void WriteLastValue(T val)
    {
        bufferCurrentPosition++;
        if (bufferCurrentPosition >= bufferCapacity)
        {
            bufferCurrentPosition = 0;
            dataArray[bufferCurrentPosition] = val;
        }
        else
        {
            dataArray[bufferCurrentPosition] = val;
        }
    }

    public T ReadLastValue()
    {
        return dataArray[bufferCurrentPosition];
    }

    public T ReadFromBuffer(float seconds)
    {
        int howManyBeforeLast = (int)(howManyRecordsPerSecond * seconds);

        if ((bufferCurrentPosition - howManyBeforeLast) < 0)
        {
            int showingIndex = bufferCapacity - (howManyBeforeLast - bufferCurrentPosition);
            return dataArray[showingIndex];
        }
        else
        {
            return dataArray[bufferCurrentPosition - howManyBeforeLast];
        }
    }
    private void MoveLastBufferPosition(float seconds)
    {
        int howManyBeforeLast = (int)(howManyRecordsPerSecond * seconds);

        if ((bufferCurrentPosition - howManyBeforeLast) < 0)
        {
            bufferCurrentPosition = bufferCapacity - (howManyBeforeLast - bufferCurrentPosition);
        }
        else
        {
            bufferCurrentPosition -= howManyBeforeLast;
        }
    }
    private void OnBuffersRestore(float seconds)
    {
        MoveLastBufferPosition(seconds);
    }

}