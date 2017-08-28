using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class DragablePlane : MonoBehaviour
{
    #region public 
    public LayerMask childLayerMask;
    public bool dragEnd;
    #endregion

    #region class only
    protected BoxCollider collider;

    protected GameObject    activeObject;
    protected Vector3       activePoint;

    protected int childLayer;

    protected RaycastHit[] raycastHitCache;
    protected virtual int raycastHitCacheSize 
    {
        get {return 100;}
    }


    #endregion


    #region unity callback


    protected virtual void Start()
    {
        collider = GetComponent<BoxCollider>();
        childLayer = childLayerMask.MaskToLayer();
        raycastHitCache = new RaycastHit[raycastHitCacheSize];
    }


    // Update is called once per frame
	protected virtual void Update()
    {
        Vector3 pos = Input.mousePosition;
        float maxDis =Vector3.Distance (
            Camera.main.transform.position ,
            transform.position) * 2;

        // 是否按下按钮
        if(Input.GetButton("Fire1"))
        {
            dragEnd = false;
            Ray ray = Camera.main.ScreenPointToRay(pos);

            // 没有活动的对象，从所有的对象中选择一个
            if (activeObject == null)
            {
                RaycastHit hit;
                if(FindAClosetChild(out hit,ray,maxDis))
                {
                    // 选中当前的
                    activeObject = hit.transform.gameObject;
                    activePoint = hit.point;

                    ActiveObject(activeObject);
                    //print("Active Object : " + activeObject + "at " + activePoint);
                }
            }
                // 已经选中了个对象，直接中在平面中获取坐标
            else
            {
                RaycastHit hit;
                if (collider.Raycast(ray, out hit, maxDis))
                {
                    Vector3 d = hit.point - activePoint;
                    activePoint = hit.point;
                    
                    activeObject.transform.localPosition += d;
                    MoveObject(activeObject, d);
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
                DeactiveObject(activeObject);
                activeObject = null;
                dragEnd = true;
            }
        }

    }
    #endregion


    #region virtual function
    protected virtual void ActiveObject(GameObject go)
    {

    }

    protected virtual void DeactiveObject(GameObject go)
    {

    }

    protected virtual void MoveObject(GameObject go,Vector3 delta)
    {

    }

    protected virtual int RaycastHitOrder(GameObject go)
    {
        return 1;
    }

    #endregion

    #region function

    bool FindAClosetChild(out RaycastHit hit,Ray ray,float maxDistance)
    {
        hit = default(RaycastHit);
        int n = Physics.RaycastNonAlloc(ray, raycastHitCache, maxDistance, 1 << childLayer);
        if (n == 0) return false;

        int max = 0, maxOrder = 0;
        for(int i=0;i<n;i++)
        {
            int newOrder = RaycastHitOrder(raycastHitCache[i].transform.gameObject);
            if (newOrder > maxOrder)
            {
                maxOrder = newOrder;
                max = i;
            }
        }
        hit = raycastHitCache[max];
        return true;
    }


    #endregion
}
