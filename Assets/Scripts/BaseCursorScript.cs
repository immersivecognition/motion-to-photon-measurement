using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UXF;
using Valve.VR;

public abstract class BaseCursorScript : MonoBehaviour
{
    public abstract EyeScript LeftEyeScript { get; }
    public abstract EyeScript RightEyeScript { get; }

    public abstract void Start();
    public abstract void AssignSettings(Session session);
    public abstract void Manage(EyeUpdateMethod updateMethod);
    protected abstract void AlignButtonDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource);
}
