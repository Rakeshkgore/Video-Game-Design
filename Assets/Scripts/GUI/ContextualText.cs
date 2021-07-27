using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContextualText : MonoBehaviour
{
    public static ContextualText Singleton { get; private set; }

    [TextArea]
    public string separator = "\n. . . . .\n";

    private Text text;
    private List<string> texts = new List<string>();

    void Awake()
    {
        if (Singleton == null)
        {
            Singleton = this;
        }
        else
        {
            Debug.LogError($"ContextualText script exists on both {Singleton.name} and {name}! {name} will be ignored.");
        }

        text = GetComponentInChildren<Text>();
        gameObject.SetActive(false);
    }

    public static void Show(string text)
    {
        Singleton.texts.Add(text);
        Singleton.UpdateText();
    }

    public static void Hide(string text)
    {
        Singleton.texts.Remove(text);
        Singleton.UpdateText();
    }

    public static void ShowFor(string text, float duration)
    {
        Singleton.texts.Add(text);
        Singleton.UpdateText();
        Singleton.RemoveAfter(text, duration);
    }

    private void UpdateText()
    {
        if (texts.Count == 0)
        {
            gameObject.SetActive(false);
        }
        else
        {
            text.text = string.Join(separator, texts);
            gameObject.SetActive(true);
        }
    }

    private void RemoveAfter(string text, float duration)
    {
        StartCoroutine(RemoveAfterCoro(text, duration));
    }

    private IEnumerator RemoveAfterCoro(string text, float duration)
    {
        yield return new WaitForSeconds(duration);
        texts.Remove(text);
        UpdateText();
    }
}
