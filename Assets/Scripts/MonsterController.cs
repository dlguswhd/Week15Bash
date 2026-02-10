using UnityEngine;
using UnityEngine.AI;

public class MonsterController : MonoBehaviour
{
    [SerializeField]
    private float maxHp = 100f;
    [SerializeField]
    private float walkSpeed = 2.0f;
    [SerializeField]
    private float runSpeed = 6.0f;
    [SerializeField]
    public float attackDamage = 10f;

    [SerializeField]
    private float detectRange = 10f;
    [SerializeField]
    private float attackRange = 2.0f;
    [SerializeField]
    private float attackCooldown = 1.5f;

    [SerializeField]
    private Transform player; // 플레이어(탱크)의 Transform

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

        // 플레이어가 연결 안 되어 있으면 자동으로 찾기
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        SetPatrolPoint();
    }

    void Update()
    {
        if (isDead) return; // 죽었으면 아무것도 안 함
        Detect();
    }

    private void Detect()
    {
        if (player == null) return;

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

        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            anim.SetFloat("Speed", 0f);
            patrolTimer += Time.deltaTime;

            if (patrolTimer >= 2.0f)
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
        agent.isStopped = true; // 공격할 땐 멈춤
        anim.SetFloat("Speed", 0f);

        // 플레이어 바라보기
        Vector3 dir = player.position - transform.position;
        dir.y = 0;
        if (dir != Vector3.zero) // 회전 오류 방지
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 10f * Time.deltaTime);
        }

        // 쿨타임 체크
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            // 공격 애니메이션 실행
            anim.SetTrigger("Attack");
            lastAttackTime = Time.time;

            TankHealth targetHealth = player.GetComponent<TankHealth>();

            if (targetHealth != null)
            {
                targetHealth.TakeDamage(attackDamage);
                Debug.Log($"몬스터 공격! 탱크 HP 감소. 데미지: {attackDamage}");
            }
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

        agent.enabled = false;
        anim.SetFloat("Speed", 0f);

        anim.SetBool("Die", true);
        anim.SetTrigger("Die");

        GetComponent<Collider>().enabled = false;
        Destroy(gameObject, 3.0f);
    }

    private void SetPatrolPoint()
    {
        Vector3 randomPos = Random.insideUnitSphere * 10f + transform.position;
        if (NavMesh.SamplePosition(randomPos, out NavMeshHit hit, 10f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            TankHealth targetHealth = collision.gameObject.GetComponent<TankHealth>();
            if (targetHealth != null && Time.time - lastAttackTime >= attackCooldown)
            {
                targetHealth.TakeDamage(attackDamage);
                lastAttackTime = Time.time;
            }
        }
    }
}