using UnityEngine;

public class SimpleFreeLookCamera : MonoBehaviour
{
    [SerializeField]
    private float movementSpeed     = 50.0f,
                  fastMovementSpeed = 200.0f,
                  cameraSensitivity = 3.0f;

    void Start()
    {
        //TODO might want to add a proper way to unlock mouse cursor later. But pressing the ESC key works in editor for now.
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        bool fastCam = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        float movementSpeed = fastCam ? this.fastMovementSpeed : this.movementSpeed;

        if (Input.GetKey(KeyCode.A))
        {
            transform.position = transform.position + (-transform.right * movementSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.D))
        {
            transform.position = transform.position + (transform.right * movementSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.W))
        {
            transform.position = transform.position + (transform.forward * movementSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.S))
        {
            transform.position = transform.position + (-transform.forward * movementSpeed * Time.deltaTime);
        }


        float newRotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * cameraSensitivity;
        float newRotationY = transform.localEulerAngles.x - Input.GetAxis("Mouse Y") * cameraSensitivity;
        transform.localEulerAngles = new Vector3(newRotationY, newRotationX, 0f);
    }
}
