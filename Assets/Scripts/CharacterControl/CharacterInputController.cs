using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterInputController : MonoBehaviour {

    public string Name = "George P Burdell";

    private float filteredForwardInput = 0f;
    private float filteredTurnInput = 0f;

    public bool InputMapToCircular = true;

    public float forwardInputFilter = 5f;
    public float turnInputFilter = 5f;

    private float forwardSpeedLimit = 1f;


    public float Forward
    {
        get;
        private set;
    }

    public float Turn
    {
        get;
        private set;
    }

    public bool Action
    {
        get;
        private set;
    }

    public bool Jump
    {
        get;
        private set;
    }

    void OnMove(InputValue value)
    {
        Vector2 movementVector = value.Get<Vector2>();
        Forward = movementVector.y;
        Turn = movementVector.x;
    }

    void OnJump(InputValue value)
    {
        Vector2 movementVector = value.Get<Vector2>();
        Forward = movementVector.y;
        Turn = movementVector.x;
    }

    void OnPause()
    {
        GameObject.FindObjectOfType<PauseMenuToggle>().OnPause();
    }
}
