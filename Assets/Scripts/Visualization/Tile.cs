using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {

    private Material _canonicalMaterial = null;

    internal bool IsPlayable { get; set; }
    internal Vector2Int Location { get; set; }
    internal Material CanonicalMaterial {
        set{
            _canonicalMaterial = value;
            GetComponentInChildren<MeshRenderer>().material = _canonicalMaterial;            
        }
        get{ return _canonicalMaterial;}
    }

    internal Material Material{
        set{
            GetComponentInChildren<MeshRenderer>().material = value;
        }
        get{
            return GetComponentInChildren<MeshRenderer>().material;
        }
    }

    internal string Text{
        set{
            GetComponentInChildren<TextSizer>().Text = value;
        }
        get{
            return GetComponentInChildren<TextSizer>().Text;
        }
    }

    internal Color TextColor{
        set{
            GetComponentInChildren<TextMesh>().color = value;
        }
    }
}
