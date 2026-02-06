using UnityEngine;

public class Shell : MonoBehaviour
{
    [SerializeField]
    private GameObject explosionPrefab;    // Explosion 프리팹

    [SerializeField]
    private float shellDamage = 50f;       // 포탄의 데미지

    private void OnCollisionEnter(Collision collision)
    {
        // 데미지 처리
        MonsterController enemy = collision.gameObject.GetComponent<MonsterController>();
        if (enemy != null)
        {
            enemy.TakeDamage(shellDamage); // 데미지 입히기
        }

        // 이펙트 생성
        if (explosionPrefab != null)
        {
            // 포탄이 있는 위치에 폭발 이펙트
            GameObject explosion = Instantiate(explosionPrefab, transform.position, transform.rotation);

            // 폭발 이펙트는 삭제 시간
            Destroy(explosion, 3.0f);
        }

        // 포탄 삭제
        Destroy(gameObject);
    }
}