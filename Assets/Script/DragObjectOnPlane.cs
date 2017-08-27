using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragObjectOnPlane : MonoBehaviour
{

    #region public 
    public Collider  planeCollider;
    #endregion

    #region static
    protected static GameObject   activeObject;
    protected static Vector3 activePoint;
    #endregion

    #region unity callback
    // Update is called once per frame
	void Update () {

        Vector3 pos = Input.mousePosition;
        float maxDis =Vector3.Distance (Camera.main.transform.position ,
            planeCollider.transform.position) * 2;


        // 是否按下按钮
        if(Input.GetButton("Fire1"))
        {
            Ray ray = Camera.main.ScreenPointToRay(pos);

            // 没有活动的对象，从所有的对象中选择一个
            if (activeObject == null)
            {
                RaycastHit hit;
                Physics.Raycast(ray, out hit, maxDis, 1 << gameObject.layer);
                if (hit.transform != null)
                {
                    // 选中当前的
                    activeObject = hit.transform.gameObject;
                    activePoint = hit.point;

                    ActiveObject();
                    //print("Active Object : " + activeObject + "at " + activePoint);
                }
            }
                // 已经选中了个对象，直接中在平面中获取坐标
            else
            {
                RaycastHit hit;
                if (planeCollider.Raycast(ray, out hit, maxDis))
                {
                    Vector3 d = hit.point - activePoint;
                    activePoint = hit.point;
                    
                    activeObject.transform.localPosition += d;
                    MoveObject(d);
                }
                else
                    activeObject = null; // 已经移出了平面

            }
        }
            // 释放按钮
        else
        {
            if (activeObject != null)
            {
                //print("Deactive Object : " + activeObject);
                DeactiveObject();
                activeObject = null;
            }
        }

    }
    #endregion


    #region virtual function
    protected virtual void ActiveObject()
    {

    }

    protected virtual void DeactiveObject()
    {

    }

    protected virtual void MoveObject(Vector3 delta)
    {

    }

    #endregion
}
