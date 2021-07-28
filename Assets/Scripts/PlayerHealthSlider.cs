using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthSlider : MonoBehaviour
{
    public Slider playerHealthSlider;
    public GetHealth playerHealth;
    // Start is called before the first frame update
    void Start()
    {
        playerHealthSlider = GetComponent<Slider>();
        playerHealthSlider.value = playerHealth.hp;
    }

    // Update is called once per frame
    void Update()
    {
        playerHealthSlider.value = playerHealth.hp;
        playerHealthSlider.maxValue = playerHealth.mhp;
    }
}
