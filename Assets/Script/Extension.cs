using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Unity 类的 扩展函数
/// </summary>
public static class Extension {

    /// <summary>
    /// 获取指定 单一 层掩码 的 层 值
    /// </summary>
    /// <param name="mask">层掩码</param>
    /// <returns>层掩码 的 层 值</returns>
	public static int MaskToLayer(this LayerMask mask)
    {
        // 获取 掩码 的 值
        int m = mask.value;

        // 掩码为 0 ，直接返回
        if(m == 0)  return 0;

        // 逐 bit 位 遍历
        for(int i=0;i<32;i++)
        {
            // 如果当前位 为 ‘1‘， i 就是 层的值
            if ((m & 1) == 1) return i;
            
            // 掩码 右移 一位
            m >>= 1;
        }

        return 0;
    }
}

