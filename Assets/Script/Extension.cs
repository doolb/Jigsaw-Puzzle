using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extension {

	public static int MaskToLayer(this LayerMask mask)
    {
        int m = mask.value;
        for(int i=0;i<32;i++)
        {
            if ((m & 1) == 1) return i;
            m >>= 1;
        }

        return 0;
    }
}

