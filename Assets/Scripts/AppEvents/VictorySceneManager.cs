using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class VictorySceneManager : MonoBehaviour
{
    CanvasGroup canvasGroup;

    // Start is called before the first frame update
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        StartCoroutine(FadeScene());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator FadeScene()
    {
        Debug.Log("fade called");
        float duration = 3f;
        float counter = 0f;
        while (counter < duration) {
            Debug.Log("While");
            Debug.Log(canvasGroup.alpha);
            counter += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0, 1, counter / duration);
            yield return null;
        }
        Destroy(this);
    }
}
