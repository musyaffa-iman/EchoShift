using UnityEngine;
using UnityEngine.UI;

public class MobileControls : MonoBehaviour
{
    [SerializeField] private FloatingJoystick movementJoystick;
    [SerializeField] private Button attackButton;

    public FloatingJoystick MovementJoystick
    {
        get
        {
            if (movementJoystick == null)
                movementJoystick = GetComponentInChildren<FloatingJoystick>();
            return movementJoystick;
        }
    }

    public Button AttackButton
    {
        get
        {
            if (attackButton == null)
                attackButton = GetComponentInChildren<Button>();
            return attackButton;
        }
    }
}
