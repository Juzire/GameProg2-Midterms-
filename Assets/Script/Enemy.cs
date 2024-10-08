using UnityEngine;

public class Enemy : MonoBehaviour
{
    public GameManager gameManager;
    private Color enemyColor;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    public void SetColor(Color color)
    {
        enemyColor = color;
        GetComponent<Renderer>().material.color = enemyColor;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            Color bulletColor = other.GetComponent<Renderer>().material.color;

            if (AreColorsClose(bulletColor, enemyColor))
            {
                gameManager.RemoveEnemy(transform); 
                Destroy(gameObject);
                Destroy(other.gameObject);
            }
            else
            {
                Destroy(other.gameObject);
            }
        }
    }

    private bool AreColorsClose(Color color1, Color color2, float tolerance = 0.1f)
    {
        return Mathf.Abs(color1.r - color2.r) < tolerance &&
               Mathf.Abs(color1.g - color2.g) < tolerance &&
               Mathf.Abs(color1.b - color2.b) < tolerance;
    }
}