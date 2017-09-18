using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 继承 NGUI UIDragObject,拖动时暂停游戏
/// </summary>
public class DragObject : UIDragObject
{
    /// <summary>
    /// 拖拽开始
    /// </summary>
    void OnDragStart()
    {
        // 停止时间更新
        Time.timeScale = 0;
    }


    /// <summary>
    /// 拖拽结束
    /// </summary>
    void OnDragEnd()
    {
        // 恢复时间更新
        Time.timeScale = 1;
    }
}
