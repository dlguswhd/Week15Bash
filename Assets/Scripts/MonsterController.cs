using UnityEngine;
using UnityEngine.AI;

public class MonsterController : MonoBehaviour
{
    [SerializeField] 
    private float maxHp = 100f; //몬스터 스탯

    [SerializeField] 
    private float walkSpeed = 2.0f; // 순찰(Patrol) 상태일 때의 이동 속도

    [SerializeField] 
    private float runSpeed = 6.0f; // 추적(Chase) 상태일 때의 이동 속도

    [SerializeField]
    private float rotationSmoothness = 10f; // 회전

    [SerializeField]
    private float detectRange = 10f; // 감지 및 공격 범위

    [SerializeField] 
    private float attackRange = 2.0f; // 공격 범위

    [SerializeField] 
    private float attackCooldown = 1.5f; // 공격 쿨타임

    [SerializeField] 
    private Transform player;


    private Animator anim;
    private NavMeshAgent agent;
    private Collider playerCollider; 

    // 상태 확인용
    private float currentHp;
    private bool isDead = false;
    private float lastAttackTime = 0f;

    // 순찰 관련
    private Vector3 patrolDestination;
    private float patrolTimer;
    private float patrolWaitTime = 2f; // 목적지 도착 후 대기 시간

    void Start()
    {
        // 컴포넌트 가져오기
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        // 초기화
        currentHp = maxHp;
        InitializePlayerReference(); // 플레이어 콜라이더 찾기
        SetNewPatrolDestination();   // 첫 순찰 지점 설정
    }

    void Update()
    {
        // 죽었거나 플레이어가 없으면 AI 정지
        if (isDead || player == null) return;

        UpdateAIBehavior();
    }

    private void UpdateAIBehavior()
    {
        // 목표 지점 계산
        Vector3 targetPoint = GetTargetPosition();

        //  계산
        float distanceToTarget = Vector3.Distance(transform.position, targetPoint);

        // 거리별 행동 결정
        if (distanceToTarget <= attackRange)
        {
            // 공격 범위 내: 멈춰서 공격
            PerformAttack(targetPoint);
        }
        else if (distanceToTarget <= detectRange)
        {
            // 감지 범위 내: 뛰어서 추적
            ChasePlayer(targetPoint);
        }
        else
        {
            // 감지 범위 밖: 평화롭게 순찰
            Patrol();
        }
    }

    // 플레이어의 콜라이더 유무에 따라 정확한 타격 위치 반환
    private Vector3 GetTargetPosition()
    {
        if (playerCollider != null)
        {
            return playerCollider.ClosestPoint(transform.position);
        }
        return player.position; // 콜라이더가 없으면 중심점 반환
    }

    // 시작 시 플레이어의 콜라이더를 찾기
    private void InitializePlayerReference()
    {
        if (player != null)
        {
            playerCollider = player.GetComponent<Collider>();
        }
    }


    // 순찰: 랜덤한 위치로 천천히 걸어다님
    private void Patrol()
    {
        agent.isStopped = false;
        agent.speed = walkSpeed;
        anim.SetFloat("Speed", 0.5f); // 걷기 애니메이션

        // 목적지에 거의 도착했는지 확인
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            patrolTimer += Time.deltaTime;
            anim.SetFloat("Speed", 0f); // 도착했으므로 잠시 대기

            // 대기 시간이 지나면 새로운 목적지로 출발
            if (patrolTimer >= patrolWaitTime)
            {
                SetNewPatrolDestination();
                patrolTimer = 0f;
            }
        }
    }

    // 추적: 플레이어를 발견하고 뛰어감
    private void ChasePlayer(Vector3 targetPoint)
    {
        agent.isStopped = false;
        agent.speed = runSpeed;

        agent.SetDestination(targetPoint); // 탱크 표면을 향해 이동
        anim.SetFloat("Speed", 1.0f);      // 뛰기 애니메이션
    }

    // 공격: 제자리에 멈춰서 플레이어를 바라보고 공격
    private void PerformAttack(Vector3 targetPoint)
    {
        agent.isStopped = true;
        anim.SetFloat("Speed", 0f); // 정지

        // 공격 중에도 어색하지 않게 대상을 바라봄
        RotateTowards(targetPoint);

        // 쿨타임
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            anim.SetTrigger("Attack");
            lastAttackTime = Time.time;
        }
    }

    // 다음 순찰 지점을 랜덤하게 설정
    private void SetNewPatrolDestination()
    {
        // 현재 위치 기준 반경 10m 내의 랜덤 좌표 생성
        Vector3 randomDirection = Random.insideUnitSphere * 10f;
        randomDirection += transform.position;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, 10f, 1))
        {
            patrolDestination = hit.position;
            agent.SetDestination(patrolDestination);
        }
    }

    // 타겟 방향으로 회전
    private void RotateTowards(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        direction.y = 0; // 위아래 기울임 방지 (수평 회전만)

        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSmoothness);
        }
    }

    // 피격
    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHp -= damage;

        if (currentHp <= 0)
        {
            Die();
        }
        else
        {
            anim.SetTrigger("Hit");
        }
    }

    // 사망 처리
    private void Die()
    {
        isDead = true;
        agent.isStopped = true;     // 이동 정지
        anim.SetBool("Die", true);  // 사망 애니메이션

        // 시체가 길을 막지 않도록 콜라이더 비활성화
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        // 3초 뒤 오브젝트 삭제
        Destroy(gameObject, 3f);
    }
}