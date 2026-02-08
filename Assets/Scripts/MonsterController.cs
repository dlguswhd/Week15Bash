using UnityEngine;
using UnityEngine.AI;

public class MonsterController : MonoBehaviour
{
    [SerializeField] private float maxHp = 100f;
    [SerializeField] private float walkSpeed = 2.0f;
    [SerializeField] private float runSpeed = 6.0f;

    [SerializeField] private float detectRange = 10f;
    [SerializeField] private float attackRange = 2.0f;
    [SerializeField] private float attackCooldown = 1.5f;

    [SerializeField] private Transform player;

    private Animator anim;
    private NavMeshAgent agent;
    private float currentHp;
    private float lastAttackTime;
    private bool isDead;
    private float patrolTimer;

    void Start()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        currentHp = maxHp;
        SetPatrolPoint(); // 첫 순찰지점 설정
    }

    void Update()
    {
        Detect();
    }

    private void Detect()
    {
        if (isDead || player == null) return;

        float dist = Vector3.Distance(transform.position, player.position);

        if (dist <= attackRange) Attack();      // 공격 범위
        else if (dist <= detectRange) Chase();  // 감지 범위
        else Patrol();
    }

    // 순찰(Patrol)
    private void Patrol()
    {
        agent.isStopped = false;
        agent.speed = walkSpeed;
        anim.SetFloat("Speed", 0.5f);

        // 목적지 도착 체크
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            anim.SetFloat("Speed", 0f); // 대기
            patrolTimer += Time.deltaTime;

            if (patrolTimer >= 2.0f) // 2초 대기 후 이동
            {
                SetPatrolPoint();
                patrolTimer = 0f;
            }
        }
    }

    // 추적(Chase)
    private void Chase()
    {
        agent.isStopped = false;
        agent.speed = runSpeed;
        agent.SetDestination(player.position);
        anim.SetFloat("Speed", 1.0f);
    }

    // 공격(Attack)
    private void Attack()
    {
        agent.isStopped = true;
        anim.SetFloat("Speed", 0f);

        // 플레이어 바라보기 (Y축만 회전)
        Vector3 dir = player.position - transform.position;
        dir.y = 0;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 10f * Time.deltaTime);

        if (Time.time - lastAttackTime >= attackCooldown)
        {
            anim.SetTrigger("Attack");
            lastAttackTime = Time.time;
        }
    }

    // 피격 및 사망
    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHp -= damage;
        if (currentHp <= 0) Die();
        else anim.SetTrigger("Hit");
    }

    private void Die()
    {
        isDead = true;

        // [핵심] 죽을 때 물리/이동 완전히 끄기
        agent.enabled = false;      // 네비메쉬 끄기
        anim.SetFloat("Speed", 0f); // 달리기 모션 강제 종료

        anim.SetBool("Die", true);
        anim.SetTrigger("Die");

        GetComponent<Collider>().enabled = false; // 시체 충돌 무시
        Destroy(gameObject, 3.0f);
    }

    // 랜덤 순찰지점 생성
    private void SetPatrolPoint()
    {
        Vector3 randomPos = Random.insideUnitSphere * 10f + transform.position;
        if (NavMesh.SamplePosition(randomPos, out NavMeshHit hit, 10f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }
}