using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DirtCard : MonoBehaviour {
    public Sprite[] _nominalSprite;
    public Image _image;

    private void OnEnable()
    {
        _image.sprite = _nominalSprite[Random.Range(0, _nominalSprite.Length)];
    }
}
