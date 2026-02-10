using UnityEngine;

public class TankController : MonoBehaviour
{

    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private float rotateSpeed = 100.0f;

    [SerializeField] private float turretSpeed = 150.0f;
    [SerializeField] private float gunSpeed = 100.0f;
    [SerializeField] private float maxGunAngle = 20.0f;
    [SerializeField] private float minGunAngle = -9.0f;
    [SerializeField] private bool reverseGunAngle = false; // 포신 반전 여부

    [SerializeField] private GameObject tankTower;
    [SerializeField] private GameObject tankGun;
    [SerializeField] private GameObject cameraPivot;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private LayerMask aimLayerMask;

    private float camX, camY;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (mainCamera == null) mainCamera = Camera.main;

        // 초기 카메라 각도
        if (cameraPivot != null)
        {
            Vector3 rot = cameraPivot.transform.localRotation.eulerAngles;
            camX = rot.y;
            camY = rot.x;
        }
    }

    void Update()
    {
        HandleMove();   // 이동
        HandleCamera(); // 카메라
        HandleAim();    // 조준
    }

    // 탱크 이동 (WASD)
    private void HandleMove()
    {
        float v = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
        float h = Input.GetAxis("Horizontal") * rotateSpeed * Time.deltaTime;

        transform.Translate(0, 0, v);
        transform.Rotate(0, h, 0);
    }

    // 카메라 회전 (마우스)
    private void HandleCamera()
    {
        if (!cameraPivot) return;

        camX += Input.GetAxis("Mouse X") * 2.0f;
        camY -= Input.GetAxis("Mouse Y") * 2.0f;
        camY = Mathf.Clamp(camY, -40f, 60f); // 상하 각도 제한

        cameraPivot.transform.localRotation = Quaternion.Euler(camY, camX, 0);
    }

    // 조준 (포탑 & 포신)
    private void HandleAim()
    {
        if (!mainCamera) return;

        // 화면 중앙 레이캐스트
        Ray ray = mainCamera.ViewportPointToRay(Vector3.one * 0.5f);
        Vector3 target = Physics.Raycast(ray, out RaycastHit hit, 1000f, aimLayerMask)
                         ? hit.point
                         : ray.GetPoint(1000f);

        // 포탑 좌우 회전 (Y축)
        if (tankTower)
        {
            Vector3 dir = target - tankTower.transform.position;
            dir.y = 0; // 높이 무시

            if (dir != Vector3.zero)
            {
                Quaternion rot = Quaternion.LookRotation(dir);
                tankTower.transform.rotation = Quaternion.RotateTowards(
                    tankTower.transform.rotation, rot, turretSpeed * Time.deltaTime);
            }
        }

        // 포신 상하 회전
        if (tankGun)
        {
            Vector3 dir = target - tankGun.transform.position;
            Vector3 localDir = tankTower.transform.InverseTransformDirection(dir); 

            float angle = Mathf.Atan2(localDir.y, localDir.z) * Mathf.Rad2Deg;

            angle = reverseGunAngle ? angle : -angle;

            // 각도 제한
            angle = Mathf.Clamp(angle, minGunAngle, maxGunAngle);
            Quaternion rot = Quaternion.Euler(angle, 0, 0);

            tankGun.transform.localRotation = Quaternion.RotateTowards(
                tankGun.transform.localRotation, rot, gunSpeed * Time.deltaTime);
        }
    }
}