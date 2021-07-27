using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveText : MonoBehaviour
{
    public static ObjectiveText Singleton { get; private set; }

    private Text text;

    void Awake()
    {
        if (Singleton == null)
        {
            Singleton = this;
        }
        else
        {
            Debug.LogError($"ObjectiveText script exists on both {Singleton.name} and {name}! {name} will be ignored.");
        }

        text = GetComponentInChildren<Text>();
    }

    public static void SetObjective(string text)
    {
        Singleton.text.text = "Objective: " + text;
    }
}
