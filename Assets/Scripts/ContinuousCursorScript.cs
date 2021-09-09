using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using UXF;
using System.Linq;

public class ContinuousCursorScript : BaseCursorScript
{
    Session session;

    [SerializeField] private EyeScript leftEyeScript;
    [SerializeField] private EyeScript rightEyeScript;
    public override EyeScript LeftEyeScript { get { return leftEyeScript; } }
    public override EyeScript RightEyeScript { get { return rightEyeScript; } }
    private Dictionary<EyeUpdateMethod, EyeScript> updateToEyeScript;

    [Space]

    [SerializeField] private SteamVR_Action_Boolean alignButtonPress;
    [SerializeField] private SteamVR_Input_Sources handType;

    private List<float> boundaries = new List<float>(2) { 0, 0 };
    private List<bool> aligned = new List<bool>(2) { false, false };
    private bool collectingBoundarySamples = false;
    private int nBoundarySamples;
    [SerializeField][ReadOnly]private List<float> thresholds = new List<float>();
    private int nThreshold = 2;
    private float lagPaddingPercent = 5f;
    
    private Dictionary<EyeUpdateMethod, float> currentPosition;
    private Dictionary<EyeUpdateMethod, float> previousPosition;

    public override void Start()
    {
        
    }

    /// <summary>
    /// Use the session settings to set the parameters 
    /// </summary>
    public override void AssignSettings(Session session)
    {
        this.session = session;

        lagPaddingPercent = session.settings.GetFloat("lag_padding_percent");
        nThreshold = session.settings.GetInt("n_thresholds");
        nBoundarySamples = session.settings.GetInt("n_boundary_samples");

        alignButtonPress.AddOnStateDownListener(AlignButtonDown, handType);

        updateToEyeScript = new Dictionary<EyeUpdateMethod, EyeScript>()
        {
            { EyeUpdateMethod.Update, leftEyeScript },
            { EyeUpdateMethod.BeforeRender, rightEyeScript }
        };

        currentPosition = new Dictionary<EyeUpdateMethod, float>()
        {
            { EyeUpdateMethod.Update, float.PositiveInfinity },
            { EyeUpdateMethod.BeforeRender, float.PositiveInfinity }
        };

        previousPosition = new Dictionary<EyeUpdateMethod, float>()
        {
            { EyeUpdateMethod.Update, float.PositiveInfinity },
            { EyeUpdateMethod.BeforeRender, float.PositiveInfinity }
        };
    }

    /// <summary>
    /// Perform this action when the align button (mapped to the trigger) is pressed
    /// </summary>
    protected override void AlignButtonDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
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
        aligned[boundaryIndex] = true;

        if (boundaryIndex == 1)
            SetThresholds();
    }

    /// <summary>
    /// Create the threshold positions
    /// </summary>
    private void SetThresholds()
    {
        float padding = 0.01f * lagPaddingPercent * (boundaries[1] - boundaries[0]);
        float leftThreshold = boundaries[0] + padding;
        float rightThreshold = boundaries[1] - padding;

        thresholds = Linspace(leftThreshold, rightThreshold, nThreshold);
    }

    /// <summary>
    /// Generate a sequence of equally spaced numbers between the start and end values (inclusive) with a defined number of divisions. 
    /// </summary>
    private List<float> Linspace(float start, float end, int divisions) {
        List<float> values = new List<float>();

        float step = (end - start) / ((float) divisions - 1f);

        for (int i = 0; i < divisions; i++) 
            values.Add(start + (float) i * step);

        return values;
    }

    /// <summary>
    /// Check if either the origin or the boundary have been crossed since the previous frame
    /// </summary>
    private bool CheckThresholdCrossed(float currentPos, float previousPos)
    {
        foreach (float threshold in thresholds) 
            if (threshold >= previousPos && threshold <= currentPos || threshold >= currentPos && threshold <= previousPos)
                return true;
            
        return false;
    }

    /// <summary>
    /// If the scene has been aligned, for the given update method check if the screen needs resetting, 
    /// then check if a threshold has been crossed and if so turn the screen to the threshold colour. 
    /// </summary>
    public override void Manage(EyeUpdateMethod updateMethod)
    {
        previousPosition[updateMethod] = currentPosition[updateMethod];
        currentPosition[updateMethod] = transform.position.x;

        if (aligned[0] & aligned[1]) 
        {
            bool defaultColor = updateToEyeScript[updateMethod].CheckCurrentColorDefault();
            if (!defaultColor)
                updateToEyeScript[updateMethod].ResetColor();
            
            bool boundaryCrossed = CheckThresholdCrossed(currentPosition[updateMethod], previousPosition[updateMethod]);
            if (boundaryCrossed) 
                updateToEyeScript[updateMethod].SetColor(ScreenColorType.Threshold);
        }
    }

}
