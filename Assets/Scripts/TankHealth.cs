using UnityEngine;
using UnityEngine.UI; // UI 사용 필수

public class TankHealth : MonoBehaviour
{
    public float startingHealth = 100f;          // 시작 체력
    public GameObject explosionPrefab;           // 죽을 때 터지는 이펙트
    public Text healthText;                      // (UI) HP 표시 텍스트

    private float currentHealth;
    private bool dead;

    private void OnEnable()
    {
        currentHealth = startingHealth;
        dead = false;
        UpdateHealthUI();
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        UpdateHealthUI(); // 맞을 때마다 UI 갱신

        if (currentHealth <= 0f && !dead)
        {
            OnDeath();
        }
    }

    private void UpdateHealthUI()
    {
        if (healthText != null)
        {
            healthText.text = "HP: " + Mathf.Max(0, (int)currentHealth);
        }
    }

    private void OnDeath()
    {
        dead = true;

        // 폭발 이펙트 생성
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, transform.rotation);
        }

        // 카메라 살리기
        if (Camera.main != null && Camera.main.transform.IsChildOf(transform))
        {
            Camera.main.transform.parent = null;
        }
    }
}