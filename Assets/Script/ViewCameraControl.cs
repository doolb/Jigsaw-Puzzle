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
    /// 可视化视口大小的对象 渲染组件
    /// </summary>
    SpriteRenderer rend;

    /// <summary>
    /// 可视化视口摄像头
    /// </summary>
    Camera viewCam;

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
    /// 激活对象在视口中的坐标
    /// </summary>
    public Vector2 value = new Vector2(0.5f, 0.5f);

    /// <summary>
    /// 是否在有效范围中点击了一次
    /// </summary>
    bool clickInView = false;

    /// <summary>
    /// 是否暂停
    /// </summary>
    bool pause = true;

    /// <summary>
    /// 平滑移动的时间
    /// </summary>
    float deltaTime;

    /// <summary>
    /// 覆盖 基类 的 Start
    /// </summary>
    protected override void Start()
    {
        // 只需要 collider
        collider = GetComponent<BoxCollider>();

        // 寻找 控制主摄像头的 父对象
        viewArea = transform.Find("View Area").gameObject;
        rend = viewArea.GetComponent<SpriteRenderer>();

        // 设置 对象的 层
        childLayer = viewArea.layer;

        // 保存当前的视口范围为 最大的范围
        maxViewSize = Camera.main.orthographicSize;

        // 保存当前的缩放大小
        maxImageSize = viewArea.transform.localScale.x;

        // 获取 视口可视化摄像头
        viewCam = cam;

    }

    /// <summary>
    /// 每帧执行一次， 不受时间控制
    /// </summary>
    void Update()
    {
        // 如果暂停，直接返回
        if (pause) return;

        // 已经有另一实例在工作
        if (workingId != 0 && workingId != GetInstanceID()) return;

        // 更新时间
        deltaTime += Time.deltaTime;

        // 在按下按钮瞬间，重置时间
        if (Input.GetButtonDown("Fire1"))
            deltaTime = 0;

        // 判断是否按下鼠标
        if (!Input.GetButton("Fire1") || !MoveValue())
        {
            // 清除工作标记
            workingId = 0;

            // 清除激活的对象
            activeObject = null;

            // 需要在有效范围中点动一次
            clickInView = false;

            return;
        }


        // 标记当前实例在工作
        workingId = GetInstanceID();

        // 设置激活对象
        activeObject = viewArea;

        // 移动对象
        MoveObject(cam.ViewportToScreenPoint(value));

    }

    /// <summary>
    /// 禁用 基类 的 FixedUpdate
    /// </summary>
    protected override void FixedUpdate()
    {

    }

    

    bool MoveValue()
    {

        // 获取当前的鼠标在 视口中坐标
        Vector2 viewport = cam.ScreenToViewportPoint(Input.mousePosition);

        // 判断是否在视口范围内
        if (!viewport.IsInView())
        {
            // 不是有效的点击，直接返回
            if (!clickInView)
                return false;
            // 否则，把坐标裁剪到视口中
            else
            {
                viewport = viewport.ClipInView();
            }
        }
        else
        {
            // 在有效范围中点击
            clickInView = true;
        }


        // 移动位置视口坐标
        value = Vector2.Lerp(value, viewport, deltaTime);

        // 重置 delta time
        deltaTime = 0;

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

    // 显示或 隐藏 视口
    public void Toggle(bool show)
    {
        // 切换 暂停
        pause = !show;

        // 显示或 隐藏  Camera
        viewCam.enabled = show;

        // 显示或 隐藏  图像显示
        rend.enabled = show;
    }



}
