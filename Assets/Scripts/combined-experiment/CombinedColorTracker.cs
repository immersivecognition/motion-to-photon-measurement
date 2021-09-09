using UnityEngine;
using UnityEngine.UI;
using UXF;

public class CombinedColorTracker : Tracker
{
    [SerializeField] private CombinedCursorScript cursorScript;

    /// <summary>
	/// Sets measurementDescriptor and customHeader to appropriate values
	/// </summary>
	protected override void SetupDescriptorAndHeader()
	{
        measurementDescriptor = "movement_and_color";
        
		customHeader = new string[]
		{
			"pos_x",
            "left_color",
			"right_color"
		};
	}

	/// <summary>
	/// Returns current position and rotation values
	/// </summary>
	protected override string[] GetCurrentValues()
	{
		Vector3 p = cursorScript.transform.position;
        string leftColor = cursorScript.LeftEyeScript.CurrentColor.ToString();
		string rightColor = cursorScript.RightEyeScript.CurrentColor.ToString();

		string format = "0.########";

		// return position, rotation (x, y, z) as an array
		var values =  new string[]
		{
			p.x.ToString(format),
			leftColor,
			rightColor
		};

		return values;
	}
}
