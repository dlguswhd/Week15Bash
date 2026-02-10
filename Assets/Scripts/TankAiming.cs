using UnityEngine;
using UnityEngine.UI;

public class TankAiming : MonoBehaviour
{
    [SerializeField]
    private Camera mainCamera;  // 메인 카메라를
    [SerializeField]
    private float normalFOV = 60f;  // 평소 시야
    [SerializeField]
    private float aimFOV = 20f;     // 조준할 때 시야
    [SerializeField]
    private float zoomSpeed = 10f;  // 줌이 되는 속도

    [SerializeField]
    private GameObject crosshairUI; // 화면 중앙 조준점
    [SerializeField]
    private GameObject scopeUI;     // 조준경 이미지

    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            // 우클릭 중일 때: 줌인을 하고 조준경 UI
            ZoomIn();
        }
        else
        {
            // 우클릭을 뗐을 때: 일반상태
            ZoomOut();
        }
    }

    private void ZoomIn()
    {
        mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, aimFOV, Time.deltaTime * zoomSpeed);

        // UI 교체
        if (scopeUI != null) scopeUI.SetActive(true);
        if (crosshairUI != null) crosshairUI.SetActive(false);
    }

    private void ZoomOut()
    {
        mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, normalFOV, Time.deltaTime * zoomSpeed);

        // UI 교체
        if (scopeUI != null) scopeUI.SetActive(false);
        if (crosshairUI != null) crosshairUI.SetActive(true);
    }
}