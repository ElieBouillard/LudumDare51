using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CMF
{
	//This character movement input class is an example of how to get input from a keyboard to control the character;
    public class CharacterKeyboardInput : CharacterInput
    {
		public string horizontalInputAxis = "Horizontal";
		public string verticalInputAxis = "Vertical";
		public KeyCode jumpKey = KeyCode.Space;

		//If this is enabled, Unity's internal input smoothing is bypassed;
		public bool useRawInput = true;

		public int ControlId;
		
		private void Start()
		{
			if(PlayerPrefs.HasKey("Controls"))
				ControlId = PlayerPrefs.GetInt("Controls");
		}

		public override float GetHorizontalMovementInput()
		{
			if (useRawInput)
			{
				float horizontalValue = 0;
				if (ControlId == 0)
				{
					if (Input.GetKey(KeyCode.Q)) horizontalValue = -1;
					else if (Input.GetKey(KeyCode.D)) horizontalValue = 1;
					else horizontalValue = 0;
				}
				else
				{
					if (Input.GetKey(KeyCode.A)) horizontalValue = -1;
					else if (Input.GetKey(KeyCode.D)) horizontalValue = 1;
					else horizontalValue = 0;
				}

				return horizontalValue;
			}
			else
				return Input.GetAxis(horizontalInputAxis);
		}

		public override float GetVerticalMovementInput()
		{
			if (useRawInput)
			{
				float verticalValue = 0;
				if (ControlId == 0)
				{
					if (Input.GetKey(KeyCode.Z)) verticalValue = 1;
					else if (Input.GetKey(KeyCode.S)) verticalValue = -1;
					else verticalValue = 0;
				}
				else
				{
					if (Input.GetKey(KeyCode.W)) verticalValue = 1;
					else if (Input.GetKey(KeyCode.S)) verticalValue = -1;
					else verticalValue = 0;
				}

				return verticalValue;
			}
			else
				return Input.GetAxis(verticalInputAxis);
		}

		public override bool IsJumpKeyPressed()
		{
			return Input.GetKey(jumpKey);
		}
    }
}
