using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region ServerControls
    private void FixedUpdate()
    {
        if (Client.instance.Connected())
        {
            SendInputToServer();
        } else 
            WipePlayers();
    }

    private void WipePlayers()
    {
        foreach (PlayerManager player in GameManager.players.Values)
        {
            Destroy(player.gameObject);
        }
        GameManager.players.Clear();
    }

    private void SendInputToServer()
    {
        if (cursorFreedom)
            return;

        bool[] _inputs = new bool[]
        {
            Input.GetKey(KeyCode.W),
            Input.GetKey(KeyCode.S),
            Input.GetKey(KeyCode.A),
            Input.GetKey(KeyCode.D),
            Input.GetKey(KeyCode.Space),
            Input.GetKey(KeyCode.LeftShift),
            Input.GetMouseButton(0),
            Input.GetMouseButton(1),
            Input.GetKey(KeyCode.LeftControl)
        };

        ClientSend.PlayerMovement(_inputs);
        ClientSend.PlayerRotation();
    }
    #endregion

    #region ClientControls

    private bool cursorFreedom = false;

    private void LockedMode(bool locked)
    {
        if (locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            CameraController.instance.focused = true;
            UIManager.instance.InGameMenu(false);
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            CameraController.instance.focused = false;
            UIManager.instance.InGameMenu(true);
        }
    }

    private void Update()
    {
        ClientInputs();

        //Focused Camera
        if (!cursorFreedom && Cursor.lockState == CursorLockMode.None)
        {
            LockedMode(true);
        } 
        //Menu
        else if (cursorFreedom && Cursor.lockState == CursorLockMode.Locked)
        {
            LockedMode(false);
        }
    }

    private void ClientInputs()
    {
        if (Input.GetKeyDown(KeyCode.Escape)){
            cursorFreedom = !cursorFreedom;
        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            UIManager.instance.ShowLeaderboard(true);
        } 
        else if (Input.GetKeyUp(KeyCode.Tab))
        {
            UIManager.instance.ShowLeaderboard(false);
        }
    }

    private void OnDestroy()
    {
        LockedMode(false);
        UIManager.instance.InGameMenu(false);
    }

    #endregion
}
