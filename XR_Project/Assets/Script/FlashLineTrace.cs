using UnityEngine;

public class FlashLineTrace : MonoBehaviour
{
    public float lineLength = 7f; // 레이저 길이
    private RaycastHit hit;

    public bool isTriggered = false;

    private ChasePlayer currentTarget = null;

    // Update is called once per frame
    void Update()
    {
        if (!isTriggered)
        {
            return; // isTriggered가 false이면 레이저를 쏘지 않음
        }

        // 레이저를 쏘고 충돌 여부를 확인
        if (Physics.Raycast(transform.position, transform.forward, out hit, lineLength))
        {
            // 충돌한 오브젝트가 있다면 해당 오브젝트의 이름을 출력
            Debug.Log($"레이저 충돌: {hit.collider.gameObject.name}");

            if (hit.collider.gameObject.CompareTag("Entity"))
            {
                Debug.Log("엔티티와 충돌");
                // 충돌한 엔티티의 ChasePlayer 스크립트를 가져와서 StopAI() 호출
                currentTarget = hit.collider.GetComponent<ChasePlayer>();

                if (currentTarget != null)
                {
                    currentTarget.isLocked = true;
                }
            }
            else
            {
                // 충돌한 오브젝트가 엔티티가 아니라면 이전에 잠금 상태였던 엔티티를 해제
                if (currentTarget != null)
                {
                    currentTarget.isLocked = false;
                    currentTarget = null;
                }
            }
        }
    }

    public void SetTriggered(bool triggered)
    {
        isTriggered = triggered;

        // isTriggered가 false로 설정되면 현재 잠금 상태인 엔티티를 해제
        if (!isTriggered && currentTarget != null)
        {
            currentTarget.isLocked = false;
            currentTarget = null;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * lineLength);
    }
}
