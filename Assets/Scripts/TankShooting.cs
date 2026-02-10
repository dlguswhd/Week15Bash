using System.Collections;
using UnityEngine;
using UnityEngine.UI; 

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


    [SerializeField]
    private int maxAmmo = 20;       // 최대 탄약 수
    private int currentAmmo;        // 현재 남은 탄약

    [SerializeField]
    private Text reloadTimerText; // 남은 시간 표시 
    [SerializeField]
    private Text reloadLabelText; // 텍스트 애니메이션
    [SerializeField]
    private Text ammoText;        // 남은 탄약 표시 

    private void Start()
    {
        currentAmmo = maxAmmo; // 탄약 가득 채우기
        UpdateAmmoUI();        // UI 갱신
    }

    void Update()
    {
        ReloadTime();
        UpdateUI();
    }

    private void Fire()
    {
        // 탄약 감소 및 UI 갱신
        currentAmmo--;
        UpdateAmmoUI();

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

    private void ReloadTime()
    {
        if (Input.GetMouseButtonDown(0) && Time.time >= nextFireTime && currentAmmo > 0)
        {
            Fire();
            // 다음 발사 가능 시간 갱신
            nextFireTime = Time.time + reloadTime;

            // 발사 시 "Reloading..." 점 애니메이션 코루틴 시작
            StartCoroutine(AnimateReloadLabel());
        }
    }

    private void UpdateUI()
    {
        // 장전 중이라면
        if (Time.time < nextFireTime)
        {
            // UI 켜기
            if (reloadTimerText != null) reloadTimerText.gameObject.SetActive(true);
            if (reloadLabelText != null) reloadLabelText.gameObject.SetActive(true);

            // 남은 시간 계산
            float timeLeft = nextFireTime - Time.time;

            // 타이머 텍스트 갱신
            if (reloadTimerText != null)
            {
                reloadTimerText.text = timeLeft.ToString("F1") + "s";
            }
        }
        else
        {
            // 장전이 끝났으면 UI 끄기
            if (reloadTimerText != null) reloadTimerText.gameObject.SetActive(false);
            if (reloadLabelText != null) reloadLabelText.gameObject.SetActive(false);
        }
    }

    // 탄약 UI 갱신
    private void UpdateAmmoUI()
    {
        if (ammoText != null)
        {
            ammoText.text = "Shells: " + currentAmmo + " / " + maxAmmo;
        }
    }

    // Reloading 글자 애니메이션
    IEnumerator AnimateReloadLabel()
    {
        // 장전 중일 때만
        while (Time.time < nextFireTime)
        {
            if (reloadLabelText != null) reloadLabelText.text = "Reloading.";
            yield return new WaitForSeconds(0.5f);

            if (Time.time >= nextFireTime) break; //  종료
            if (reloadLabelText != null) reloadLabelText.text = "Reloading..";
            yield return new WaitForSeconds(0.5f);

            if (Time.time >= nextFireTime) break;
            if (reloadLabelText != null) reloadLabelText.text = "Reloading...";
            yield return new WaitForSeconds(0.5f);
        }
    }
}