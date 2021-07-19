using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthText : MonoBehaviour
{
    private Text healthText;
    public Transform player;
    public GetHealth golemHealth;
    public GetHealth rhinoHealth;
    // Start is called before the first frame update
    void Start()
    {
        healthText = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        // FIXME
        healthText.text = "Enemy Health: " + (player.position.x < -34.33073 ? golemHealth.hp : rhinoHealth.hp);
    }
}
