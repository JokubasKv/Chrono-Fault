using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFollowMouse : MonoBehaviour
{
    PlayerInput inputActions;

    [SerializeField] Camera cam;
    [SerializeField] Transform player;
    [SerializeField] [Range (0f, 16f)] float treshold = 6f;



    #region -Awake/OnEnable/OnDisable -
    private void Awake()
    {
        inputActions = new PlayerInput();

        inputActions.Enable();
    }
    #endregion
    private void Update()
    {
        Vector3 input_Mouse = cam.ScreenToWorldPoint(inputActions.Gameplay.MousePosition.ReadValue<Vector2>());
        Vector3 targetPos = (player.position + input_Mouse) / 2f;

        targetPos.x = Mathf.Clamp(targetPos.x, -treshold + player.position.x, treshold + player.position.x);
        targetPos.y = Mathf.Clamp(targetPos.y, -treshold + player.position.y, treshold + player.position.y);

        this.transform.position = targetPos;
    }
}
