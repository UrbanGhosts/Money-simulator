using UnityEngine;
using UnityEngine.UI;

public class CartBackground : MonoBehaviour {

    public Sprite[] _nominalSprite;
    public Image _image;

    private void Awake()
    {
        if (PlayerPrefs.HasKey("THEME_MONEY") == true)
        {
            _image.sprite = _nominalSprite[PlayerPrefs.GetInt("THEME_MONEY")];
        }
    }
}
