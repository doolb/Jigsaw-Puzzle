using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Renderer))]
public class Background : MonoBehaviour {

    Renderer rend;

	// Use this for initialization
	void Start () {
        rend = GetComponent<Renderer>();

        rend.material.mainTexture = Resources.Load<Texture>("Image/bg1");
	}
	
	public void ChangeBackground()
    {
        string name = UIPopupList.current.value;
        if(rend == null) return;
        rend.material.mainTexture = Resources.Load<Texture>("Image/" + name);
    }
}
