using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// deal with aim point
/// </summary>
public class CaculateAiming : MonoBehaviour
{
    private PlayerController playerController;

    //public Transform dubugTransform;
    public LayerMask layerMask = new LayerMask();
    public Vector3 mouseWorldPosition;
    public Vector3 aimDirection;

    public Transform _castPoint;

    public Transform LeftHandPosition;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
    }

    void Update()
    {
        mouseWorldPosition = Vector3.zero;

        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        if(Physics.Raycast(ray, out RaycastHit raycastHit, 999f, layerMask))
        {
            //debugOBJ.position = raycastHit.point;
            mouseWorldPosition = raycastHit.point;
        }

        if(playerController.armState == PlayerController.ArmState.Aim)
        {
            Vector3 worldAimTarget = mouseWorldPosition;
            worldAimTarget.y = transform.position.y;
            aimDirection = (worldAimTarget - transform.position).normalized;
            transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f);
            //在這邊調整_castPoint
            _castPoint.LookAt(mouseWorldPosition);
        }

    }
    
    public Transform GetLeftHandPosition()
    {
        return LeftHandPosition;
    }

}
