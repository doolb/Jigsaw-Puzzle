using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 拖拽摄像机的类，从ngui 中复制
/// </summary>
public class DragCameraVisual : MonoBehaviour
{
    /// <summary>
    /// 要拖拽的目标
    /// </summary>
    public UIDraggableCamera draggableCamera;

    /// <summary>
    /// 开始拖拽事件
    /// </summary>
    public List<EventDelegate> onDragStart = new List<EventDelegate>();

    /// <summary>
    /// 拖拽结束事件
    /// </summary>
    public List<EventDelegate> onDragEnd = new List<EventDelegate>();

    /// <summary>
    /// Automatically find the draggable camera if possible.
    /// </summary>

    void Awake()
    {
        if (draggableCamera == null)
        {
            draggableCamera = NGUITools.FindInParents<UIDraggableCamera>(gameObject);
        }
    }

    /// <summary>
    /// Forward the press event to the draggable camera.
    /// </summary>

    void OnPress(bool isPressed)
    {
        if (enabled && NGUITools.GetActive(gameObject) && draggableCamera != null)
        {
            draggableCamera.Press(isPressed);
        }
    }

    /// <summary>
    /// Forward the drag event to the draggable camera.
    /// </summary>

    void OnDrag(Vector2 delta)
    {
        if (enabled && NGUITools.GetActive(gameObject) && draggableCamera != null)
        {
            // 反转 delta
            draggableCamera.Drag(-delta);
        }
    }

    /// <summary>
    /// Forward the scroll event to the draggable camera.
    /// </summary>

    void OnScroll(float delta)
    {
        if (enabled && NGUITools.GetActive(gameObject) && draggableCamera != null)
        {
            // 反转 delta
            draggableCamera.Scroll(-delta);
        }
    }

    /// <summary>
    /// 拖拽开始
    /// </summary>
    void OnDragStart()
    {
        if (enabled && NGUITools.GetActive(gameObject) && draggableCamera != null)
        {
            // 通知 拖拽开始
            for (int i = 0; i < onDragStart.Count; i++)
                onDragStart[i].Execute();
        }
    }

    /// <summary>
    /// 拖拽结束
    /// </summary>
    void OnDragEnd()
    {
        if (enabled && NGUITools.GetActive(gameObject) && draggableCamera != null)
        {
            // 通知 拖拽结束
            for (int i = 0; i < onDragEnd.Count; i++)
                onDragEnd[i].Execute();
        }
    }
}
