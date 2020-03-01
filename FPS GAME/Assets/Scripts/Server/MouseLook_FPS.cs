using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook_FPS : MonoBehaviour
{
    [SerializeField] private Transform player_Root;
    [SerializeField] private Transform look_Root;
    [SerializeField] private bool invert;
    [SerializeField] private bool can_Unlock = true;
    [SerializeField] private float sensivity = 5f;
    [SerializeField] private int smooth_Step = 10;
    [SerializeField] private float smooth_Weight = 0.4f;
    [SerializeField] private float roll_Angle = 10f;
    [SerializeField] private float roll_Speed = 3f;
    [SerializeField] private Vector2 default_Look_Limits = new Vector2(-70f, 80f);

    private Vector2 look_Angles;
    public Vector2 current_Mouse_Look;
    private Vector2 smooth_Move;
    private float current_Roll_Angle;
    private int last_Look_Frame;


    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; //Start the game with the cursor locked.
    }

    private void Update()
    {
        LockAndUnlockCursor();

        // Only allow to rotate if the cursor is locked.
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            LookAround();
        }
    }

    private void LockAndUnlockCursor()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }

    private void LookAround()
    {
        // Gets the mouse input axis.
        current_Mouse_Look = new Vector2(Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"));

        // Here we are using a Vector2 (look_Angles) to seperate the mouse axis, since both axis have different operations to perform.
        look_Angles.x += current_Mouse_Look.x * sensivity * (invert ? 1f : -1f);
        look_Angles.y += current_Mouse_Look.y * sensivity;

        // Limits how far we can roll on the X axis (this mean looking up and down).
        look_Angles.x = Mathf.Clamp(look_Angles.x, default_Look_Limits.x, default_Look_Limits.y);

        // This is giving a slight camera angle which is then passed to the Z rotation axis to simulate a head tilt.
        current_Roll_Angle = Mathf.Lerp(current_Roll_Angle, Input.GetAxisRaw("Mouse X") * roll_Angle, Time.deltaTime * roll_Speed);

        // Apply rotation on the X axis to look up and down and apply the slight tilt on the Z axis.
        // This is done to the LookRoot which holds the cameras.
        look_Root.localRotation = Quaternion.Euler(look_Angles.x, 0f, current_Roll_Angle);
        // Apply rotation to the Y axis to look left and right to the player root body.
        player_Root.localRotation = Quaternion.Euler(0f, look_Angles.y, 0f);
    }
}