using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 提供了一个可拖拽子对象的容器
/// </summary>
[RequireComponent(typeof(BoxCollider))]
public class DragablePlane : MonoBehaviour
{
    /// <summary>
    /// 正在工作的实例
    /// </summary>
    protected static int workingId;

    #region 变量

    /// <summary>
    /// 表示拖拽是否结束
    /// </summary>
    public bool dragEnd;

    /// <summary>
    /// 进行碰撞测试的摄像机
    /// </summary>
    public Camera cam;

    /// <summary>
    /// collider, 决定可以拖拽的范围
    /// </summary>
    protected BoxCollider collider;

    /// <summary>
    /// 激活的对象
    /// </summary>
    protected GameObject activeObject;

    /// <summary>
    /// 保存上次拖拽的位置
    /// </summary>
    protected Vector3 activePoint;

    /// <summary>
    /// 可拖拽子对象的 层
    /// </summary>
    public int childLayer = 31;

    /// <summary>
    /// 碰撞的缓存
    /// </summary>
    protected RaycastHit[] raycastHitCache;

    /// <summary>
    /// 碰撞的缓存的大小
    /// </summary>
    protected virtual int raycastHitCacheSize
    {
        get { return 100; }
    }


    #endregion


    #region unity 函数

    /// <summary>
    /// 初始化
    /// </summary>
    protected virtual void Awake()
    {
        // 获取 collider 对象
        collider = GetComponent<BoxCollider>();

        // 初始化缓存
        raycastHitCache = new RaycastHit[raycastHitCacheSize];

        // 如果没有设置摄化机， 就使用默认的
        if (cam == null) cam = Camera.main;
    }


    /// <summary>
    /// 固定时间执行，可以用 Time.timeScale 来控制
    /// </summary>
    protected virtual void FixedUpdate()
    {
        // 移动对象
        MoveObject(Input.mousePosition);
    }
    #endregion


    #region 虚函数

    /// <summary>
    /// 当有一个对象被激活时调用
    /// </summary>
    /// <param name="go">当前激活的对象</param>
    protected virtual void ActiveObject(GameObject go)
    {

    }

    /// <summary>
    /// 当有一个对象被取消激活时调用
    /// </summary>
    /// <param name="go">被取消激活的对象</param>
    protected virtual void DeactiveObject(GameObject go)
    {

    }

    /// <summary>
    /// 当有一个对象被移动时调用
    /// </summary>
    /// <param name="go">被移动的对象</param>
    /// <param name="delta">移动的偏移</param>
    protected virtual void MoveObject(GameObject go, Vector3 delta)
    {
        // 移动当前激活的对象
        activeObject.transform.position += delta;
    }

    /// <summary>
    /// 获取 对象的 碰撞优先值
    /// </summary>
    /// <param name="go">需要获取 碰撞优先值 的对象</param>
    /// <returns>对象的 碰撞优先值</returns>
    protected virtual int RaycastHitOrder(GameObject go)
    {
        return 1;
    }

    #endregion

    #region 函数

    /// <summary>
    /// 移动对象
    /// </summary>
    /// <param name="pos">屏幕坐标</param>
    protected void MoveObject(Vector3 pos)
    {
        // 判断是否有实例在工作，并且是否是当前实例
        if (workingId != 0 && workingId != GetInstanceID()) return;

        // 碰撞尝试的最大距离
        float maxDis = Vector3.Distance(
            cam.transform.localPosition,
            transform.localPosition) * 2;


        // 是否按下按钮
        if (Input.GetButton("Fire1"))
        {

            // 拖拽开始
            dragEnd = false;

            // 当前位置发出的光线
            Ray ray = cam.ScreenPointToRay(pos);

            // 没有活动的对象，从所有的对象中选择一个
            if (activeObject == null)
            {

                // 寻找一个最近的
                RaycastHit hit;
                if (FindClosetChild(out hit, ray, maxDis))
                {
                    // 选中当前的对象
                    activeObject = hit.collider.gameObject;

                    // 记录当前的坐标
                    activePoint = hit.point;

                    // 调用虚函数
                    ActiveObject(activeObject);

                    // 标记当前的实例
                    workingId = GetInstanceID();
                }
            }
            // 已经选中了个对象，直接中在平面中获取坐标
            else
            {
                RaycastHit hit;
                if (collider.Raycast(ray, out hit, maxDis))
                {
                    // 和上次保存的点对比，得到偏移
                    Vector3 d = hit.point - activePoint;
                    activePoint = hit.point;

                    // 调用虚函数
                    MoveObject(activeObject, d);
                }
                else
                    activeObject = null; // 已经移出了平面

            }

        }
        // 释放按钮
        else
        {
            // 是否有激活的对象
            if (activeObject != null)
            {
                // 调用虚函数
                DeactiveObject(activeObject);

                // 清除激活标志
                activeObject = null;

                // 拖拽结束
                dragEnd = true;

                // 清除工作实例标记
                workingId = 0;
            }
        }


    }


    /// <summary>
    /// 寻找一个最近的对象
    /// </summary>
    /// <param name="hit">记录碰撞的结果</param>
    /// <param name="ray">需要测试的光线</param>
    /// <param name="maxDistance">测试的最大距离</param>
    /// <returns>是否找到对象</returns>
    bool FindClosetChild(out RaycastHit hit, Ray ray, float maxDistance)
    {
        // 设置默认值
        hit = default(RaycastHit);

        // 测试所有处于相应层中的对象
        int n = Physics.RaycastNonAlloc(ray, raycastHitCache, maxDistance, 1 << childLayer);

        // 如果没找到，返回 false
        if (n == 0) return false;


        // 记录最大对象，和最大的 优先值
        int max = -1, maxOrder = -1;

        // 循环遍历所有的结果
        for (int i = 0; i < n; i++)
        {
            // 获取当前对象的顺先值
            int newOrder = RaycastHitOrder(raycastHitCache[i].collider.gameObject);

            // 是否是最大的
            if (newOrder > maxOrder)
            {
                // 记录 当前的对象，和 优先值
                maxOrder = newOrder;
                max = i;
            }
        }

        if (max == -1) return false;


        // 找到一个最近的
        hit = raycastHitCache[max];
        return true;
    }


    #endregion
}
