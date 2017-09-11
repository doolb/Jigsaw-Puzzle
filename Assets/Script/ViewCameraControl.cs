using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// 视囗控制脚本
/// </summary>
public class ViewCameraControl : DragablePlane
{
    /// <summary>
    /// 可视化视口大小的对象
    /// </summary>
    GameObject viewArea;

    /// <summary>
    /// 可视化视口大小的对象 边界大小
    /// </summary>
    public float border = .1f;

    /// <summary>
    /// 最大的摄像头视口范围
    /// </summary>
    float maxViewSize;

    /// <summary>
    /// 可视化视口大小的对象 图像大小
    /// </summary>
    float maxImageSize;

    /// <summary>
    /// 最小缩放等级 
    /// </summary>
    public float minZoom = 0.2f;


    /// <summary>
    /// 覆盖 基类 的 Start
    /// </summary>
    protected override void Start()
    {
        // 只需要 collider
        collider = GetComponent<BoxCollider>();

        viewArea = transform.Find("View Area").gameObject;

        childLayer = viewArea.layer;

        maxViewSize = Camera.main.orthographicSize;
        maxImageSize = viewArea.transform.localScale.x;

        //transform.Find("Zoom Bar").GetComponent<UISlider>().onChange.Add(new EventDelegate(OnZoom));

    }

    /// <summary>
    /// 每帧执行一次， 不受时间控制
    /// </summary>
    void Update()
    {
        // 判断是否按下鼠标
        if (!Input.GetButton("Fire1"))
        {
            // 清除工作标记
            workingId = 0;

            // 清除激活的对象
            activeObject = null;
            return;
        }


        // 判断是否在视口范围内
        Vector3 viewport = cam.ScreenToViewportPoint(Input.mousePosition);

        if (!IsInView(viewport))
        {
            // 清除工作标记
            workingId = 0;

            // 清除激活的对象
            activeObject = null;
            return;
        }

        // 标记当前实例在工作
        workingId = GetInstanceID();

        // 设置激活对象
        activeObject = viewArea;

        // 移动对象
        MoveObject(cam.ViewportToScreenPoint(viewport));
    }

    /// <summary>
    /// 禁用 基类 的 FixedUpdate
    /// </summary>
    protected override void FixedUpdate()
    {

    }

    /// <summary>
    /// 是否在视口中
    /// </summary>
    /// <param name="viewport">视口坐标</param>
    /// <returns></returns>
    bool IsInView(Vector3 viewport)
    {
        // 视口范围 为 （0，1），再加上 判断边界值
        if (viewport.x < border || viewport.x > 1 - border)
            return false;
        if (viewport.y < border || viewport.y > 1 - border)
            return false;

        return true;
    }

    /// <summary>
    /// 缩放主视口
    /// </summary>
    public void OnZoom()
    {
        // 获取缩放等级
        float zoom = UISlider.current.value;

        // 是否小于最小缩放等级
        if (zoom < minZoom) zoom = minZoom;

        // 缩放视口
        Camera.main.orthographicSize = maxViewSize * zoom;

        // 缩放视口可视化对象
        viewArea.transform.localScale = new Vector3(maxImageSize, maxImageSize, 1) * zoom;
    }
}
