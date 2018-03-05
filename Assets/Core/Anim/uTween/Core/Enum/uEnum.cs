using UnityEngine;
using System.Collections;

namespace BridgeUI.uTween {
    public enum Direction
    {
		Reverse = -1,
		Toggle = 0,
		Forward = 1
	}

	public enum Trigger {
		OnPointerEnter,
		OnPointerDown,
		OnPointerClick,
		OnPointerUp,
		OnPointerExit,
	}

    public enum eShake
    {
        Position,
        Rotation,
        Scale
    }
}