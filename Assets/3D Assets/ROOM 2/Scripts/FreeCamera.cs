using UnityEngine;
using UnityEngine.InputSystem;

// Demo fly-camera shipped with the asset pack.
// Rewritten for the new Input System: the project has Active Input Handling set
// to "Input System Package (New)", where UnityEngine.Input.GetAxis throws an
// InvalidOperationException on every call.
public class FreeCamera : MonoBehaviour
{
	public float movementSpeed = 5.0f;
	public float lookSpeed = 0.05f;

	void FixedUpdate()
	{
		Keyboard keyboard = Keyboard.current;
		Mouse mouse = Mouse.current;

		if (keyboard != null)
		{
			float horizontal = 0f;
			float vertical = 0f;

			if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) horizontal -= 1f;
			if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) horizontal += 1f;
			if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) vertical -= 1f;
			if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) vertical += 1f;

			float step = Time.fixedDeltaTime * movementSpeed;

			transform.Translate(horizontal * step, 0f, 0f);
			transform.Translate(0f, 0f, vertical * step);
		}

		if (mouse != null)
		{
			Vector2 delta = mouse.delta.ReadValue() * lookSpeed;
			transform.eulerAngles += new Vector3(-delta.y, delta.x, 0f);
		}
	}
}
