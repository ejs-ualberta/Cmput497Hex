using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(TextMesh))]
public class TextSizer : MonoBehaviour
{
    //Sets the text that appears on a 3D object and automatically changes the size based on text length;
    private readonly Dictionary<int,int> PieceSizes = new Dictionary<int,int>(){ {1,5},{2,4},{3,3} };
    private readonly Dictionary<int,int> TileSizes = new Dictionary<int,int>(){ {1,8},{2,7},{3,5} };
    internal string Text{
        get{
            return GetComponent<TextMesh>().text;
        }
        set{
            var comp = GetComponent<TextMesh>();
            comp.text = value;  
            var len = comp.text.Length;
            var sizes = (GetComponentInParent<Tile>() != null) ? TileSizes : PieceSizes;
            if(sizes.ContainsKey(len)){
                comp.characterSize = sizes[len];
            }
        }
    }
}

