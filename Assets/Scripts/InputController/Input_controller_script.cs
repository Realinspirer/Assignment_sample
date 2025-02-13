using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(PlayerInput))]
public class Input_controller_script : MonoBehaviour
{
    public static Vector2 mouse_position;
    public static bool mouse_down;

    //called by playerinput component
    void OnCursorPosition(InputValue pointer_position)
    {
        mouse_position = pointer_position.Get<Vector2>();

    }
    void OnPointerDown(InputValue pointer_val)
    {
        mouse_down = pointer_val.Get<float>() > 0;
    }
}
