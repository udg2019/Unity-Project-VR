using UnityEngine;
using TMPro; // TextMeshPro 관련 기능을 사용하기 위해 필요

public class MinigameManager : MonoBehaviour
{
    [Header("Goal Settings")]
    [Tooltip("수집해야 할 책의 총 개수입니다.")]
    public int maxCollectibles = 3; 
    private int collectedCount = 0;

    [Header("UI Reference")]
    [Tooltip("수집 현황을 표시할 TextMeshPro 컴포넌트를 할당하세요.")]
    public TextMeshProUGUI scoreText;

    [Header("Success Settings")]
    [Tooltip("게임 성공 시 띄울 '탈출' UI 패널을 할당하세요. (Inspector에서 비활성화 상태로 시작)")]
    public GameObject successPanel; // 게임 성공 시 띄울 UI 패널 (선택 사항)

    // 🚨 책 리스폰을 위한 변수 추가 🚨
    [Header("Item Respawn Settings")]
    [Tooltip("다시 생성할 책 프리팹의 배열입니다.")]
    public GameObject[] bookPrefabs;

    [Tooltip("책들이 다시 생성될 스폰 지점들의 트랜스폼 배열입니다.")]


    public Transform[] spawnPoints;

    void Start()
    {
        // 1. 초기 UI 설정 (예: 0/3)
        UpdateScoreUI(); 
        
        // 2. 성공 패널 비활성화 (시작 시 숨김)
        if (successPanel != null)
        {
            successPanel.SetActive(false);
        }

        // 3. 게임 시작 시 시간 흐름 정상화 (만약을 대비해)
        Time.timeScale = 1f;

        // 4. 필수 컴포넌트 체크
        if (scoreText == null)
        {
            Debug.LogError("Score Text가 할당되지 않았습니다. Inspector를 확인하세요.");
        }

        // 🚨 게임 시작 시 책들이 파괴된 상태일 수 있으므로, 리스폰 함수를 한 번 호출하여 맵에 채웁니다.
        RespawnAllItems(); 

    }

    // CollectibleItem.cs에서 호출되는 함수
    public void CollectItem()
    {
        // 이미 목표를 달성했으면 중복 실행 방지
        if (collectedCount >= maxCollectibles) return; 

        // 1. 카운트 증가
        collectedCount++;
        
        // 2. UI 업데이트
        UpdateScoreUI();
        
        Debug.Log("책 획득! 현재: " + collectedCount + " / " + maxCollectibles);

        // 3. 성공 조건 체크
        if (collectedCount >= maxCollectibles)
        {
            GameSuccess();
        }
    }

    // UI 텍스트를 업데이트하는 함수
    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = collectedCount + " / " + maxCollectibles;
        }
    }

    // 🚨 게임 오버 시 호출될 책 카운트 초기화 및 오브젝트 리스폰 함수 🚨
    public void ResetCollectedItems()
    {
        collectedCount = 0;
        UpdateScoreUI();
        Debug.Log("Game Over! 책 수집 카운트 초기화 및 오브젝트 리스폰 시작.");
        
        RespawnAllItems(); // 🚨 책 오브젝트를 맵에 다시 생성
    }

    // 🚨 파괴된 모든 책 오브젝트를 다시 생성하는 함수 🚨
    private void RespawnAllItems()
    {
        // 1. 씬에 남아있는 모든 책들을 정리 (중복 생성 방지)
        // 안전을 위해 현재 씬에 남아있는 CollectibleItem 태그를 가진 모든 오브젝트를 파괴합니다.
        CollectibleItem[] existingItems = FindObjectsOfType<CollectibleItem>();
        foreach (CollectibleItem item in existingItems)
        {
            Destroy(item.gameObject);
        }

        // 2. 프리팹과 스폰 지점의 개수가 맞는지 확인
        if (bookPrefabs.Length != spawnPoints.Length || bookPrefabs.Length == 0)
        {
            Debug.LogError("책 프리팹 개수와 스폰 지점 개수가 일치하지 않거나 설정되지 않았습니다! 리스폰 불가.");
            return;
        }

        // 3. 각 스폰 지점에 책을 다시 생성합니다.
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            if (bookPrefabs[i] != null && spawnPoints[i] != null)
            {
                // Instantiate(생성할 오브젝트, 위치, 회전)
                Instantiate(bookPrefabs[i], spawnPoints[i].position, spawnPoints[i].rotation);
            }
        }
        
        Debug.Log("모든 책 오브젝트가 원래 위치에 다시 생성되었습니다.");
    }


    // 게임 성공 시 호출되는 로직
    private void GameSuccess()
    {
        Debug.Log("미니게임 성공! 모든 책을 수집했습니다.");

        // 1. 성공 패널 활성화 ("탈출" 메시지 표시)
        if (successPanel != null)
        {
            successPanel.SetActive(true);
        }
        
        // 2. 🚨 게임 정지 (시간 흐름을 멈춰서 적과 플레이어의 모든 움직임을 멈춥니다.)
        Time.timeScale = 0f;

        // 3. 여기에 성공 사운드 재생 등 추가 로직 구현
    }
}