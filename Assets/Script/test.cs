using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(UITexture))]
public class test : MonoBehaviour
{


    [Header("Image")]
    public Texture image;

    public Vector2 imageTile;

    public Vector2 imageOffset;

    [Header("Mark")]

    public Texture mark;

    public Vector2 markTile;

    public Vector2 markOffset;


    UITexture rend;
    


    // Use this for initialization
    void Start()
    {

        rend = GetComponent<UITexture>();

        rend.material = Resources.Load<Material>("puzzle");
        rend.onRender  +=SetMaterialValue;

    }


    void SetMaterialValue(Material mat)
    {

        if (mat != null)
        {
            mat.SetTexture("_MarkTex", mark);
            mat.SetTextureScale("_MarkTex", markTile);
            mat.SetTextureOffset("_MarkTex", markOffset);

            rend.mainTexture = image;
            mat.SetTextureScale("_MainTex", imageTile);
            mat.SetTextureOffset("_MainTex", imageOffset);
        }   

    }
}
