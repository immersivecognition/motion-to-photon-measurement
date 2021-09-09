using System;
using System.Collections.Generic;

public class DiscreteHandler
{
    private int windowLength;
    private float madModifier;
    private Queue<float> currentWindow;
    private Queue<float> originalWindow;
    private CursorStatus cursorStatus = CursorStatus.Default;
    private Dictionary<CursorStatus, ScreenColorType> statusToColor = new Dictionary<CursorStatus, ScreenColorType>()
    {
        { CursorStatus.Default, ScreenColorType.Default },
        { CursorStatus.PreInitiate, ScreenColorType.PreInitiate },
        { CursorStatus.Initiate, ScreenColorType.Initiate }
    };
    private int framesToDisable = 0;
    private int nDisableFrames = 90;

    public DiscreteHandler(int windowLength, float madModifier)
    {
        this.windowLength = windowLength;
        this.madModifier = madModifier;

        currentWindow = new Queue<float>(windowLength);
    }

    private void AddPositionToQueue(Queue<float> queue, float newPosition)
    {
        if (queue.Count == windowLength)
            queue.Dequeue();
        queue.Enqueue(newPosition);
    }

    public ScreenColorType Manage(float newPosition)
    {
        if (currentWindow.Count < windowLength)
        {
            AddPositionToQueue(currentWindow, newPosition);
            return ScreenColorType.Default;
        }
        
        switch(cursorStatus)
        {
            case CursorStatus.Default:
            {
                if (framesToDisable == 0)
                {
                    // get median absolute deviation of current window and check new position is clased as an outlier to that sample
                    float[] currentWindowArr = currentWindow.ToArray();
                    float medianPosition = GetMedian(currentWindowArr);
                    float[] absoluteDeviations = GetAbsoluteDeviations(currentWindowArr, medianPosition);
                    float madPosition = 1.4826f * GetMedian(absoluteDeviations);

                    if (madPosition != 0 && Math.Abs(newPosition - medianPosition) / madPosition > madModifier)
                    {
                        cursorStatus = CursorStatus.PreInitiate;
                        originalWindow = new Queue<float>(currentWindow);
                    }
                }

                break;
            }
            case CursorStatus.PreInitiate:
            {
                // get median absolute deviation of the original window from when pre-initiate was called and see if the new sample is also an outlier,
                // if so then mark as full initiate, otherwise go back to default
                float[] originalWindowArr = currentWindow.ToArray();
                float medianPosition = GetMedian(originalWindowArr);
                float[] absoluteDeviations = GetAbsoluteDeviations(originalWindowArr, medianPosition);
                float madPosition = 1.4826f * GetMedian(absoluteDeviations);

                if (madPosition != 0 && Math.Abs(newPosition - medianPosition) / madPosition > madModifier)
                    cursorStatus = CursorStatus.Initiate;
                else 
                    cursorStatus = CursorStatus.Default;

                originalWindow = null;
                break;
            }
            case CursorStatus.Initiate:
            {
                cursorStatus = CursorStatus.Default;
                framesToDisable = nDisableFrames;
                break;
            }
        }

        AddPositionToQueue(currentWindow, newPosition);
        if (framesToDisable > 0)
            framesToDisable--;

        return statusToColor[cursorStatus];
    }

    private float GetMedian(float[] positions)
    {
        float[] sortedPositions = (float[]) positions.Clone();
        System.Array.Sort(sortedPositions);

        int size = sortedPositions.Length;
        int mid = size / 2;
        if (size % 2 != 0)
            return sortedPositions[mid];
        
        return (sortedPositions[mid] + sortedPositions[mid - 1]) * 0.5f;
    }

    private float[] GetAbsoluteDeviations(float[] positions, float medianPosition)
    {
        float[] absoluteDeviations = new float[positions.Length];

        for (int i = 0; i < positions.Length; i++)
            absoluteDeviations[i] = Math.Abs(positions[i] - medianPosition);
        
        return absoluteDeviations;
    }

    public void ResetQueues()
    {
        currentWindow = new Queue<float>(windowLength);
        originalWindow = null;
    }
}

public enum CursorStatus 
{ 
    Default, PreInitiate, Initiate 
}