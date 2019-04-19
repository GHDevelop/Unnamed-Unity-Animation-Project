using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Manually Set")]

    [Header("Handles")]

    [Tooltip("Where the left hand of the character grabs."),
        SerializeField] private Transform _lHandle;
    public Transform LHandle
    {
        get { return _lHandle; }
        private set {_lHandle = value; }
    }

    [Tooltip("Where the right hand of the character grabs."),
        SerializeField] private Transform _rHandle;
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

    [Header("AI Parameters")]
    [SerializeField] private float _firingRadius;
    public float FiringRadius
    {
        get
        {
            return _firingRadius;
        }

        set
        {
            _firingRadius = value;
        }
    }

    [SerializeField] private float _firingRange;
    public float FiringRange
    {
        get
        {
            return _firingRange;
        }

        set
        {
            _firingRange = value;
        }
    }

    [Header("Viewing Only")]

    [Tooltip("Weapon's Transform (don't touch)"),
        SerializeField] private Transform _myTransform;
    public Transform MyTransform
    {
        get { return _myTransform; }
        private set { _myTransform = value; }
    }

    [Tooltip("The animator for this weapon"),
        SerializeField] private Animator _animator;
    public Animator Animator
    {
        get { return _animator; }
        protected set { _animator = value; }
    }

    [Tooltip("If this is present, collision with this object will deal damage."),
        SerializeField] private CollisionsDealDamage _damageFlag;
    public CollisionsDealDamage DamageFlag
    {
        get { return _damageFlag; }
        protected set { _damageFlag = value; }
    }

    /// <summary>
    /// GetComponent
    /// </summary>
    void Start()
    {
        MyTransform = GetComponent<Transform>();
        Animator = GetComponent<Animator>();
        DamageFlag = GetComponent<CollisionsDealDamage>();
    }

    /// <summary>
    /// Nothing
    /// </summary>
    void Update()
    {
        //Do nothing for now
    }

    /// <summary>
    /// Plays an animation for "swinging" the weapon
    /// </summary>
    public virtual void Fire()
    {
        if (Animator)
        {
            Animator.Play("Swing");
        }
    }
}
