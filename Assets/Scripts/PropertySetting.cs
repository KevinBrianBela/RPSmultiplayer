using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PropertySetting : MonoBehaviourPunCallbacks
{
    [SerializeField] Slider slider;
    [SerializeField] TMP_InputField inputField;

    [SerializeField] string propertyKey;

    [SerializeField] float initialValue;

    [SerializeField] float minValue = 1;
    [SerializeField] float maxValue = 100;

    [SerializeField] bool wholeNumbers = true;

    void Start()
    {
        slider.minValue = minValue;
        slider.maxValue = maxValue;
        slider.wholeNumbers = wholeNumbers;
        inputField.contentType = wholeNumbers ? TMP_InputField.ContentType.IntegerNumber : TMP_InputField.ContentType.DecimalNumber;

        if(PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(propertyKey, out var value))
        {
            UpdateSliderInputField((float)value);
        }
        else
        {
            UpdateSliderInputField(initialValue);
        }

        if (PhotonNetwork.IsMasterClient == false)
        {
            slider.interactable = false;
            inputField.interactable = false;
        }
    }

    public void InputFromSlider(float value)
    {
        if (PhotonNetwork.IsMasterClient == false)
            return;

        UpdateSliderInputField(value);
        SetCustomProperty(value);
    }

    public void InputFromField(string stringValue)
    {
        if (PhotonNetwork.IsMasterClient == false)
            return;

        if(float.TryParse(stringValue, out var floatValue))
        {
            floatValue = Mathf.Clamp(floatValue, slider.minValue, slider.maxValue);
            UpdateSliderInputField(floatValue);
            SetCustomProperty(floatValue);
        }
    }

    private void SetCustomProperty(float value)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        var property = new Hashtable();
        property.Add(propertyKey, value);

        PhotonNetwork.CurrentRoom.SetCustomProperties(property);
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.TryGetValue(propertyKey, out var value) && PhotonNetwork.IsMasterClient == false)
        {
            UpdateSliderInputField((float) value);
        }
    }

    private void UpdateSliderInputField(float value)
    {
        var floatValue = (float)value;

        slider.value = floatValue;

        if (wholeNumbers)
            inputField.text = (Mathf.RoundToInt(floatValue)).ToString("D");
        else
            inputField.text = floatValue.ToString("F2");
    }
}
