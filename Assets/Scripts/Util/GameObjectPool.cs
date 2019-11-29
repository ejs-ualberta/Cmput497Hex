using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectPool
{
    public GameObject BaseObject {get; private set;}
    public Transform Parent {get;private set;}
    private List<GameObject> _inactive = new List<GameObject>();
    private List<GameObject> _active = new List<GameObject>();

    public GameObjectPool(GameObject baseObject,Transform parent){
        BaseObject = baseObject;
        Parent = parent;
        foreach(Transform child in Parent.transform)
            _inactive.Add(child.gameObject);
    }

    public void Retire(GameObject gameObject){
        gameObject.SetActive(false);
        _active.Remove(gameObject);
        if(_inactive.Contains(gameObject))   
           return;
        
        _inactive.Add(gameObject);
        
    }

    public void RetireAll(){
        foreach(var obj  in _active){
            obj.SetActive(false);
            _inactive.Add(obj);
        }

        _active.Clear();
    }

    public GameObject Request(){
        if(_inactive.Count == 0){
            var newObj = UnityEngine.Transform.Instantiate(BaseObject,Parent);
            _active.Add(newObj);
            return newObj;
        }

        var obj = _inactive[0];
        _inactive.RemoveAt(0);
        if(_active.Contains(obj)){
            Debug.LogError("Active already has " + obj.name);
        }
        _active.Add(obj);
        obj.SetActive(true);
        return obj;
    }

    public bool IsObjectActive(GameObject gameObject){
        return _active.Contains(gameObject);
    }
    
}
