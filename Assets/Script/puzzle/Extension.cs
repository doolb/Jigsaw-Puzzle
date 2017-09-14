using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 扩展 unity 对象
/// </summary>
public static class Extension
{
    /// <summary>
    /// 判断 vector2 向量 是否在视口中
    /// </summary>
    /// <param name="viewport">视口坐标</param>
    /// <returns></returns>
    public static bool IsInView(this Vector2 viewport)
    {
        // 视口范围 为 （0，1），再加上 判断边界值
        if (viewport.x < 0 || viewport.x > 1)
            return false;
        if (viewport.y < 0 || viewport.y > 1)
            return false;

        return true;
    }


    /// <summary>
    /// 获取 Vector2 的法线
    /// </summary>
    /// <param name="v">要获取法线的向量</param>
    /// <returns>法线的向量</returns>
    public static Vector2 GetNormal(this Vector2 v)
    {
        // 反转 x,y ,再加个负号
        return new Vector2(v.y, -v.x);
    }

    /// <summary>
    /// 计算两条直线的交点
    /// </summary>
    /// <param name="p1">第一条直线的第一点</param>
    /// <param name="p2">第一条直线的第二点</param>
    /// <param name="p3">第二条直线的第一点</param>
    /// <param name="p4">第二条直线的第二点</param>
    /// <param name="intersection">返回的交点</param>
    /// <param name="t">交点所在的偏移</param>
    /// <returns></returns>
    public static bool LineIntersection(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, out Vector2 intersection, out float t)
    {
        // 设置默认值
        t = 0;
        intersection = Vector2.zero;

        // 得到直线的向量
        Vector2 vb = p2 - p1;
        Vector2 vd = p4 - p3;
        Vector2 vc = p3 - p1;

        // 获取直线向量的法线
        Vector2 vbn = vb.GetNormal();
        Vector2 vdn = vd.GetNormal();

        float r, s;

        // 第一条的向量和第二条的法线 作点积
        r = Vector2.Dot(vb, vdn);

        // 如果结果为 0 ，表示两条直线平行，直接返回
        if (r == 0) return false;

        // 计算交点在第一条直线上的偏移
        t = Vector2.Dot(vc, vdn) / r;

        // 计算交点在第二条直线上的偏移
        s = Vector2.Dot(-vc, vbn) / Vector2.Dot(vd, vbn);

        // 判断交点的有效性
        if (t > 0 && t < 1 && s > 0 && s < 1)
        {
            // 返回交点
            intersection = p1 + vb * t;
            return true;
        }
        return false;
    }

    /// <summary>
    /// 把直线裁剪到单位 钜形中
    /// </summary>
    /// <param name="point">要裁剪的点</param>
    /// <returns></returns>
    public static Vector2 ClipInView(this Vector2 point)
    {
        // 钜形的中心
        Vector2 center = Vector2.one * .5f;

        // 保存裁剪后的点
        Vector2 o = center;

        // 裁剪后的偏移
        float time = 10000f;

        // 缓存每次交点
        Vector2 inter = Vector2.zero;

        // 缓存每次偏移
        float t;


        // 下边界
        if (LineIntersection(center, point, Vector2.zero, new Vector2(1, 0), out inter, out t))
        {
            // 如果偏移有效，并且更小
            if (t < time && t > 0)
            {
                // 更新交点
                time = t;
                o = inter;
            }
        }

        // 上边界
        if (LineIntersection(center, point, new Vector2(0, 1), new Vector2(1, 1), out inter, out t))
        {
            // 如果偏移有效，并且更小
            if (t < time && t > 0)
            {
                // 更新交点
                time = t;
                o = inter;
            }
        }

        // 左边界
        if (LineIntersection(center, point, new Vector2(0, 0), new Vector2(0, 1), out inter, out t))
        {
            // 如果偏移有效，并且更小
            if (t < time && t > 0)
            {
                // 更新交点
                time = t;
                o = inter;
            }
        }

        // 右边界
        if (LineIntersection(center, point, new Vector2(1, 0), new Vector2(1, 1), out inter, out t))
        {
            // 如果偏移有效，并且更小
            if (t < time && t > 0)
            {
                // 更新交点
                time = t;
                o = inter;
            }
        }

        // 返回交点
        return o;
    }


}