using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerCount : MonoBehaviour
{
    public int Count { get; private set; } = 0;
    public bool IsTriggered { get => Count > 0; }

    void OnTriggerEnter()
    {
        ++Count;
    }

    void OnTriggerExit()
    {
        --Count;
    }
}
