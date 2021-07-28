using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InvisibilitySlider : MonoBehaviour
{
    public Slider invisibilitySlider;
    public GameObject invisibilityFillArea;
    public GameObject invisibilityText;
    public Invisibility player;
    private GetBlessed gb;

    // Start is called before the first frame update
    void Start()
    {
        invisibilitySlider = GetComponent<Slider>();
        gb = player.GetComponent<GetBlessed>();

        Update();
    }

    // Update is called once per frame
    void Update()
    {
        invisibilityText.SetActive(gb.DaedalusPassed);
        invisibilityFillArea.SetActive(gb.DaedalusPassed);
        invisibilitySlider.value = Mathf.Clamp01((player.invisibleUntil - Time.time) / player.duration);
    }
}
