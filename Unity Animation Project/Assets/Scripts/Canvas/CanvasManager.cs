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
            float? valueAsNumber = float.Parse(value) * 100;
            if (valueAsNumber != null)
            {
                valueAsNumber = Mathf.Ceil((float)valueAsNumber);
                string messageToInsertInText = string.Format(HPTextString, valueAsNumber);
                _hpPercentText.text = messageToInsertInText;
            }
        }
    }

    [Tooltip("The slider used to represent hp behind the text"),
        SerializeField] private Slider _hpSlider;
    public float HPSlider
    {
        set
        {
            _hpSlider.value = value;
        }
    }

    [Tooltip("The text used to represent the equipped weapon type on the character. Meant to be set in WeaponManager.cs"),
        SerializeField] private Text _weaponTypeText;
    public string WeaponTypeText
    {
        get { return _weaponTypeText.text; }
        set { _weaponTypeText.text = value; }
    }

    [Tooltip("The text used to represent the remaining number of lives"),
        SerializeField] private Text _livesText;
    public int LivesText
    {
        set
        {
            if (_livesText != null)
            {
                string livesString = "";
                for (int index = 0; index < value; index++)
                {
                    livesString += "❤️";
                }

                _livesText.text = livesString;
            }
        }
    }

    [Header("Viewing Only")]

    [Tooltip("The canvas attached to the Canvas manager's object. Set automatically"),
        SerializeField] private Canvas _attachedCanvas;
    public Canvas AttachedCanvas
    {
        get { return _attachedCanvas; }
        set { _attachedCanvas = value; }
    }

    [Tooltip("The attached transform, used to rotate canvas to camera if it is rendered in world space"),
        SerializeField] private Transform _myTransform;
    public Transform MyTransform
    {
        get
        {
            return _myTransform;
        }

        set
        {
            _myTransform = value;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        AttachedCanvas = GetComponent<Canvas>();
        MyTransform = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        //Does nothing
    }

    public void UpdateHPGraphics(float newHPPercent)
    {
        HPPercentText = newHPPercent.ToString();
        HPSlider = newHPPercent;
    }
}
