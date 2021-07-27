using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class Credits : MonoBehaviour
{
    private AudioSource clickAudio;
    public void CreditsSceneWithSound()
    {
        StartCoroutine(btnClickSound());
    }
    IEnumerator btnClickSound()
    {
        clickAudio = GetComponent<AudioSource>();
        clickAudio.Play();
        yield return new WaitForSeconds(clickAudio.clip.length*0.2f);
        CreditsScene();
    }
    public void CreditsScene()
    {
        SceneManager.LoadScene("Credits");
    }
}