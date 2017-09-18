using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// 视囗控制脚本
/// </summary>
public class ViewControl : MonoBehaviour
{
    /// <summary>
    /// 目标摄像机
    /// </summary>
    Camera cam;

    /// <summary>
    /// 拖拽脚本
    /// </summary>
    DragCameraVisual drag;

    /// <summary>
    /// 摄像机可视化对象
    /// </summary>
    GameObject visual;


    /// <summary>
    /// 视口摄像机的显示
    /// </summary>
    UIViewport viewport;


    /// <summary>
    /// 最大的摄像头视口范围
    /// </summary>
    float maxViewSize
    {
        get { return GameLoader.instance.puzzleGame.camSize; }
    }

    /// <summary>
    /// 最小缩放等级 
    /// </summary>
    public float minZoom = 0.2f;

    /// <summary>
    /// 当前缩放等级
    /// </summary>
    float zoom = 1;


    /// <summary>
    /// 覆盖 基类 的 Start
    /// </summary>
    void Awake()
    {
        // 获取目标摄像头
        Transform target = GameLoader.instance.uiRootGame.transform.Find("Camera");
        cam = target.GetComponent<Camera>();

        // 挂载拖拽脚本
        UIDraggableCamera dragg = target.gameObject.AddComponent<UIDraggableCamera>();
        // 设置拖拽范围
        dragg.rootForBounds = GameLoader.instance.uiRootView.transform;
        // 设置拖拽速度
        dragg.scale = Vector2.one * 3;

        // 加载辅助显示对象
        visual = NGUITools.AddChild(target.gameObject, Resources.Load<GameObject>("Camera Visualize"));

        // 在游戏摄像机中隐藏
        visual.transform.localPosition = new Vector3(0, 0, -1);

        // 加载拖拽脚本
        drag = gameObject.AddComponent<DragCameraVisual>();
        drag.enabled = false;
        // 设置目标摄像头
        drag.draggableCamera = dragg;

        // 开始拖拽事件
        drag.onDragStart.Add(new EventDelegate(() =>
            {
                // 拖拽是暂停游戏
                Time.timeScale = 0;
            }));

        // 结束拖拽事件
        drag.onDragEnd.Add(new EventDelegate(() =>
            {
                // 拖拽时恢复游戏
                Time.timeScale = 1;
            }));


        // 设置视口摄像机的显示
        viewport = transform.parent.Find("Camera").GetComponent<UIViewport>();
        // 获取视口显示对象
        Transform viewWindow = GameLoader.instance.uiRoot.transform.Find("Panel - UI").Find("View Window");
        // 关联左上角
        viewport.topLeft = viewWindow.Find("TopLeft");
        // 关联右下角
        viewport.bottomRight = viewWindow.Find("BottomRight");


        // 游戏大小改变更新大小
        GameLoader.instance.puzzleGame.onResize.Add(new EventDelegate(() =>
            {
                // 缩放 可视化对象
                visual.transform.localScale = Vector3.one * maxViewSize * zoom * 0.81f;
                
                // 缩放 拖拽范围
                transform.localScale = visual.transform.localScale * 1.5f;

                // 缩放 视口范围
                viewport.fullSize = 9 * maxViewSize * zoom;
            }));
    }

    /// <summary>
    /// 缩放主视口
    /// </summary>
    /// <param name="zoom">缩放大小</param>
    public void Zoom(float _zoom)
    {
        // 是否小于最小缩放等级
        if (_zoom < minZoom) zoom = minZoom;
        else zoom = _zoom;

        // 缩放视口
        cam.orthographicSize = maxViewSize * zoom;

        // 缩放 可视化对象
        visual.transform.localScale = Vector3.one * maxViewSize * zoom * 0.9f;
    }

    /// <summary>
    /// 显示或 隐藏 视口
    /// </summary>
    /// <param name="show">是否显示</param>
    public void Toggle(bool show)
    {
        // 显示或 隐藏  Camera
        drag.enabled = show;

        // 显示或 隐藏  图像显示
        visual.SetActive(show);
    }
}
