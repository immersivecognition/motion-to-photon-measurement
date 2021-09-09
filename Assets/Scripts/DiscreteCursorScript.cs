using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UXF;
using Valve.VR;

public class DiscreteCursorScript : BaseCursorScript
{
    Session session;

    [SerializeField] private EyeScript leftEyeScript;
    [SerializeField] private EyeScript rightEyeScript;
    public override EyeScript LeftEyeScript { get { return leftEyeScript; } }
    public override EyeScript RightEyeScript { get { return rightEyeScript; } }
    private Dictionary<EyeUpdateMethod, EyeScript> updateToEyeScript;

    private DiscreteHandler leftDiscreteHandler;
    private DiscreteHandler rightDiscreteHandler;
    private Dictionary<EyeUpdateMethod, DiscreteHandler> updateToHandler;

    [Space]

    [SerializeField] private SteamVR_Action_Boolean alignButtonPress;
    [SerializeField] private SteamVR_Input_Sources handType;

    private int windowLength;
    private float madModifier;

    public override void Start()
    {
        
    }

    /// <summary>
    /// Use the session settings to set the parameters 
    /// </summary>
    public override void AssignSettings(Session session)
    {
        this.session = session;

        windowLength = session.settings.GetInt("window_length");
        madModifier = session.settings.GetFloat("mad_modifier");

        alignButtonPress.AddOnStateDownListener(AlignButtonDown, handType);

        updateToEyeScript = new Dictionary<EyeUpdateMethod, EyeScript>()
        {
            { EyeUpdateMethod.Update, leftEyeScript },
            { EyeUpdateMethod.BeforeRender, rightEyeScript }
        };

        leftDiscreteHandler = new DiscreteHandler(windowLength, madModifier);
        rightDiscreteHandler = new DiscreteHandler(windowLength, madModifier);

        updateToHandler = new Dictionary<EyeUpdateMethod, DiscreteHandler>()
        {
            { EyeUpdateMethod.Update, leftDiscreteHandler },
            { EyeUpdateMethod.BeforeRender, rightDiscreteHandler }
        };
    }

    /// <summary>
    /// Perform this action when the align button (mapped to the trigger) is pressed
    /// </summary>
    protected override void AlignButtonDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        if (session.hasInitialised)
        {
            leftEyeScript.ResetColor();
            rightEyeScript.ResetColor();
            leftDiscreteHandler.ResetQueues();
            rightDiscreteHandler.ResetQueues();
            session.EndCurrentTrial();
        }
    }

    /// <summary>
    /// If the scene has been aligned, for the given update method check if the screen needs resetting, 
    /// then check if a threshold has been crossed and if so turn the screen to the threshold colour. 
    /// </summary>
    public override void Manage(EyeUpdateMethod updateMethod)
    {
        if (session.hasInitialised)
        {
            float currentPosition = transform.position.x;
            ScreenColorType newColor = updateToHandler[updateMethod].Manage(currentPosition);
            updateToEyeScript[updateMethod].SetColor(newColor);
        }
    }

}
