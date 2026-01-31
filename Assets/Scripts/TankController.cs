using UnityEngine;

public class TankController : MonoBehaviour
{

    [SerializeField]
    private float moveSpeed = 5.0f;    // 탱크 움직이는 속도
    [SerializeField]
    private float rotateSpeed = 100.0f;    // 탱크 회전속도
    [SerializeField]
    private float mouseSensitivity = 1.5f;    // 포탑 회전 속도
    [SerializeField]
    private float smoothSpeed = 5.0f;    // 포탑 회전 부드러움

    [SerializeField]
    private GameObject tankTower;
    [SerializeField]
    private GameObject tankGun;
    [SerializeField]
    private GameObject cameraPivot;

    private float currentGunAngle = 0f;

    void Update()
    {
        TankMove(); // 탱크 움직임
    }

    private void TankMove()
    {
        // 탱크 방향키 입력
        float moveInput = Input.GetAxis("Vertical");
        float rotateInput = Input.GetAxis("Horizontal");
        
        // 탱크 움직임 계산
        float move = moveInput * moveSpeed * Time.deltaTime;
        float rotate = rotateInput * rotateSpeed * Time.deltaTime;

        transform.Translate(0, 0, move);
        transform.Rotate(0, rotate, 0);

        // 포탑 좌우 회전
        if (tankTower != null)
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            tankTower.transform.Rotate(0, mouseX, 0);
        }

        // 포신 위아래 회전
        if (tankGun != null)
        {
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            currentGunAngle += mouseY;

            // 앙각 +20도, 부각 -9도
            currentGunAngle = Mathf.Clamp(currentGunAngle, -9f, 20f);

            tankGun.transform.localRotation = Quaternion.Euler(-currentGunAngle, 0, 0);

            if(cameraPivot != null)
            {
                cameraPivot.transform.localRotation = Quaternion.Euler(-currentGunAngle, 0, 0);
            }
        }
    }
}
