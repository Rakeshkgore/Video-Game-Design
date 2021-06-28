using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class GameQuitter : MonoBehaviour
{
    private AudioSource clickAudio;
    public void QuitGameWithSound()
    {
        StartCoroutine(btnClickSound());
    }
    IEnumerator btnClickSound()
    {
        clickAudio = GetComponent<AudioSource>();
        clickAudio.Play();
        yield return new WaitForSeconds(clickAudio.clip.length*0.2f);
        QuitGame();
    }
    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}