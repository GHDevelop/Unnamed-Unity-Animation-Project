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
    [SerializeField] private Weapon equippedWeapon;

    [Header("Components")]
    [SerializeField] private Animator animator;

    // Start is called before the first frame update
    void Awake()
    {
        InitializeWeaponPositions();
        animator = GetComponent<Animator>();
    }

    //index 0 will be for no weapon equipped, the transform for that position will be the container for the weapon positions
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

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EquipWeapon(Weapon weapon)
    {
        UnequipCurrentlyEquippped();

        if (weapon != null)
        {
            equippedWeapon = weapon;
            equippedWeapon.MyTransform.parent = transform;
            equippedWeapon.MyTransform.position = WeaponPositions[equippedWeapon.WeaponType].position;
            equippedWeapon.MyTransform.rotation = WeaponPositions[equippedWeapon.WeaponType].rotation;
            animator.SetInteger(weaponAnimatorParamName, (int)equippedWeapon.WeaponType);
        }
    }

    private void UnequipCurrentlyEquippped()
    {
        if (this.equippedWeapon)
        {
            Destroy(this.equippedWeapon.gameObject);
            equippedWeapon = null;
            animator.SetInteger(weaponAnimatorParamName, 0);
        }
    }
}
