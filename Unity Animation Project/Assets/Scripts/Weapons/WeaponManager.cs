using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private string weaponAnimatorParamName = "EquippedWeapon";
    [SerializeField] private Transform weaponPositionParent;

    /*[SerializeField] doesn't work for dictionaries*/ private Dictionary<WeaponEnum, Transform> _weaponPositions;
    public Dictionary<WeaponEnum, Transform> WeaponPositions
    {
        get { return _weaponPositions; }
        private set { _weaponPositions = value; }
    }

    [Header("Viewing only")]
    [SerializeField] private Weapon _equippedWeapon;
    public Weapon EquippedWeapon
    {
        get { return _equippedWeapon; }
        set
        {
            _equippedWeapon = value;
            if (_equippedWeapon)
            {
                SetEquippedWeaponPosition();
                BlockWeaponFromHittingOtherCollidersSpawnedFromSameCharacter();
                AddExemptionFromDamageToWeapon();
                EnableWeaponDamage();
                UpdateAnimatorWithWeaponType(_equippedWeapon.WeaponType);
                UpdateCanvasWithWeaponType(_equippedWeapon.WeaponType);
            }
        }
    }

    //Set by controller
    [SerializeField] private CanvasManager _associatedCanvas;
    public CanvasManager AssociatedCanvas
    {
        get { return _associatedCanvas; }
        set { _associatedCanvas = value; }
    }

    [Header("Components")]
    [SerializeField] private HealthWithSelfDestruct health;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform myTransform;

    // Start is called before the first frame update
    void Awake()
    {
        InitializeWeaponPositions();
        health = GetComponent<HealthWithSelfDestruct>();
        animator = GetComponent<Animator>();
        myTransform = GetComponent<Transform>();
    }

    //Makes sure this occurs after the controller sets the associated canvas
    private void Start()
    {
        if (AssociatedCanvas)
        {
            AssociatedCanvas.WeaponTypeText = WeaponEnum.None.ToString();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z) && EquippedWeapon)
        {
            EquippedWeapon.Fire();
        }
    }

    /// <summary>
    /// index 0 will be for no weapon equipped, the transform for that position will be the container for the weapon positions
    /// </summary>
    private void InitializeWeaponPositions()
    {
        WeaponPositions = new Dictionary<WeaponEnum, Transform>();
        WeaponEnum[] enumValueArray = EnumCounter.GetEnumValues(typeof(WeaponEnum)) as WeaponEnum[];
        Transform[] weaponPositionArray = weaponPositionParent.GetComponentsInChildren<Transform>();
        for (int index = 0; index < enumValueArray.Length; index++)
        {
            WeaponPositions.Add(enumValueArray[index], weaponPositionArray[index]);
            Debug.Log(enumValueArray[index] + " " + weaponPositionArray[index]);
        }
    }

    /// <summary>
    /// Equip a new weapon. Most of the work is done in EquippedWeapon's setter
    /// </summary>
    /// <param name="weapon"></param>
    public void EquipWeapon(Weapon weapon)
    {
        UnequipCurrentlyEquippped();

        if (weapon != null)
        {
            EquippedWeapon = weapon;
        }
    }

    /// <summary>
    /// Destroys an equipped weapon and sets the equipped weapon to none in the animator.
    /// </summary>
    private void UnequipCurrentlyEquippped()
    {
        if (this.EquippedWeapon)
        {
            Destroy(this.EquippedWeapon.gameObject);
            EquippedWeapon = null;
            UpdateAnimatorWithWeaponType(WeaponEnum.None);
        }
    }

    /// <summary>
    /// Set newly equipped weapon's position, rotation, and parent
    /// </summary>
    private void SetEquippedWeaponPosition()
    {
        EquippedWeapon.MyTransform.parent = myTransform;
        EquippedWeapon.MyTransform.position = WeaponPositions[EquippedWeapon.WeaponType].position;
        EquippedWeapon.MyTransform.rotation = WeaponPositions[EquippedWeapon.WeaponType].rotation;
    }

    /// <summary>
    /// Prevents weapon from hitting other weapons spawned by the same character, or hitting the character himself
    /// </summary>
    private void BlockWeaponFromHittingOtherCollidersSpawnedFromSameCharacter()
    {
        EquippedWeapon.gameObject.layer = this.gameObject.layer;
    }

    /// <summary>
    /// Grants this object immunity from damage by the equipped weapon.
    /// </summary>
    private void AddExemptionFromDamageToWeapon()
    {
        if (EquippedWeapon.DamageFlag)
        {
            EquippedWeapon.DamageFlag.exempt.Add(health);
        }
    }

    private void EnableWeaponDamage()
    {
        if (EquippedWeapon.DamageFlag)
        {
            EquippedWeapon.DamageFlag.activeHitbox = true;
        }
    }

    /// <summary>
    /// Updates weapon information in the animator.
    /// </summary>
    private void UpdateAnimatorWithWeaponType(WeaponEnum weaponType)
    {
        animator.SetInteger(weaponAnimatorParamName, (int)weaponType);
    }

    /// <summary>
    /// Updates the canvas with the weapon type.
    /// </summary>
    private void UpdateCanvasWithWeaponType(WeaponEnum weaponType)
    {
        if (AssociatedCanvas)
        {
            AssociatedCanvas.WeaponTypeText = weaponType.ToString();
        }
    }

    /// <summary>
    /// Sets the animator IK for both the left and right hand if a weapon is equipped.
    /// </summary>
    /// <param name="layerIndex"></param>
    private void OnAnimatorIK(int layerIndex)
    {
        if (EquippedWeapon == null)
        {
            return;
        }

        SetIKForHand(EquippedWeapon.LHandle, AvatarIKGoal.LeftHand, 1.0f);
        SetIKForHand(EquippedWeapon.RHandle, AvatarIKGoal.RightHand, 1.0f);
    }


    /// <summary>
    /// Set IK position and weight for a given hand.
    /// </summary>
    /// <param name="handle"></param>
    /// <param name="goal"></param>
    /// <param name="weight"></param>
    private void SetIKForHand(Transform handle, AvatarIKGoal goal, float weight)
    {
        if (handle)
        {
            animator.SetIKPosition(goal, handle.position);
            animator.SetIKPositionWeight(goal, weight);
            animator.SetIKRotation(goal, handle.rotation);
            animator.SetIKRotationWeight(goal, weight);
        }
        else
        {
            animator.SetIKPositionWeight(goal, 0f);
            animator.SetIKRotationWeight(goal, 0f);
        }
    }
}
