using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowWalk : CameraWalk
{
    private Quaternion _initialRotation;
    private Transform _parent;
    private Vector3 initialLocalPosition;

    [Header("设置")]
    public bool isRotate = false;
    protected override void Start()
    {
        base.Start();

        _initialRotation = transform.rotation;
        _parent = transform.parent;
        initialLocalPosition = transform.position - _parent.position;
    }
    protected virtual void LateUpdate()
    {
        if (!isRotate)
        {
            transform.rotation = _initialRotation;
            transform.position = _parent.position + initialLocalPosition;
        }

        
    }
    protected override void myCameraWalk()
    {
        // 已修复编码乱码的注释。
        float horizontal = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        float vertical = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
        Vector3 prePos = transform.position;
        transform.Translate(horizontal, 0, vertical, Space.Self);
        initialLocalPosition += transform.position - prePos;
        //
        if (Input.GetKey(KeyCode.Q))
        {
            prePos = transform.position;
            transform.Translate(0, -moveSpeed * Time.deltaTime, 0, Space.Self);
            initialLocalPosition += transform.position - prePos;
        }
        if (Input.GetKey(KeyCode.E))
        {
            prePos = transform.position;
            transform.Translate(0, moveSpeed * Time.deltaTime, 0, Space.Self);
            initialLocalPosition += transform.position - prePos;
        }
        // 已修复编码乱码的注释。
        float fov = GetComponent<Camera>().fieldOfView;
        fov -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        fov = Mathf.Clamp(fov, minFov, maxFov);
        GetComponent<Camera>().fieldOfView = fov;

        // 已修复编码乱码的注释。
        VisibleMouse();

        // 
        if (!isCursorVisible)
        {
            float mouseX = Input.GetAxis("Mouse X") * rotateSpeed * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * rotateSpeed * Time.deltaTime;

            transform.eulerAngles = new Vector3(transform.eulerAngles.x - mouseY, transform.eulerAngles.y + mouseX, 0);
            _initialRotation = transform.rotation;
        }
    }
}
