using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class GameStarter : MonoBehaviour
{
    private AudioSource clickAudio;
    public void StartGameWithSound()
    {
        StartCoroutine(btnClickSound());
    }
    IEnumerator btnClickSound()
    {
        clickAudio = GetComponent<AudioSource>();
        clickAudio.Play();
        yield return new WaitForSeconds(clickAudio.clip.length*0.2f);
        StartGame();
    }
    public void StartGame()
    {
        SceneManager.LoadScene("SampleScene");
    }
}