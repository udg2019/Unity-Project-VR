using UnityEngine;

// Controls player movement and rotation (CharacterController version)
public class PlayerController : MonoBehaviour
{
    public float speed = 5.0f;           // Player movement speed
    public float rotationSpeed = 120.0f; // Player rotation speed

    private CharacterController controller;

    private void Start()
    {
        controller = GetComponent<CharacterController>();

        if (controller == null)
        {
            Debug.LogError("CharacterController 컴포넌트가 Player 오브젝트에 없습니다!");
        }
    }

    private void Update()
    {
        // WASD 이동 입력
        float moveVertical = Input.GetAxis("Vertical");
        float moveHorizontal = Input.GetAxis("Horizontal");

        // 바라보는 방향 기준 이동
        Vector3 direction = (transform.forward * moveVertical) + (transform.right * moveHorizontal);

        // 충돌 적용된 CharacterController 이동
        controller.SimpleMove(direction * speed);

        // 좌우 회전
        float turn = Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime;
        transform.Rotate(0f, turn, 0f);
    }
}
