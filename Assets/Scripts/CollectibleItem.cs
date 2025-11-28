using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    // 충돌 감지 함수 (Collider의 Is Trigger가 켜져 있어야 작동)
    private void OnTriggerEnter(Collider other)
    {
        // 1. 충돌한 오브젝트가 'Player' 태그를 가지고 있는지 확인합니다.
        // 플레이어 오브젝트에 "Player" 태그를 반드시 설정해야 합니다!
        if (other.CompareTag("Player"))
        {
            // 2. 씬에 있는 MinigameManager를 찾습니다.
            // Game Manager는 보통 하나만 존재하므로, FindObjectOfType을 사용해도 무방합니다.
            MinigameManager manager = FindObjectOfType<MinigameManager>();

            if (manager != null)
            {
                // 3. Manager의 수집 함수를 호출하여 카운트를 증가시킵니다.
                manager.CollectItem();

                // 4. 수집 완료 후 책 오브젝트를 파괴합니다.
                Destroy(gameObject); 
            }
            else
            {
                Debug.LogError("MinigameManager 스크립트를 찾을 수 없습니다. 'MinigameManager'라는 빈 오브젝트에 스크립트를 할당했는지 확인하세요.");
            }
        }
    }
}