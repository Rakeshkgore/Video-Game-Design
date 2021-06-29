using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class PauseMenuToggle : MonoBehaviour
{
    private CanvasGroup cg;

    void Awake()
    {
        cg = GetComponent<CanvasGroup>();
        if (cg == null)
            Debug.Log("Canvas Group could not be found");
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if(cg.interactable)
            {
                cg.interactable = false;
                cg.blocksRaycasts = false;
                cg.alpha = 0f;
                Time.timeScale = 1f;
            } else
            {
                cg.interactable = true;
                cg.blocksRaycasts = true;
                cg.alpha = 1f;
                Time.timeScale = 0f;
            }
        }
    }
}
