using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour {

    static int maxDepth = 0;
    static GameObject topGameObject;

    SpriteRenderer sprite;

	// Use this for initialization
	void Start () 
    {
        sprite = GetComponent<SpriteRenderer>();
	}

    void OnPress(bool isPressed)
    {
        // 是否是最顶的对象
        if(topGameObject != gameObject)
        {
            topGameObject = gameObject;
            sprite.sortingOrder = ++maxDepth;
        }
    }
}
