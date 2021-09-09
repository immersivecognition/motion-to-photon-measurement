using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using UXF;
using System.Linq;
using System;

public class CombinedCursorScript : MonoBehaviour
{
    private Session session;
    [SerializeField] private EyeScript leftEyeScript;
    [SerializeField] private EyeScript rightEyeScript;
    public EyeScript LeftEyeScript { get { return leftEyeScript; } }
    public EyeScript RightEyeScript { get { return rightEyeScript; } }

    [Space]

    [SerializeField] private SteamVR_Action_Boolean alignButtonPress;
    [SerializeField] private SteamVR_Input_Sources handType;

    [Space]

    // continuous settings
    [SerializeField][ReadOnly] private List<float> boundaries = new List<float>(2) { 0, 0 };
    private List<bool> aligned = new List<bool>(2) { false, false };
    private bool collectingBoundarySamples = false;
    private int nBoundarySamples;

    [Space]
    
    [SerializeField][ReadOnly] private ScreenColorType currentScreenColor = ScreenColorType.Default;
    private int framesSinceStart = 0;
    private int nSwitchAfterFrames = 10;

    public void AssignSettings(Session session)
    {
        this.session = session;

        nBoundarySamples = session.settings.GetInt("n_boundary_samples");

        alignButtonPress.AddOnStateDownListener(AlignButtonDown, handType);
    }

    /// <summary>
    /// Perform this action when the align button (mapped to the trigger) is pressed
    /// </summary>
    private void AlignButtonDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        if (session.hasInitialised)
        {
            if (!aligned[0] & !collectingBoundarySamples)
            {
                StartCoroutine(CollectBoundarySamples(0));
            }
            else if (!aligned[1] & !collectingBoundarySamples)
            {
                StartCoroutine(CollectBoundarySamples(1));
            }
            else if (!collectingBoundarySamples)
            {
                aligned[0] = false;
                aligned[1] = false;
                leftEyeScript.ResetColor();
                rightEyeScript.ResetColor();
                currentScreenColor = ScreenColorType.Default;
                framesSinceStart = 0;
                session.EndCurrentTrial();
            }
        }
    }

    /// <summary>
    /// Collect a number of position samples so the boundary positions are based on an average over these to reduce noise. 
    /// </summary>
    private IEnumerator CollectBoundarySamples(int boundaryIndex)
    {
        collectingBoundarySamples = true;
        List<float> boundarySamples = new List<float>();

        yield return new WaitForSeconds(0.2f); // wait for vibrations to settle

        leftEyeScript.OnOneFrame(ScreenColorType.Align); 

        while (boundarySamples.Count < nBoundarySamples)
        {
            boundarySamples.Add(transform.position.x);
            yield return null;
        }

        leftEyeScript.OnOneFrame(ScreenColorType.Align); 

        boundaries[boundaryIndex] = boundarySamples.Average();
        collectingBoundarySamples = false;

        yield return null;
        yield return null;

        aligned[boundaryIndex] = true;
    }  

    public void BeforeRender()
    {
        if (aligned[0] && aligned[1])
        {
            Manage(EyeUpdateMethod.BeforeRender);
            SetScreenColors(currentScreenColor);
        }
    }

    private void Manage(EyeUpdateMethod updateMethod)
    {
        if (session != null && session.hasInitialised && aligned[0] && aligned[1])
        {
            if (currentScreenColor == ScreenColorType.Default)
            {
                currentScreenColor = ScreenColorType.Threshold;
            }
            else if (framesSinceStart % nSwitchAfterFrames == 0)
            {
                currentScreenColor = ScreenColorType.Threshold;
            }
            else 
            {
                currentScreenColor = currentScreenColor == ScreenColorType.PreInitiate ? ScreenColorType.Initiate : ScreenColorType.PreInitiate;
            }

            framesSinceStart++;
        }
    }

    private void SetScreenColors(ScreenColorType newColor)
    {
        leftEyeScript.SetColor(newColor);
        rightEyeScript.SetColor(newColor);
    }

}
