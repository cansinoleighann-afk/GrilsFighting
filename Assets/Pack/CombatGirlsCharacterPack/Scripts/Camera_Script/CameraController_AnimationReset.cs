using UnityEngine;
using UnityEngine.EventSystems;

namespace CombatGirlsCharacterPack
{
    public class CameraController_AnimationReset : MonoBehaviour
    {
        public Transform target; // 已修复编码乱码的注释。
        public float distance = 10f; // 已修复编码乱码的注释。
        public float heightOffset = 2f; // 已修复编码乱码的注释。
        public float sensitivity = 5f; // 已修复编码乱码的注释。
        public float rotationSpeedMultiplier = 0.2f; // 已修复编码乱码的注释。
        public float zoomSpeed = 5f; // 已修复编码乱码的注释。
        public float yAdjustmentSpeed = 0.2f; // 已修复编码乱码的注释。

        private float initialDistance;
        private float initialHeightOffset;
        private Vector3 initialPosition;
        private Quaternion initialRotation;
        private Vector3 initialTargetPosition;
        private Animator targetAnimator; // 已修复编码乱码的注释。

        private float currentX = 0f;
        private float currentY = 0f;
        private Vector3 dragOrigin;
        private bool isDragging = false;
        private bool isMiddleClickDragging = false;
        private float middleClickDragOriginY;

        private void Start()
        {
            // 已修复编码乱码的注释。
            initialDistance = distance;
            initialHeightOffset = heightOffset;
            initialPosition = transform.position;
            initialRotation = transform.rotation;
            initialTargetPosition = target.position;

            // 已修复编码乱码的注释。
            Vector3 angles = transform.eulerAngles;
            currentX = angles.y;
            currentY = angles.x;

            // 已修复编码乱码的注释。
            if (target != null)
            {
                targetAnimator = target.GetComponent<Animator>();
            }
        }

        private void Update()
        {
            // 已修复编码乱码的注释。
            if (EventSystem.current.IsPointerOverGameObject())
            {
                // 已修复编码乱码的注释。
                //isDragging = false;
                //isMiddleClickDragging = false;
                //return;
            }

            // 已修复编码乱码的注释。
            if (Input.GetMouseButtonDown(0))
            {
                isDragging = true;
                dragOrigin = Input.mousePosition;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                isDragging = false;
            }

            if (Input.GetMouseButtonDown(2))
            {
                isMiddleClickDragging = true;
                middleClickDragOriginY = Input.mousePosition.y;
            }
            else if (Input.GetMouseButtonUp(2))
            {
                isMiddleClickDragging = false;
            }

            // 已修复编码乱码的注释。
            if (isDragging)
            {
                Vector3 difference = Input.mousePosition - dragOrigin;
                currentX += difference.x * sensitivity * rotationSpeedMultiplier * Time.deltaTime;
                currentY -= difference.y * sensitivity * rotationSpeedMultiplier * Time.deltaTime;

                currentY = Mathf.Clamp(currentY, -90f, 90f);
            }

            // 已修复编码乱码的注释。
            if (isMiddleClickDragging)
            {
                float yDifference = (Input.mousePosition.y - middleClickDragOriginY) * yAdjustmentSpeed * Time.deltaTime;
                heightOffset -= yDifference; // 已修复编码乱码的注释。
                middleClickDragOriginY = Input.mousePosition.y; // 已修复编码乱码的注释。
            }

            // 已修复编码乱码的注释。
            float zoomInput = Input.GetAxis("Mouse ScrollWheel");
            if (zoomInput != 0f)
            {
                distance -= zoomInput * zoomSpeed;
                distance = Mathf.Clamp(distance, 1f, 100f);
            }

            // 已修复编码乱码的注释。
            if (Input.GetMouseButtonDown(1))
            {
                ResetCamera();
            }

            // 已修复编码乱码的注释。
            Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
            Vector3 offset = new Vector3(0f, heightOffset, 0f);
            transform.position = target.position + offset - rotation * Vector3.forward * distance;
            transform.LookAt(target.position + offset);
        }

        private void ResetCamera()
        {
            distance = initialDistance;
            heightOffset = initialHeightOffset;
            transform.position = initialPosition;
            transform.rotation = initialRotation;
            currentX = initialRotation.eulerAngles.y;
            currentY = initialRotation.eulerAngles.x;
            target.position = initialTargetPosition;

            // 已修复编码乱码的注释。
            if (targetAnimator != null)
            {
                targetAnimator.Play(targetAnimator.GetCurrentAnimatorStateInfo(0).fullPathHash, -1, 0f);
            }
        }
    }
}
