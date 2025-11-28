using UnityEngine;

public class DoorBuilder : MonoBehaviour
{
    [Header("문 크기 설정")]
    public float doorWidth = 1.0f; // 문 한 짝 너비
    public float doorHeight = 2.4f; // 문 높이
    public float frameThick = 0.1f; // 프레임 두께

    [Header("재질 (여기에 드래그하세요!)")]
    public Material frameMat;  // 검은색 금속
    public Material glassMat;  // 유리
    public Material handleMat; // 손잡이 (은색)

    private GameObject parentObj;

    [ContextMenu("1. 도서관 문 만들기 (클릭)")]
    public void BuildDoors()
    {
        // 청소
        if (parentObj != null) DestroyImmediate(parentObj);
        parentObj = new GameObject("Hallym_Library_Door_Set");

        // 왼쪽 문 만들기
        CreateDoor(new Vector3(-doorWidth / 2, 0, 0), false);

        // 오른쪽 문 만들기
        CreateDoor(new Vector3(doorWidth / 2, 0, 0), true);

        Debug.Log("문 설치 완료!");
    }

    void CreateDoor(Vector3 pos, bool isRight)
    {
        GameObject door = new GameObject(isRight ? "Door_R" : "Door_L");
        door.transform.parent = parentObj.transform;
        door.transform.localPosition = pos;

        // 1. 프레임 (검은색 테두리 4개)
        float fW = frameThick;
        // 좌
        CreatePart("Frame_L", new Vector3(-doorWidth / 2 + fW / 2, doorHeight / 2, 0), new Vector3(fW, doorHeight, fW * 2), frameMat, door.transform);
        // 우
        CreatePart("Frame_R", new Vector3(doorWidth / 2 - fW / 2, doorHeight / 2, 0), new Vector3(fW, doorHeight, fW * 2), frameMat, door.transform);
        // 상
        CreatePart("Frame_T", new Vector3(0, doorHeight - fW / 2, 0), new Vector3(doorWidth, fW, fW * 2), frameMat, door.transform);
        // 하
        CreatePart("Frame_B", new Vector3(0, fW / 2, 0), new Vector3(doorWidth, fW, fW * 2), frameMat, door.transform);

        // 2. 유리창
        CreatePart("Glass", new Vector3(0, doorHeight / 2, 0), new Vector3(doorWidth - fW * 2, doorHeight - fW * 2, 0.02f), glassMat, door.transform);

        // 3. 손잡이 (문 안쪽, 바깥쪽 양면)
        float handleX = isRight ? -doorWidth / 2 + 0.2f : doorWidth / 2 - 0.2f; // 손잡이 위치 (문의 안쪽 끝)
        // 바깥 손잡이
        CreatePart("Handle_Out", new Vector3(handleX, 1.0f, 0.15f), new Vector3(0.05f, 0.4f, 0.05f), handleMat, door.transform);
        // 안쪽 손잡이
        CreatePart("Handle_In", new Vector3(handleX, 1.0f, -0.15f), new Vector3(0.05f, 0.4f, 0.05f), handleMat, door.transform);
    }

    void CreatePart(string name, Vector3 pos, Vector3 scale, Material mat, Transform parent)
    {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.name = name;
        obj.transform.parent = parent;
        obj.transform.localPosition = pos;
        obj.transform.localScale = scale;
        if (mat != null) obj.GetComponent<Renderer>().material = mat;
    }
}