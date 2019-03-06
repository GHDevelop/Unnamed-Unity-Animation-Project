using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    [SerializeField] private Canvas _attachedCanvas;
    public Canvas AttachedCanvas
    {
        get { return _attachedCanvas; }
        set { _attachedCanvas = value; }
    }

    [SerializeField] private string _hpTextString;
    public string HPTextString
    {
        get { return _hpTextString; }
        private set { _hpTextString = value; }
    }

    [SerializeField] private Text _hpPercentText;
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
    // Start is called before the first frame update
    void Start()
    {
        AttachedCanvas = GetComponent<Canvas>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
