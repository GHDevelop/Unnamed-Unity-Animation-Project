using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Weapon's Transform (don't touch)")]
    [SerializeField] private Transform _myTransform;
    public Transform MyTransform
    {
        get { return _myTransform; }
        private set { _myTransform = value; }
    }

    [Header("Handles")]
    [SerializeField] private Transform _lHandle;
    public Transform LHandle
    {
        get { return _lHandle; }
        private set {_lHandle = value; }
    }

    [SerializeField] private Transform _rHandle;
    public Transform RHandle
    {
        get { return _rHandle; }
        private set { _rHandle = value; }
    }

    [Header("WeaponType")]
    [SerializeField] private WeaponEnum _weaponType;
    public WeaponEnum WeaponType
    {
        get { return _weaponType; }
        private set { _weaponType = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        MyTransform = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
