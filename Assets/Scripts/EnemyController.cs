using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyController : MonoBehaviour
{

    enum AIState 
    {
        Idle, Patrolling, Chasing
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    [Header("Patrol")]
    [SerializeField] private Transform wayPoints;
    [SerializeField] private float waitAtPoint = 2f; 
    private int currentWaypoint;
    private float waitCounter;

    [Header("Components")]
    NavMeshAgent agent;
    private Animator animator; // 🚨 Animator 참조 추가

    [Header("AI States")] 
    [SerializeField] private AIState currentState;

    [Header("Chasing")] 
    [SerializeField] private float chaseRange;
    [SerializeField] private float patrolSpeed; // 🚨 순찰 속도 추가
    [SerializeField] private float chaseSpeed;  // 🚨 추격 속도 추가

    [Header("Suspicious")]
    [SerializeField] private float suspiciousTime;
    private float timeSinceLastSawPlayer;

    private GameObject player;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>(); // 🚨 Animator 컴포넌트 가져오기
        player = GameObject.FindGameObjectWithTag("Player");

        waitCounter = waitAtPoint;
        timeSinceLastSawPlayer = suspiciousTime;

        // 🚨 시작 시 AI 정지 및 순찰 속도 설정
        agent.isStopped = true;
        agent.speed = patrolSpeed;

    }


    private void Update()
    {

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        // 🚨 1. 애니메이션 제어: agent의 실제 이동 속도를 Animator의 "Speed" 파라미터에 전달
        if (animator != null)
        {
            animator.SetFloat("Speed", agent.velocity.magnitude); 
        }


        // 🚨 2. 추격 우선순위 체크 (모든 상태보다 먼저 체크)
        if (distanceToPlayer <= chaseRange)
        {
            if (currentState != AIState.Chasing)
            {
                currentState = AIState.Chasing;
                agent.isStopped = false; // 움직임 허용
                agent.speed = chaseSpeed; // 달리기 속도 설정
            }
            timeSinceLastSawPlayer = suspiciousTime; // 플레이어를 계속 보면 의심 시간 초기화
        }



        switch (currentState)
        {
            
            case AIState.Idle:
                
                agent.isStopped = true; // 대기 중 정지 유지

                if (waitCounter > 0 )
                {
                    waitCounter -= Time.deltaTime;
                }
                else
                {
                    currentState = AIState.Patrolling;
                    agent.isStopped = false; // 🚨 정지 해제
                    agent.speed = patrolSpeed; // 순찰 속도 설정
                    agent.SetDestination(wayPoints.GetChild(currentWaypoint).position);

                }


                if (distanceToPlayer <= chaseRange)
                {
                    currentState = AIState.Chasing;
                }

                break;



            case AIState.Patrolling:

                agent.isStopped = false; // 순찰 중 움직임 유지

                if (agent.remainingDistance <= 0.2f) //감지범위
                {
                    currentWaypoint++;
                    if (currentWaypoint >= wayPoints.childCount)
                    {
                        currentWaypoint = 0;
                    }
                    currentState = AIState.Idle;
                    waitCounter = waitAtPoint;
                    agent.isStopped = true; // Idle로 전환 시 정지
                }


                if (distanceToPlayer <= chaseRange)
                {
                    currentState = AIState.Chasing;
                }

                break;
            


            case AIState.Chasing:
                
                agent.isStopped = false; // 추격 중 움직임 유지
                agent.speed = chaseSpeed; // 혹시 모를 속도 재설정

                agent.SetDestination(player.transform.position);
                if (distanceToPlayer > chaseRange) 
                {
                    agent.isStopped = true;
                    agent.velocity = Vector3.zero;
                    timeSinceLastSawPlayer -= Time.deltaTime;

                    if (timeSinceLastSawPlayer <= 0)
                    {
                        currentState = AIState.Idle;
                        timeSinceLastSawPlayer = suspiciousTime;
                        agent.isStopped = false;
                    }
                    
                }

                break;
        }




        if(agent.remainingDistance <= 0.2f)
        {
            currentWaypoint++;
            if(currentWaypoint >= wayPoints.childCount)
            {
                currentWaypoint = 0;
            }

            agent.SetDestination(wayPoints.GetChild(currentWaypoint).position);
        }


        
    }

    // EnemyController.cs






// ... (다른 변수 및 함수) ...

/*
// --- 🚨 [수정됨] OnCollisionEnter로 전환 🚨 ---
    // 플레이어의 Is Trigger를 해제했으므로, 물리적 충돌(OnCollisionEnter)을 사용합니다.
    private void OnCollisionEnter(Collision collision)
    {
        // 충돌한 오브젝트가 플레이어인지 확인
        if (collision.gameObject.CompareTag("Player"))
        {
            // PlayerHealthManager 컴포넌트를 가져옴
            PlayerHealthManager healthManager = collision.gameObject.GetComponent<PlayerHealthManager>();

            if (healthManager != null)
            {
                // **🚨 무적 상태 체크:** 플레이어가 무적 상태가 아닐 때만 피격 처리
                // PlayerHealthManager에서 isInvulnerable을 public으로 변경했으므로 바로 접근 가능합니다.
                if (!healthManager.isInvulnerable)
                {
                    healthManager.TakeHit();
                    
                    // 플레이어에게 대미지를 입힌 후, 적 AI를 잠시 멈추고 리셋합니다.
                    StartCoroutine(ResetEnemyAI(1f)); 
                }
            }
        }
    }
*/



// --- 🚨플레이어 피격 및 리스폰 조직 ---

    private void OnTriggerEnter(Collider other)

    {

        if (other.CompareTag("Player"))

        {

            PlayerHealthManager healthManager = other.GetComponent<PlayerHealthManager>();



            if (healthManager != null)

            {

                // **🚨 무적 상태 체크:** 플레이어가 무적 상태가 아닐 때만 피격 처리

                if (!healthManager.isInvulnerable) // isInvulnerable 변수는 public이 아니므로,

                                                  // HealthManager에서 isInvulnerable을 public으로 변경하거나

                                                  // GetIsInvulnerable() 함수를 HealthManager에 추가해야 함

                {

                    healthManager.TakeHit();

                   

                    // 플레이어를 잡은 후 적 AI 리셋

                    StartCoroutine(ResetEnemyAI(1f));

                }

                // 만약 무적 상태라면, 적은 아무것도 하지 않고 플레이어를 통과시킵니다.

            }

        }

    }




     // --- 🚨 적 AI 리셋 코루틴 ---
    IEnumerator ResetEnemyAI(float delay)
    {
        // 1. 적의 움직임을 즉시 멈춥니다.
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        
        // 2. 플레이어 리스폰 및 페이드 시간이 끝날 때까지 대기
        yield return new WaitForSeconds(delay); 

        // 3. 상태를 순찰(Patrolling)로 리셋합니다.
        currentState = AIState.Patrolling;
        agent.isStopped = false;
        agent.speed = patrolSpeed;
        
        // 4. 리셋 후 바로 다음 웨이포인트로 이동하도록 목적지 재설정
        // (현재 currentWaypoint는 순찰 로직에 의해 설정된 상태입니다.)
        if (wayPoints != null && currentWaypoint < wayPoints.childCount)
        {
            agent.SetDestination(wayPoints.GetChild(currentWaypoint).position);
        }
        
        // 5. 플레이어를 놓친 것으로 간주하고 의심 시간 초기화
        timeSinceLastSawPlayer = suspiciousTime; 
    }



    
}
