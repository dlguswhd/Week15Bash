using UnityEngine;

public class Shell : MonoBehaviour
{
    [SerializeField]
    private GameObject explosionPrefab;    // Explosion 프리팹

    [SerializeField]
    private float shellDamage = 50f;       // 포탄의 데미지

    [SerializeField]
    private float explosionRadius = 5f;    // 폭발 반경
    [SerializeField]
    private float explosionForce = 1000f;  // 폭발 힘
    [SerializeField]
    private LayerMask damageLayer;         // 데미지를 줄 대상 레이어

    private void OnCollisionEnter(Collision collision)
    {
        // 스플래시 데미지 처리
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius, damageLayer);

        foreach (Collider nearbyObject in colliders)
        {
            // 물리적인 폭발 효과
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }

            // 데미지 처리
            MonsterController enemy = nearbyObject.GetComponent<MonsterController>();
            if (enemy != null)
            {
                enemy.TakeDamage(shellDamage); // 데미지 입히기
            }
        }

        // 이펙트 생성
        if (explosionPrefab != null)
        {
            // 포탄이 있는 위치에 폭발 이펙트
            GameObject explosion = Instantiate(explosionPrefab, transform.position, transform.rotation);

            // 폭발 이펙트 삭제 시간
            Destroy(explosion, 3.0f);
        }

        // 포탄 삭제
        Destroy(gameObject);
    }
}