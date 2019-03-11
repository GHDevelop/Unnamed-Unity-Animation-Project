using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Weapon))]
public class WeaponPickup : Pickup
{
    [Header("Weapon Component")]
    [Tooltip("Contains the attached WeaponComponent. Set automatically. \nDO NOT TOUCH"),
        SerializeField] private Weapon _weaponInfo;
    public Weapon WeaponInfo
    {
        get { return _weaponInfo; }
        protected set { _weaponInfo = value; }
    }

    /// <summary>
    /// Gets the Weapon component
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        WeaponInfo = GetComponent<Weapon>();
    }

    /// <summary>
    /// Currently does not deviate from the base update
    /// </summary>
    protected override void Update()
    {
        base.Update();
    }

    /// <summary>
    /// For WeaponPickup, OnCollect searches for a weapon manager, attaches itself to that weapon manager, and destroys the pickup component.
    /// </summary>
    /// <param name="other"></param>
    protected override void OnCollect(Collider other)
    {
        WeaponManager characterWeaponManager = other.GetComponentInParent<WeaponManager>();
        Weapon collidingObjectAsWeapon = other.GetComponentInParent<Weapon>();

        if (characterWeaponManager == null || collidingObjectAsWeapon != null)
        {
            return;
        }

        characterWeaponManager.EquipWeapon(WeaponInfo);
        Destroy(this);
    }
}
