using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CartController : MonoBehaviour {
    public AudioSource _source;
    public AudioClip[] _clips;
    public Shaking _shake;
    public GameObject _dirt;
    public GameMeneger gm;
    private Image _image;
    private int _nominal;
    private int i;
    private bool isDead;
    private float dirtCount = 1f;
    public static float dirtMinCount = 0.5f; // Upgrade
    public static int thimeCart = 0;

    [Serializable]
    public class SubList
    {
        public List<Sprite> list = new List<Sprite>();
    }
    public List<SubList> grounsTheme = new List<SubList>();


    private void Awake()
    {
        if (PlayerPrefs.HasKey("THEME_MONEY") == true)
        {
            thimeCart = PlayerPrefs.GetInt("THEME_MONEY");
        }
        _image = GetComponent<Image>();
        SetNominal();
    }

    // Update is called once per frame
    void Update () {
        if (gm.cards.Contains(this.transform) && gm.cards.IndexOf(this.transform) == 0 && isDead == false && gm.menuPanel.activeSelf == false)
        {
#if UNITY_STANDALONE || UNITY_EDITOR

            if (Input.GetKey(KeyCode.Mouse0))
            {
                this.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                float x = Mathf.Clamp(this.transform.localPosition.x, -10f, 10f);
                float y = Mathf.Clamp(this.transform.localPosition.y, -10f, 10f);
                this.transform.localPosition = new Vector2(x, y);
            }
            else if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                gm.Invoke("NextCart", .25f);
                isDead = true;
                //i = (this.transform.localPosition.x > 0) ? i = 1 : i = -1;
                i = (Camera.main.ScreenToWorldPoint(Input.mousePosition).normalized.x > 0) ? i = 1 : i = -1;
                _source.PlayOneShot((i < 0) ? _clips[0] : _clips[1]);
                if (_dirt.activeSelf == true && i < 0)
                {
                    _shake.enabled = true;
                    gm.MistakeScore(_nominal);
                }
                else if ( _dirt.activeSelf == false && i > 0)
                {
                    _shake.enabled = true;
                }

                //Cashebake
                else if (_dirt.activeSelf == true && i > 0)
                {
                    if(gm.casheBack != gm.cashebackCurrent)
                    {
                        gm.cashebackCurrent++;
                    }
                    else
                    {
                        gm.cashebackCurrent = 1;
                        gm.Score(_nominal);
                    }
                }

                gm.Score(i < 0 ? _nominal : 0);
                MoveCard();
                Invoke("ResetCard", 0.65f);
            }
#else
            if (Input.touches.Length > 0)
            {
                Touch touch = Input.touches[0];

                if (touch.phase == TouchPhase.Moved)
                {
                    this.transform.position = Camera.main.ScreenToWorldPoint(touch.position);
                    float x = Mathf.Clamp(this.transform.localPosition.x, -10f, 10f);
                    float y = Mathf.Clamp(this.transform.localPosition.y, -10f, 10f);
                    this.transform.localPosition = new Vector2(x, y);
                }
                else if (touch.phase == TouchPhase.Ended)
                {
                    gm.Invoke("NextCart", .25f);
                    isDead = true;
                    //i = (this.transform.localPosition.x > 0) ? i = 1 : i = -1;
                    i = (Camera.main.ScreenToWorldPoint(touch.position).normalized.x > 0) ? i = 1 : i = -1;
                    _source.PlayOneShot((i < 0) ? _clips[0] : _clips[1]);
                    if (_dirt.activeSelf == true && i < 0)
                    {
                        _shake.enabled = true;
                        gm.MistakeScore(_nominal);
                    }
                    else if (_dirt.activeSelf == false && i > 0)
                    {
                        _shake.enabled = true;
                    }


                    //Cashebake
                    else if (_dirt.activeSelf == true && i > 0)
                    {
                        if (gm.casheBack != gm.cashebackCurrent)
                        {
                            gm.cashebackCurrent++;
                        }
                        else
                        {
                            gm.cashebackCurrent = 1;
                            gm.Score(_nominal);
                        }
                    }

                    gm.Score(i < 0 ? _nominal : 0);
                    MoveCard();
                    Invoke("ResetCard", 0.65f);
                }
            }
#endif
        }
        else if (gm.menuPanel.activeSelf == true && dirtCount != 1f)
        {
            dirtCount = 1f;
        }
    }

    private void MoveCard()
    {
        this.transform.localPosition -= transform.up * 20f * i;
        Invoke("MoveCard", 0.02f);
    }

    private void ResetCard()
    {
        CancelInvoke();
        transform.localPosition = Vector2.zero;
        SetNominal();
        isDead = false;
        gm.CartReady();
        i = 0;
    }

    private void SetNominal()
    {
        int rar = UnityEngine.Random.Range(0, 100);

        _dirt.SetActive((UnityEngine.Random.value > dirtCount) ? true : false);
        dirtCount = Mathf.Clamp(dirtCount - 0.05f, dirtMinCount, 1f);

        if (rar < 2)
        {
            _nominal = 1000;
            _image.sprite = grounsTheme[thimeCart].list[5];
        }
        else if (rar < 10)
        {
            _nominal = 500;
            _image.sprite = grounsTheme[thimeCart].list[4];
        }
        else if (rar < 15)
        {
            _nominal = 100;
            _image.sprite = grounsTheme[thimeCart].list[3];
        }
        else if (rar < 25)
        {
            _nominal = 50;
            _image.sprite = grounsTheme[thimeCart].list[2];
        }
        else if (rar < 60)
        {
            _nominal = 25;
            _image.sprite = grounsTheme[thimeCart].list[1];
        }
        else if (rar < 99)
        {
            _nominal = 10;
            _image.sprite = grounsTheme[thimeCart].list[0];
        }
    }
}
