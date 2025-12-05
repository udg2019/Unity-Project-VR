using UnityEngine;

using UnityEngine.UI;

using System.Collections; // Coroutine을 사용하기 위해 필요



public class PlayerHealthManager : MonoBehaviour

{

    // --- 설정 변수 ---

    [Header("Health Settings")]

    public int maxHealth = 3;

    private int currentHealth;



    // --- UI 및 리스폰 변수 ---

    [Header("UI and Respawn Settings")]

    public GameObject[] heartImages; // Border (꺼진 하트) UI

    public Transform respawnPoint;   // 리스폰 지점

    public GameObject gameOverPanel; // 게임 오버 UI

    public GameObject fadePanel;     // 페이드 인/아웃에 사용할 검은색 패널

    [Range(0.5f, 2f)] public float fadeDuration = 1.0f; // 페이드 효과 지속 시간 (1초 설정)



    // --- 무적 변수 ---

    [Header("Invulnerability")]

    [SerializeField] private float invulnerabilityDuration = 2.0f; // 무적 시간 (2초)

    public bool isInvulnerable = false; // 🚨 public으로 변경하여 EnemyController에서 접근 가능하게 함



    // --- 초기화 ---

    void Start()

    {

        InitializeGame();

    }



    // 게임 초기 상태 설정 (시작 시 또는 게임 오버 후 리셋 시)

    void InitializeGame()

    {

        currentHealth = maxHealth;

        UpdateHealthUI(); // UI 업데이트



        // 게임 오버 화면 및 페이드 패널 비활성화

        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        if (fadePanel != null) fadePanel.SetActive(false);



        // 무적 상태 초기화

        isInvulnerable = false;



        // 플레이어 리스폰 (게임 시작 위치로)

        RespawnPlayerImmediate();

    }



    // --- 핵심 로직: 피격 처리 ---

    public void TakeHit()

    {

        // 🚨 무적 상태이거나 이미 게임 오버 상태인 경우 피격 무시

        if (currentHealth <= 0 || isInvulnerable) return;



        // 1. 체력 감소 및 UI 업데이트

        currentHealth--;

        Debug.Log("Player Hit! Current Health: " + currentHealth);

        UpdateHealthUI();



        // 2. 게임 오버 체크 및 처리

        if (currentHealth <= 0)

        {

            StartCoroutine(GameOverSequence()); // 게임 오버 시퀀스 시작

        }

        else

        {

            // 3. 리스폰 시퀀스 시작 (페이드 및 무적 포함)

            StartCoroutine(RespawnSequence());

        }

    }



    // --- 체력 UI 업데이트 ---

    void UpdateHealthUI()

    {

        for (int i = 0; i < maxHealth; i++)

        {

            if (i < heartImages.Length)

            {

                // i가 현재 체력과 같거나 클 때 Border(꺼진 하트)를 활성화

                heartImages[i].SetActive(i >= currentHealth);

            }

        }

    }



    // --- (즉시) 리스폰 로직 ---

    // 페이드 효과 도중 위치를 즉시 이동시킬 때 사용

    void RespawnPlayerImmediate()

    {

        if (respawnPoint != null)

        {

            transform.position = respawnPoint.position;

            transform.rotation = respawnPoint.rotation;

        }

        else

        {

            Debug.LogError("Respawn Point is not set!");

        }

    }



    // --- 🏃 무적 & 페이드 리스폰 시퀀스 (가장 중요) ---

    IEnumerator RespawnSequence()

    {

        // 1. ⚔️ 무적 시작

        isInvulnerable = true;



        // 2. 🌑 페이드 아웃 시작

        yield return StartCoroutine(FadeScreen(true));



        // 3. 🔄 플레이어 리스폰 (어두운 상태에서)

        RespawnPlayerImmediate();



        // 4. ✨ 페이드 인 (화면 복귀)

        yield return StartCoroutine(FadeScreen(false));



        // 5. 🛡️ 무적 시간 대기 (페이드 인 완료 후부터)

        // 무적 상태임을 플레이어에게 시각적으로 알리기 위해 깜빡임 효과 등을 추가할 수 있습니다.

        yield return new WaitForSeconds(invulnerabilityDuration);



        // 6. ⚔️ 무적 해제

        isInvulnerable = false;

        Debug.Log("Invulnerability Ended.");

    }



    // --- 💀 게임 오버 시퀀스 ---

    IEnumerator GameOverSequence()

    {

        // 1. 🌑 페이드 아웃 시작 (게임 오버 시에도 리스폰 전에 화면 가리기)

        yield return StartCoroutine(FadeScreen(true));



        // 2. 🔄 플레이어 리스폰 (게임 오버 후 리셋될 때 시작 위치로 이동)

        RespawnPlayerImmediate();



        // 🚨 3. MinigameManager 찾아서 책 카운트와 오브젝트 초기화 (핵심 로직) 🚨

        MinigameManager minigameManager = FindObjectOfType<MinigameManager>();



        if (minigameManager != null)

        {

            minigameManager.ResetCollectedItems(); // 💥 이 함수가 호출되어야 책이 리셋됩니다! 💥

        }
        else
        {

            Debug.LogError("PlayerHealthManager: MinigameManager를 찾지 못했습니다. Hierarchy에 활성화되어 있는지 확인하세요!");

        }





        // 3. 💀 게임 오버 화면 활성화

        if (gameOverPanel != null)

        {

            // 페이드 패널을 비활성화하고, 게임 오버 패널을 활성화

            if (fadePanel != null) fadePanel.SetActive(false);

            gameOverPanel.SetActive(true);

        }



        // 4. ⏱️ 3초 대기 (게임 오버 화면을 충분히 보여줌)

        yield return new WaitForSeconds(3f);



        // 5. 🔄 게임 상태 리셋 (체력 리셋 포함)

        InitializeGame();



        // 6. ✨ 페이드 인 (리셋 후 다시 플레이 가능)

        yield return StartCoroutine(FadeScreen(false));

    }



    // --- ✨ 페이드 인/아웃 코루틴 ---

    // fadeOut: true면 어두워지고(0 -> 1), false면 밝아짐(1 -> 0)

    IEnumerator FadeScreen(bool fadeOut)

    {

        if (fadePanel == null) yield break;



        fadePanel.SetActive(true);

        CanvasGroup canvasGroup = fadePanel.GetComponent<CanvasGroup>();

        if (canvasGroup == null)

        {

            // CanvasGroup이 없다면 추가

            canvasGroup = fadePanel.AddComponent<CanvasGroup>();

        }



        float startAlpha = fadeOut ? 0f : 1f;

        float endAlpha = fadeOut ? 1f : 0f;

        float time = 0f;



        canvasGroup.alpha = startAlpha;



        while (time < fadeDuration)

        {

            time += Time.deltaTime;

            float normalizedTime = time / fadeDuration;

            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, normalizedTime);

            yield return null;

        }



        canvasGroup.alpha = endAlpha;



        // 밝아지는 페이드 인이 완료되었을 때만 패널을 비활성화하여 렌더링을 멈춥니다.

        if (!fadeOut)

        {

            fadePanel.SetActive(false);

        }

    }

}