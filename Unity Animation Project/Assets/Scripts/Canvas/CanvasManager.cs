using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    [Header("Set Manually")]

    [Tooltip("The string used to represent the HP Text's format. \nMust use {0} in the string to represent where the HP% will be placed in the string"),
        SerializeField] private string _hpTextString;
    public string HPTextString
    {
        get { return _hpTextString; }
        private set { _hpTextString = value; }
    }

    [Tooltip("The text used to represent the HP on the character. Uses the HPTextString propery as the format. Meant to be set in HealthWithSelfDestruct.cs"),
        SerializeField] private Text _hpPercentText;
    public string HPPercentText
    {
        get { return _hpPercentText.text; }
        set
        {
            float? valueAsNumber = float.Parse(value);
            if (valueAsNumber != null)
            {
                valueAsNumber = Mathf.Ceil((float)valueAsNumber);
                string messageToInsertInText = string.Format(HPTextString, valueAsNumber);
                _hpPercentText.text = messageToInsertInText;
            }
        }
    }

    [Tooltip("The text used to represent the equipped weapon type on the character. Meant to be set in WeaponManager.cs"),
        SerializeField] private Text _weaponTypeText;
    public string WeaponTypeText
    {
        get { return _weaponTypeText.text; }
        set { _weaponTypeText.text = value; }
    }

    [Header("Viewing Only")]

    [Tooltip("The canvas attached to the Canvas manager's object. Set automatically"),
        SerializeField]
    private Canvas _attachedCanvas;
    public Canvas AttachedCanvas
    {
        get { return _attachedCanvas; }
        set { _attachedCanvas = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        AttachedCanvas = GetComponent<Canvas>();
    }

    // Update is called once per frame
    void Update()
    {
        //Does nothing for now.
    }
}
