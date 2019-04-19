using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableDictionaryObjectGOFl : SerializableDictionaryObject<GameObject, float>
{
    //empty
}

public class SerializableDictionaryObject<K, V> : TrickTheEditorParent
{
    [SerializeField] public K key;
    [SerializeField] public V value;
}

[System.Serializable]
public class TrickTheEditorParent
{
    //empty
}
