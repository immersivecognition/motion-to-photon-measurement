using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UXF;

public class EyeScript : MonoBehaviour
{
    [SerializeField] private GameObject defaultCanvas;
    [SerializeField] private GameObject alignCanvas;
    [SerializeField] private GameObject thresholdCanvas;
    [SerializeField] private GameObject initiateCanvas;
    [SerializeField] private GameObject preInitiateCanvas;
    [SerializeField] private int activeLayer = 9;
    private int inactiveLayer = 11;
    private Dictionary<ScreenColorType, GameObject> screenColorTypeToCanvasMap;

    [Space]

    [SerializeField][ReadOnly] private Color defaultColor;
    [SerializeField][ReadOnly] private Color alignColor;
    [SerializeField][ReadOnly] private Color thresholdColor;
    [SerializeField][ReadOnly] private Color initiateColor;
    [SerializeField][ReadOnly] private Color preInitiateColor;
    private ScreenColorType currentColor = ScreenColorType.Default;
    public ScreenColorType CurrentColor { get { return currentColor; } }

    void Start()
    {
        screenColorTypeToCanvasMap =  new Dictionary<ScreenColorType, GameObject>()
        {
            { ScreenColorType.Default, defaultCanvas },
            { ScreenColorType.Align, alignCanvas },
            { ScreenColorType.Threshold, thresholdCanvas },
            { ScreenColorType.Initiate, initiateCanvas },
            { ScreenColorType.PreInitiate, preInitiateCanvas }
        };
    }

    /// <summary>
    /// Use the session settings to set the parameters 
    /// </summary>
    public void AssignSettings(Session session)
    {
        ColorUtility.TryParseHtmlString(session.settings.GetString("default_color"), out defaultColor);
        ColorUtility.TryParseHtmlString(session.settings.GetString("threshold_color"), out thresholdColor);
        ColorUtility.TryParseHtmlString(session.settings.GetString("align_color"), out alignColor);
        ColorUtility.TryParseHtmlString(session.settings.GetString("initiate_color"), out initiateColor);
        ColorUtility.TryParseHtmlString(session.settings.GetString("pre_initiate_color"), out preInitiateColor);

        defaultCanvas.GetComponent<Image>().color = defaultColor;
        alignCanvas.GetComponent<Image>().color = alignColor;
        thresholdCanvas.GetComponent<Image>().color = thresholdColor;
        initiateCanvas.GetComponent<Image>().color = initiateColor;
        preInitiateCanvas.GetComponent<Image>().color = preInitiateColor;

        SetColor(ScreenColorType.Default);
    }

    public void ResetColor()
    {
        SetColor(ScreenColorType.Default);
    }

    public void SetColor(ScreenColorType screenColorType)
    {
        defaultCanvas.layer = inactiveLayer;
        alignCanvas.layer = inactiveLayer;
        thresholdCanvas.layer = inactiveLayer;
        initiateCanvas.layer = inactiveLayer;
        preInitiateCanvas.layer = inactiveLayer;

        screenColorTypeToCanvasMap[screenColorType].layer = activeLayer;
        currentColor = screenColorType;
    }

    public bool CheckCurrentColorDefault()
    {
        return currentColor == ScreenColorType.Default;
    }

    /// <summary>
    /// Turns the eye on for one frame
    /// </summary>
    public void OnOneFrame(ScreenColorType screenColorType)
    {
        StartCoroutine(TurnOnOneFrame(screenColorType));
    }

    /// <summary>
    /// Turns the eye on for one frame
    /// </summary>
    private IEnumerator TurnOnOneFrame(ScreenColorType screenColorType)
    {
        SetColor(screenColorType);
        yield return null;
        SetColor(ScreenColorType.Default);
    }

}

/// <summary>
/// The type of flash that should be shown
/// </summary>
public enum ScreenColorType
{
    Default, Align, Threshold, PreInitiate, Initiate
}
