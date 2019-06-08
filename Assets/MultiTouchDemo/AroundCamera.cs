/*************************************************************************
 *  Copyright © 2017-2018 Mogoson. All rights reserved.
 *------------------------------------------------------------------------
 *  File         :  AroundCamera.cs
 *  Description  :  Camera rotate around target gameobject.
 *------------------------------------------------------------------------
 *  Author       :  Mogoson
 *  Version      :  0.1.0
 *  Date         :  4/8/2018
 *  Description  :  Initial development version.
 *************************************************************************/

using UnityEngine;

namespace Mogoson.UCamera
{
    /// <summary>
    /// Camera rotate around target gameobject.
    /// </summary>
    [AddComponentMenu("Mogoson/UCamera/AroundCamera")]
    [RequireComponent(typeof(Camera))]
    public class AroundCamera : MonoBehaviour
    {
        #region Field and Property
        /// <summary>
        /// Around center.
        /// </summary>
        public Transform target;

        /// <summary>
        /// Settings of mouse button, pointer and scrollwheel.
        /// </summary>
        public MouseSettings mouseSettings = new MouseSettings(1, 10, 10);

        /// <summary>
        /// Range limit of angle.
        /// </summary>
        public Range angleRange = new Range(-90, 90);

        /// <summary>
        /// Range limit of distance.
        /// </summary>
        public Range distanceRange = new Range(1, 10);

        /// <summary>
        /// Damper for move and rotate.
        /// </summary>
        [Range(0, 10)]
        public float damper = 5;

        /// <summary>
        /// Camera current angls.
        /// </summary>
        public Vector2 CurrentAngles { protected set; get; }

        /// <summary>
        /// Current distance from camera to target.
        /// </summary>
        public float CurrentDistance { protected set; get; }

        /// <summary>
        /// Camera target angls.
        /// </summary>
        protected Vector2 targetAngles;

        /// <summary>
        /// Target distance from camera to target.
        /// </summary>
        protected float targetDistance;
        private Touch oldTouch1;  //上次触摸点1(手指1)  
        private Touch oldTouch2;
        public float PinchSensitivity { private get; set; }
        public float SlideSensitivity { private get; set; }
        #endregion

        #region Protected Method

        protected virtual void Awake()
        {
            if (Input.touchSupported)
            {
                Input.multiTouchEnabled = true;
                Debug.Log("MultiTouchEnabled!");
            }
        }

        protected virtual void Start()
        {
            Initialize();
        }

        protected virtual void LateUpdate()
        {
            AroundByMouse();
        }

        /// <summary>
        /// Initialize component.
        /// </summary>
        protected virtual void Initialize()
        {
            CurrentAngles = targetAngles = transform.eulerAngles;
            CurrentDistance = targetDistance = Vector3.Distance(transform.position, target.position);
        }

        /// <summary>
        /// Camera around target by mouse.
        /// </summary>
        protected void AroundByMouse()
        {
            if (Input.GetMouseButton(mouseSettings.mouseButtonID))
            {
                //Mouse pointer.
                targetAngles.y += Input.GetAxis("Mouse X") * mouseSettings.pointerSensitivity;
                targetAngles.x -= Input.GetAxis("Mouse Y") * mouseSettings.pointerSensitivity;

                //Range.
                targetAngles.x = Mathf.Clamp(targetAngles.x, angleRange.min, angleRange.max);
            }
            if (Input.multiTouchEnabled&&Input.touchCount==1&&Input.GetTouch(0).phase==TouchPhase.Moved)
            {
                //Mouse pointer.
                targetAngles.y += Input.GetTouch(0).deltaPosition.x * SlideSensitivity;
                targetAngles.x -= Input.GetTouch(0).deltaPosition.y* SlideSensitivity;

                //Range.
                targetAngles.x = Mathf.Clamp(targetAngles.x, angleRange.min, angleRange.max);
            }

            //Mouse scrollwheel.
            if (Input.mouseScrollDelta != Vector2.zero)
            {
                targetDistance -= Input.GetAxis("Mouse ScrollWheel") * mouseSettings.wheelSensitivity;
                targetDistance = Mathf.Clamp(targetDistance, distanceRange.min, distanceRange.max);
            }else if (Input.multiTouchEnabled && Input.touchCount >=2) //如果按下了双指
            {
                //多点触摸, 放大缩小  
                Touch newTouch1 = Input.GetTouch(0);
                Touch newTouch2 = Input.GetTouch(1);

                //第2点刚开始接触屏幕, 只记录，不做处理  
                if (newTouch2.phase == TouchPhase.Began)
                {
                    oldTouch2 = newTouch2;
                    oldTouch1 = newTouch1;
                    return;
                }

                //计算老的两点距离和新的两点间距离，变大要放大模型，变小要缩放模型  
                float oldDistance = Vector2.Distance(oldTouch1.position, oldTouch2.position);
                float newDistance = Vector2.Distance(newTouch1.position, newTouch2.position);

                //两个距离之差，为正表示放大手势， 为负表示缩小手势  
                float offset = newDistance - oldDistance;
                Debug.Log(offset);
                targetDistance -= offset * PinchSensitivity;
                targetDistance = Mathf.Clamp(targetDistance, distanceRange.min, distanceRange.max);
                //记住最新的触摸点，下次使用  
                oldTouch1 = newTouch1;
                oldTouch2 = newTouch2;
            }

            //Lerp.
            CurrentAngles = Vector2.Lerp(CurrentAngles, targetAngles, damper * Time.deltaTime);
            CurrentDistance = Mathf.Lerp(CurrentDistance, targetDistance, damper * Time.deltaTime);

            //Update transform position and rotation.
            transform.rotation = Quaternion.Euler(CurrentAngles);
            transform.position = target.position - transform.forward * CurrentDistance;
        }
        #endregion
    }

}