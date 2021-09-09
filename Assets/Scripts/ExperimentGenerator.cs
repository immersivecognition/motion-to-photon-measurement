using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UXF;

public class ExperimentGenerator : MonoBehaviour
{
    public void GenerateExperiment(Session session)
    {
        uint nRepeat = 100;
        Block mainBlock = new Block(nRepeat, session);
    }
}
