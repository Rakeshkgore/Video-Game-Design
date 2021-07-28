using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class EnemyHealthSlider : MonoBehaviour
{
    public float maxEnemyDistance = 30f;
    public Slider enemyHealthSlider;
    public GameObject enemyHealthFillArea;
    public Text enemyHealthText;
    public GetHealth[] enemies;
    public Transform player;
    public Camera mainCamera;

    void Awake()
    {
        enemyHealthSlider = GetComponent<Slider>();
        enemies = FindObjectsOfType<GetHealth>();
    }

    // Update is called once per frame
    void Update()
    {
        bool showHealthBar = false;
        string currentEnemyName = "";
        float currentEnemyDistance = float.PositiveInfinity;
        float currentEnemyHp = 1f;
        float currentEnemyMaxHp = 1f;

        foreach (GetHealth enemy in enemies)
        {
            float enemyHp = enemy.hp;
            if (enemy.hp <= 0f)
            {
                continue;
            }

            string enemyName = enemy.enemyName;
            if (string.IsNullOrEmpty(enemyName))
            {
                continue;
            }

            Vector3 enemyPosition = enemy.transform.position;
            if (!IsVisibleToPlayer(enemyPosition))
            {
                continue;
            }

            float enemyDistance = Vector3.Distance(player.position, enemyPosition);
            if (enemyDistance <= maxEnemyDistance && enemyDistance < currentEnemyDistance)
            {
                showHealthBar = true;
                currentEnemyName = enemyName;
                currentEnemyDistance = enemyDistance;
                currentEnemyHp = enemyHp;
                currentEnemyMaxHp = enemy.mhp;
            }
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
            viewportPoint.x >= 0.1f
            && viewportPoint.x <= 0.9f
            && viewportPoint.y >= 0.1f
            && viewportPoint.y <= 0.9f
            && viewportPoint.z >= mainCamera.nearClipPlane
            && viewportPoint.z <= mainCamera.farClipPlane
        );
    }
}
