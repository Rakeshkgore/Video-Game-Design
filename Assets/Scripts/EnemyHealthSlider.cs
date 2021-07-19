using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class EnemyHealthSlider : MonoBehaviour
{
    public Slider enemyHealthSlider;
    public GameObject enemyHealthFillArea;
    public Text enemyHealthText;
    public GetHealth golemHealth;
    public GetHealth rhinoHealth;
    public Transform player;
    public Camera mainCamera;
    // Start is called before the first frame update
    void Start()
    {
        enemyHealthSlider = GetComponent<Slider>();

        enemyHealthSlider.value = golemHealth.maxHp;
    }

    // Update is called once per frame
    void Update()
    {
        string currentEnemyName;
        float currentEnemyHp;
        float currentEnemyMaxHp;
        bool showHealthBar;

        if (player.position.x < -34.33073) // FIXME
        {
            currentEnemyName = "Rock Golem";
            currentEnemyHp = golemHealth.hp;
            currentEnemyMaxHp = golemHealth.maxHp;
            showHealthBar = currentEnemyHp > 0f && (
                currentEnemyHp < currentEnemyMaxHp
                || IsVisibleToPlayer(golemHealth.transform.position)
            );
        }
        else
        {
            currentEnemyName = "Rhino";
            currentEnemyHp = rhinoHealth.hp;
            currentEnemyMaxHp = rhinoHealth.maxHp;
            showHealthBar = currentEnemyHp > 0f;
        }

        enemyHealthText.gameObject.SetActive(showHealthBar);
        enemyHealthFillArea.SetActive(showHealthBar);
        enemyHealthText.text = currentEnemyName;
        enemyHealthSlider.minValue = 0;
        enemyHealthSlider.maxValue = currentEnemyMaxHp;
        enemyHealthSlider.value = currentEnemyHp;
    }

    private bool IsVisibleToPlayer(Vector3 point)
    {
        Vector3 viewportPoint = mainCamera.WorldToViewportPoint(point);
        return (
            viewportPoint.x >= 0f
            && viewportPoint.x <= 1f
            && viewportPoint.y >= 0f
            && viewportPoint.y <= 1f
            && viewportPoint.z >= mainCamera.nearClipPlane
            && viewportPoint.z <= mainCamera.farClipPlane
        );
    }
}
