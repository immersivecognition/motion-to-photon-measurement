using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UXF;

public class CursorManager : MonoBehaviour
{
    private Session session;

    [Space]

    [SerializeField] private BaseCursorScript continuousCursor;
    [SerializeField] private BaseCursorScript discreteCursor;
    public BaseCursorScript activeCursor { get; private set; }

    public void AssignSettings(Session session)
    {
        this.session = session;

        bool continuous = session.settings.GetBool("continuous");
        activeCursor = continuous ? continuousCursor : discreteCursor;
        activeCursor.AssignSettings(session);
        activeCursor.gameObject.SetActive(true);
    }

    public void Update()
    {
        if (session && session.hasInitialised)
            activeCursor.Manage(EyeUpdateMethod.Update);
    }

    public void BeforeRender()
    {
        if (session && session.hasInitialised)
            activeCursor.Manage(EyeUpdateMethod.BeforeRender);
    }
}

/// <summary>
/// Control whether the eye script will be updated in the Update or OnBeforeRender loop
/// </summary>
public enum EyeUpdateMethod
{
    Update, BeforeRender
}