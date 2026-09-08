using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovements : MonoBehaviour
{
    Vector2 movement;
    Vector2 camRot;
    Vector3 velocity;
    public CharacterController myController;
    public float speed = 5f;
    float xRotation = 0f;
    public float mouseSen = 100f;
    public Transform camRotate;
    public float gravity = -9.81f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    public void Move(InputAction.CallbackContext context)
    {
        movement = context.ReadValue<Vector2>();
    }

    public void CameraRotation(InputAction.CallbackContext context)
    {
        camRot = context.ReadValue<Vector2>() * mouseSen;
    }


    // Update is called once per frame
    void Update()
    {
        if (myController == null || !myController.enabled)
            return;

        // Gravity
        if (myController.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Small negative value to keep grounded
        }

        velocity.y += gravity * Time.deltaTime;

        // Horizontal movement and gravity are applied in a single Move call.
        // Two separate calls meant two full collision sweeps per frame.
        Vector3 move = movement.x * transform.right + movement.y * transform.forward;
        myController.Move((move * speed + velocity) * Time.deltaTime);

        // NOTE: camRot is a mouse delta, so it is deliberately NOT scaled by
        // Time.deltaTime - that would make the look speed frame-rate dependent in
        // the opposite direction. Sensitivity is left exactly as it was tuned.
        xRotation -= camRot.y;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        if (camRotate != null)
            camRotate.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        transform.Rotate(Vector3.up * camRot.x);
    }
}
