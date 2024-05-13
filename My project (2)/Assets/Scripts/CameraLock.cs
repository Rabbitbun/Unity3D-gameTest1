using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;
using System;

public class CameraLock : MonoBehaviour
{
    [Header("Objects")]
    [Space]
    [SerializeField] private Camera mainCamera;            // your main camera object.
    [SerializeField] private CinemachineFreeLook cinemachineFreeLook; //cinemachine free lock camera object.
    [SerializeField] private PlayerController playerController;


    [SerializeField] private InputReader _inputReader = default;
    //[Space]
    //[Header("UI")]
    //[SerializeField] private Image aimIcon;  // ui image of aim icon u can leave it null.
    //[Space]
    //[Header("Settings")]
    //[Space]
    [SerializeField] private string enemyTag; // the enemies tag.
    //[SerializeField] private KeyCode _Input;
    [SerializeField] private Vector2 targetLockOffset;
    [SerializeField] private float minDistance; // minimum distance to stop rotation if you get close to target
    [SerializeField] private float maxDistance;

    [SerializeField] private GameObject[] gos;

    public bool isTargeting;

    private float maxAngle;
    private Transform currentTarget;
    private float mouseX;
    private float mouseY;

    public event Action<Transform> OnTargetLocked;

    private void OnEnable()
    {
        _inputReader.lockCameraEvent += AssignTarget;
    }

    private void OnDisable()
    {
        _inputReader.lockCameraEvent -= AssignTarget;
    }

    void Start()
    {
        maxAngle = 90f; // always 90 to target enemies in front of camera.
        cinemachineFreeLook.m_XAxis.m_InputAxisName = "";
        cinemachineFreeLook.m_YAxis.m_InputAxisName = "";
    }

    void LateUpdate()
    {
        if (!isTargeting)
        {
            mouseX = playerController.mouseXY.x;
            mouseY = playerController.mouseXY.y;
        }
        else
        {
            NewInputTarget(currentTarget);
        }

        ////if (aimIcon)
        ////    aimIcon.gameObject.SetActive(isTargeting);

        cinemachineFreeLook.m_XAxis.m_InputAxisValue = mouseX;
        cinemachineFreeLook.m_YAxis.m_InputAxisValue = mouseY;

        //if (playerController.IsLockCamera)
        //{
        //    Vector3 targetPosition = GetTargetPosition(); // 獲取目標位置
        //    Vector3 aimDirection = (targetPosition - transform.position).normalized;

        //    // 如果玩家沒有移動,則直接面向目標
        //    if (playerController.playerMovement.sqrMagnitude < 0.1f && )
        //    {
        //        transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f);
        //    }
        //    else
        //    {
        //        // 如果玩家有移動,則繼續之前的處理
        //        //Vector3 worldAimTarget = mouseWorldPosition;
        //        //worldAimTarget.y = transform.position.y;
        //        //aimDirection = (worldAimTarget - transform.position).normalized;
        //        //transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f);
        //    }

        //    //在這邊調整_castPoint
        //    //_castPoint.LookAt(targetPosition);
        //}
        if (playerController.IsLockCamera)
        {
            Vector3 targetPosition = GetTargetPosition(); // 獲取目標位置
            targetPosition.y = transform.position.y;
            Vector3 aimDirection = (targetPosition - transform.position).normalized;

            // 如果玩家沒有移動,則直接面向目標
            if (playerController.playerMovement.sqrMagnitude < 0.1f)
            {
                transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 15f);
            }
            else
            {
                // 如果玩家有移動,則繼續之前的處理
                //Vector3 worldAimTarget = mouseWorldPosition;
                //worldAimTarget.y = transform.position.y;
                //aimDirection = (worldAimTarget - transform.position).normalized;
                //transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f);
            }

            //在這邊調整_castPoint
            //_castPoint.LookAt(targetPosition);
        }
    }

    public Vector3 GetTargetPosition()
    {
        if (currentTarget != null)
        {
            return currentTarget.position;
        }
        else
        {
            return transform.position; // 如果沒有鎖定目標,則使用玩家自身的位置
        }
    }

    private void AssignTarget()
    {
        if (isTargeting)
        {
            isTargeting = false;
            currentTarget = null;
            return;
        }

        if (ClosestTarget())
        {
            currentTarget = ClosestTarget().transform;
            isTargeting = true;
        }
    }


    private void NewInputTarget(Transform target) // sets new input value.
    {
        if (!currentTarget) return;

        Vector3 viewPos = mainCamera.WorldToViewportPoint(target.position);

        //if (aimIcon)
        //    aimIcon.transform.position = mainCamera.WorldToScreenPoint(target.position);

        if ((target.position - transform.position).magnitude < minDistance) return;
        mouseX = (viewPos.x - 0.5f + targetLockOffset.x) * 3f;              // you can change the [ 3f ] value to make it faster or  slower
        mouseY = (viewPos.y - 0.5f + targetLockOffset.y) * 3f;              // don't use delta time here.
    }


    private GameObject ClosestTarget() // this is modified func from unity Docs ( Gets Closest Object with Tag ). 
    {
        //GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag(enemyTag);
        GameObject closest = null;
        float distance = maxDistance;
        float currAngle = maxAngle;
        Vector3 position = transform.position;
        foreach (GameObject go in gos)
        {
            Vector3 diff = go.transform.position - position;
            float curDistance = diff.magnitude;
            if (curDistance < distance)
            {
                Vector3 viewPos = mainCamera.WorldToViewportPoint(go.transform.position);
                Vector2 newPos = new Vector3(viewPos.x - 0.5f, viewPos.y - 0.5f);
                if (Vector3.Angle(diff.normalized, mainCamera.transform.forward) < maxAngle)
                {
                    closest = go;
                    currAngle = Vector3.Angle(diff.normalized, mainCamera.transform.forward.normalized);
                    distance = curDistance;
                }
            }
        }
        return closest;
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, maxDistance);
    }
}