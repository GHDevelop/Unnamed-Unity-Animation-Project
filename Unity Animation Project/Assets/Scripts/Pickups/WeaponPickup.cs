using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Weapon))]
public class WeaponPickup : Pickup
{
    private Weapon weaponInfo;

    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        weaponInfo = GetComponent<Weapon>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    protected override void OnCollect(Collider other)
    {
        WeaponManager characterWeaponManager = other.GetComponentInParent<WeaponManager>();
        if (characterWeaponManager == null)
        {
            return;
        }

        characterWeaponManager.EquipWeapon(weaponInfo);
        Destroy(this);
    }

    
}
