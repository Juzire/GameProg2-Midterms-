using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public List<Transform> enemyPos = new List<Transform>();
    public float distance;
    public float rangeRadius;
    public GameObject bulletPrefab;
    public Transform shootPoint;
    public float bulletSpeed = 10f;
    private Color[] colors = { Color.blue, Color.green, Color.red }; 
    public Renderer playerRenderer;
    public Transform player;
    public float enemySpeed = 2f;

    private Color playerColor;
    private bool isGameOver = false;

    public Canvas gameOverCanvas;
    public TextMeshProUGUI gameOverText;
    public Image gameOverImage;

    private bool enemyInRange = false;

    void Start()
    {
        ChangePlayerColor(); 
        foreach (var enemy in enemyPos)
        {
            Color enemyColor = GetRandomColor();
            enemy.GetComponent<Enemy>().SetColor(enemyColor);
        }
        gameOverCanvas.gameObject.SetActive(false);
    }

    void Update()
    {
        if (isGameOver) return;

        Transform closestEnemy = null;  
        float closestDistance = Mathf.Infinity;  

        foreach (var enemy in enemyPos)
        {
            if (enemy == null) continue;
            distance = Vector3.Distance(transform.position, enemy.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy;
            }

            if (distance <= rangeRadius && !enemyInRange)
            {
                enemyInRange = true;
                InvokeRepeating("ShootBullet", 0, 1f);
            }

            enemy.position = Vector3.MoveTowards(enemy.position, player.position, enemySpeed * Time.deltaTime);

            if (distance <= 0.5f)
            {
                GameOver();
            }
        }

        if (closestEnemy != null && closestDistance <= rangeRadius)
        {
            transform.LookAt(closestEnemy.position); 
        }

        if (Input.GetMouseButtonDown(0))
        {
            ChangePlayerColor();
        }
    }


    void ShootBullet()
    {
        if (enemyInRange && !isGameOver)
        {
            Transform targetEnemy = null;
            foreach (var enemy in enemyPos)
            {
                if (enemy == null) continue;

                float distanceToEnemy = Vector3.Distance(transform.position, enemy.position);
                if (distanceToEnemy <= rangeRadius)
                {
                    targetEnemy = enemy;
                    break;
                }
            }

            if (targetEnemy != null)
            {
                if (bulletPrefab != null)
                {
                    GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, Quaternion.identity);
                    bullet.GetComponent<Renderer>().material.color = playerColor;
                    StartCoroutine(MoveBullet(bullet, targetEnemy));
                    Destroy(bullet, 5f);

                }
            }
        }
    }

    IEnumerator MoveBullet(GameObject bullet, Transform targetEnemy)
    {
        if (bullet == null || targetEnemy == null)
        {
            yield break;  
        }

        Vector3 direction = (targetEnemy.position - bullet.transform.position).normalized;

        while (bullet != null && targetEnemy != null)
        {
            bullet.transform.position += direction * bulletSpeed * Time.deltaTime;

            if (Vector3.Distance(bullet.transform.position, targetEnemy.position) < 0.5f)
            {
                Debug.Log("Bullet hit the enemy!");
                Destroy(bullet);  
                RemoveEnemy(targetEnemy); 
                yield break;  
            }

            yield return null;  
        }

        if (bullet != null)
        {
            Destroy(bullet);
        }
    }

    void ChangePlayerColor()
    {
        playerColor = GetRandomColor();
        playerRenderer.material.color = playerColor;
    }

    Color GetRandomColor()
    {
        return colors[Random.Range(0, colors.Length)];
    }

    public void RemoveEnemy(Transform enemyTransform)
    {
        if (enemyPos.Contains(enemyTransform))
        {
            enemyPos.Remove(enemyTransform);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(player.position, rangeRadius);
    }

    void GameOver()
    {
        isGameOver = true;
        CancelInvoke("ShootBullet");  
        gameOverCanvas.gameObject.SetActive(true);
        gameOverText.gameObject.SetActive(true);
        gameOverImage.gameObject.SetActive(true); 
    }
}
