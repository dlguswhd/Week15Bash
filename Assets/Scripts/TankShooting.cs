using UnityEngine;

public class TankShooting : MonoBehaviour
{
    [SerializeField]
    public GameObject shellPrefab;    // Shell 프리팹
    [SerializeField]
    private Transform firePoint;    // 포구 끝 위치
    [SerializeField]
    private float launchForce = 1500f;    // 포탄이 날아가는 힘
    [SerializeField]
    private float reloadTime = 6.5f;  // 제장전 시간

    private float nextFireTime = 0f;

    [SerializeField]
    private GameObject muzzleFlashPrefab;    // muzzleFlash 설정

    void Update()
    {
        ReloadTime();
    }

    private void Fire()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // 발사 이펙트
            if (muzzleFlashPrefab != null)
            {
                // 포구 위치(firePoint)에 불꽃 이펙트를 생성
                GameObject flash = Instantiate(muzzleFlashPrefab, firePoint.position, firePoint.rotation);

                // 불꽃은 1초 뒤에 삭제
                Destroy(flash, 0.1f);
            }

            // 포탄을 firePoint 위치에 생성
            GameObject shell = Instantiate(shellPrefab, firePoint.position, firePoint.rotation);

            // 생성된 포탄의 Rigidbody를 가져온다
            Rigidbody rb = shell.GetComponent<Rigidbody>();

            // firePoint의 앞방향으로 launchForce만큼 힘을 준다
            rb.AddForce(firePoint.forward * launchForce);

            // 포탄이 사라지지 않을 떄, 5초 뒤에 삭제
            Destroy(shell, 5.0f);
        }
    }

    private void ReloadTime()
    {
        if (Input.GetMouseButtonDown(0) && Time.time >= nextFireTime)
        {
            Fire();
            // 다음 발사 가능 시간 갱신
            nextFireTime = Time.time + reloadTime;
        }
    }
}