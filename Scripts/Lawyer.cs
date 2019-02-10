using UnityEngine.UI;
using UnityEngine;
using System;

public class Lawyer : MonoBehaviour {
    public Button _button;

    void OnEnable ()
    {
        if (PlayerPrefs.HasKey("LAWYER_TIME") == true)
        {
            _button.interactable = ((DateTime.Now.DayOfYear - PlayerPrefs.GetInt("LAWYER_TIME")) > 7) ? false : true;
        }
    }
}
