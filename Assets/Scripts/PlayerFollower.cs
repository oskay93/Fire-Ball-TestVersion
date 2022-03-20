//This class is on the main camera to follow player.
using UnityEngine;

public class PlayerFollower : MonoBehaviour
{

    [SerializeField] Vector3 offset;
    Vector3 smoothedPos;
    private Transform player;

    public void SetPosition(Transform p)
    {
        player = p;
        Vector3 desiredPos = player.position + offset;
        smoothedPos = Vector3.Lerp(transform.position, desiredPos, 10);
    }

    private void Update()
    {
        SetPosition(PlayerController.Instance.transform);
        if (player == null) return; // If No Player Found, Don't Continue Update Function
        transform.position = smoothedPos;
    }
}