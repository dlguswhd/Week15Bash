using UnityEngine;

public class ShellExplosion : MonoBehaviour
{
    [SerializeField]
    private GameObject explosionPrefab;    // Explosion 프리팹

    // 포탄이 다른 물체와 부딪혔을 때 자동으로 실행
    private void OnCollisionEnter(Collision collision)
    {
        //  이펙트 생성
        if (explosionPrefab != null)
        {
            // 포탄이 있는 위치에 폭발 이펙트
            GameObject explosion = Instantiate(explosionPrefab, transform.position, transform.rotation);

            // 폭발 이펙트는 연기가 다 사라질 때쯤 삭제
            Destroy(explosion, 3.0f);
        }

        // 포탄 삭제
        Destroy(gameObject);
    }
}