using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovements : MonoBehaviour
{
    Vector2 movement;
    Vector2 camRot;
    public CharacterController myController;
    public float speed = 5f;
    float xRotation = 0f;
    public float mouseSen = 100f;
    public Transform camRotate;
    public float gravity = -9.81f;


    public void Move(InputAction.CallbackContext context)
    {
        movement = context.ReadValue<Vector2>();
    }

    public void CameraRotation(InputAction.CallbackContext context)
    {
        camRot = context.ReadValue<Vector2>() * mouseSen * Time.deltaTime;
    }


    // Update is called once per frame
    void Update()
    {
        Vector3 move = movement.x * transform.right + movement.y * transform.forward;
        myController.Move(move * speed * Time.deltaTime);

        xRotation -= camRot.y;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        camRotate.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        transform.Rotate(Vector3.up * camRot.x);

    }
}
