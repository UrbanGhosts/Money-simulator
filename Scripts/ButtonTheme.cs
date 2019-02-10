using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonTheme : MonoBehaviour {
    public Purchaser pur;
    public string nameProduct;
    public int idTheme;

	// Use this for initialization
	public void Tap () {
		if (Purchaser.CheckBuyState(nameProduct) == true)
        {
            CartController.thimeCart = idTheme;
            PlayerPrefs.SetInt("THEME_MONEY", CartController.thimeCart);
        }
        else
        {
            pur.BuyConsumable(nameProduct);
        }
	}
}
