using UnityEngine;
using System.Collections;

public class TankDust : MonoBehaviour
{

    [SerializeField] private ParticleSystem leftDust;
    [SerializeField] private ParticleSystem rightDust;

    private Coroutine stopCoroutine;

    void Update()
    {
        DetectMove();
    }

    private void DetectMove()
    {
        // 입력 감지 (움직이는 중인지 확인)
        float v = Input.GetAxis("Vertical");
        float h = Input.GetAxis("Horizontal");

        // 아주 미세한 입력은 무시
        bool isMoving = Mathf.Abs(v) > 0.1f || Mathf.Abs(h) > 0.1f;

        if (isMoving)
        {
            PlayDust();
        }
        else
        {
            StopDustWithDelay();
        }
    }

    private void PlayDust()
    {
        // 멈추려던 계획이 있다면 취소
        if (stopCoroutine != null)
        {
            StopCoroutine(stopCoroutine);
            stopCoroutine = null;
        }

        // 파티클이 꺼져있다면 재생
        if (!leftDust.isPlaying) leftDust.Play();
        if (!rightDust.isPlaying) rightDust.Play();
    }

    private void StopDustWithDelay()
    {
        if (leftDust.isPlaying && stopCoroutine == null)
        {
            stopCoroutine = StartCoroutine(StopDelayRoutine());
        }
    }

    // 1.5초 뒤에 Rma
    private IEnumerator StopDelayRoutine()
    {
        yield return new WaitForSeconds(1.5f);

        leftDust.Stop();
        rightDust.Stop();

        stopCoroutine = null;
    }
}