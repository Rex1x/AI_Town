using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
    public float smoothSpeed = 0.001f; // 이동 속도 조정
    public float zoomSpeed = 0.05f; // 줌인/줌아웃 속도
    public float minZoom = 2f; // 최소 줌 (가장 많이 줌인)
    public float maxZoom = 10f; // 최대 줌 (가장 많이 줌아웃)


    private Vector3 targetPosition;
    private bool isMoving = false;

    private void Update()
    {
        // 마우스 클릭으로 카메라 이동
        // UI 클릭을 무시하도록 보완
        if (Input.GetMouseButtonDown(0))
        {
            if (!IsPointerOverUIElement())
            {
                Vector3 clickedPosition = GetWorldPositionFromScreen(Input.mousePosition);
                SetTargetPosition(clickedPosition);
            }
        }
        // 카메라 부드럽게 이동
        if (isMoving)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);

            // 목표 위치에 도달하면 이동 멈춤
            if (Vector3.Distance(transform.position, targetPosition) < 0.05f)
            {
                isMoving = false;
            }
        }

        // 키보드 입력으로 줌인/줌아웃
        if (Input.GetKey(KeyCode.W))
        {
            ZoomCamera(1); // W 키로 줌인
        }
        else if (Input.GetKey(KeyCode.S))
        {
            ZoomCamera(-1); // S 키로 줌아웃
        }

    }


    // 화면의 클릭 위치를 월드 공간의 위치로 변환하는 메서드
    private Vector3 GetWorldPositionFromScreen(Vector3 screenPosition)
    {
        // 카메라 앞에 가상의 평면을 생성하고 그 평면에 클릭한 위치를 투영
        Plane plane = new Plane(Vector3.forward, Vector3.zero); // Z=0인 평면 생성
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        float distance;

        if (plane.Raycast(ray, out distance))
        {
            return ray.GetPoint(distance);
        }

        return Vector3.zero;
    }

    // 목표 위치 설정 메서드
    public void SetTargetPosition(Vector3 position)
    {
        targetPosition = new Vector3(position.x, position.y, transform.position.z); // Z 고정
        isMoving = true;
    }

    // Resident를 클릭했을 때 카메라 이동
    public void CenterOnResident(Transform residentTransform)
    {
        SetTargetPosition(residentTransform.position);
    }

    // 카메라 줌인/줌아웃 메서드
    private void ZoomCamera(float scrollInput)
    {
        float newSize = Camera.main.orthographicSize - scrollInput * zoomSpeed;
        Camera.main.orthographicSize = Mathf.Clamp(newSize, minZoom, maxZoom);
    }

    // 새로운 메서드: UI 클릭 확인
    private bool IsPointerOverUIElement()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }
}
