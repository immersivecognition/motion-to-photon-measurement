using UnityEngine;
using UnityEngine.UI;
using UXF;

public class ColorTracker : Tracker
{
    [SerializeField] private CursorManager cursorManager;

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
		Vector3 p = cursorManager.transform.position;
        string leftColor = cursorManager.activeCursor.LeftEyeScript.CurrentColor.ToString();
		string rightColor = cursorManager.activeCursor.RightEyeScript.CurrentColor.ToString();

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
