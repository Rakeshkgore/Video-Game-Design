using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class EnemyHealthSlider : MonoBehaviour
{
    public Slider enemyHealthSlider;
    public GetHealth golemHealth;
    public GetHealth rhinoHealth;
    public Transform player;
    // Start is called before the first frame update
    void Start()
    {
        enemyHealthSlider = GetComponent<Slider>();

        enemyHealthSlider.value = golemHealth.maxHp;
    }

    // Update is called once per frame
    void Update()
    {
        enemyHealthSlider.minValue = 0;
        enemyHealthSlider.maxValue = player.position.x < -34.33073 ? golemHealth.maxHp : rhinoHealth.maxHp;
        enemyHealthSlider.value = player.position.x < -34.33073 ? golemHealth.hp : rhinoHealth.hp;
    }
}
