using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;
using System;
using GooglePlayGames;
using UnityEngine.Purchasing;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;
//using UnityEngine.Monetization;

public class GameMeneger : MonoBehaviour
{
    public Button _lawerButton; //Of course the lawyer
    public AudioListener listener;
    public Toggle toggleSound;
    //Life setting
    public Text _timerLifeTxt;
    public Button _play;
    public Sprite _fullHP;
    public Sprite _lowHP;
    public Image[] _imageLife;
    private int lifes = 3;
    private float timeLife = 0;
    //**End life setting

    public GameObject arrestPanel;
    public GameObject menuPanel;
    public List<Transform> cards;
    private List<Transform> cardsFalse = new List<Transform>();
    public Text _scoreTxt;
    public Image _timer;
    private int _money; // Save
    private int mistake = 0;
    private int mistakeCount = 500; //Upgrade
    private int payMistake = 75; //Upgrade
    private int timerAchive = 0;
    [HideInInspector]
    public int casheBack = 12; //Upgrade
    [HideInInspector]
    public int cashebackCurrent = 1;

    //prices
    private int machinePrice = 15000;
    public Image[] _imageMachine;
    public Text machineTxt;
    public Text machinePriceTxt;
    private int safePrice = 15000;
    public Image[] _imageSafe;
    public Text safeTxt;
    public Text safePriceTxt;
    private int dirtPrice = 15000;
    public Image[] _imageDirt;
    public Text dirtTxt;
    public Text dirtPriceTxt;
    private int luckPrice = 15000;
    public Image[] _imageLuck;
    public Text luckTxt;
    public Text luckPriceTxt;
    //**End prices

    //Google Play Services
    private const string achive0 = "CgkI89vXu_0REAIQAQ"; // начать играть
    private const string achive1 = "CgkI89vXu_0REAIQAg"; // набрать 5 000
    private const string achive2 = "CgkI89vXu_0REAIQAw"; // набрать 10 000
    private const string achive3 = "CgkI89vXu_0REAIQBA"; // набрать 100 000
    private const string achive4 = "CgkI89vXu_0REAIQBQ"; // набрать 500 000
    private const string achive5 = "CgkI89vXu_0REAIQBg"; // набрать 1 000 000
    private const string achive6 = "CgkI89vXu_0REAIQBw"; // провести 15 минут в игре
    private const string achive7 = "CgkI89vXu_0REAIQCA"; // провести час в игре
    private const string achive8 = "CgkI89vXu_0REAIQCQ"; // прокачать одну из способностей
    private const string achive9 = "CgkI89vXu_0REAIQCg"; // прокачать все способности

    private const string _leaderboards = "CgkI89vXu_0REAIQAA"; // const не изменяемая переменая 


    public static void GetAchive(string id)
    {
        Social.ReportProgress(id, 100.0f, (bool successe) => { });
    }
    //**End Google Play Services

    private void Awake()
    {
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().Build();
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.InitializeInstance(config);

        PlayGamesPlatform.Activate();
        //PlayGamesPlatform.Instance.Authenticate((bool successe) => { if (successe) { print("1"); } else { print("2"); } });
        Social.localUser.Authenticate((bool successe) => { });

        //Load Upgrade & progress
        if (PlayerPrefs.HasKey("Sound") == true)
        {
            toggleSound.isOn = (PlayerPrefs.GetInt("Sound") == 0) ? true : false;
        }

        if (PlayerPrefs.HasKey("Money") == true)
        {
            _money = PlayerPrefs.GetInt("Money");
            _scoreTxt.text = "Score: " + _money + "$";
        }

        if (PlayerPrefs.HasKey("MachinePrice") == true)
        {
            machinePrice = PlayerPrefs.GetInt("MachinePrice");
            casheBack -= (Mathf.CeilToInt(machinePrice / 5000) - 3) * 2;
            machineTxt.text = "<b><i>Washing machine:</i></b> \n<color=yellow>Every " + casheBack + " currency will be returned to you.</color>\n";
            machinePriceTxt.text = (machinePrice != 40000) ? machinePrice.ToString("00 000") + " $" : "";

            for (int i = 0; i < _imageMachine.Length; i++)
            {
                _imageMachine[i].color = (i <= Mathf.CeilToInt(machinePrice / 5000) - 4) ? Color.white : Color.gray;
            }
        }

        if (PlayerPrefs.HasKey("SafePrice") == true)
        {
            safePrice = PlayerPrefs.GetInt("SafePrice");
            payMistake -= (Mathf.CeilToInt(safePrice / 5000) - 3) * 10;
            safeTxt.text = "<b><i>Safe:</i></b> \n<color=yellow>You save up to " + (100 - payMistake) + "% of the total amount.</color>\n";
            safePriceTxt.text = (safePrice != 40000) ? safePrice.ToString("00 000") + " $" : "";

            for (int i = 0; i < _imageSafe.Length; i++)
            {
                _imageSafe[i].color = (i <= Mathf.CeilToInt(safePrice / 5000) - 4) ? Color.white : Color.gray;
            }
        }

        if (PlayerPrefs.HasKey("DirtPrice") == true)
        {
            dirtPrice = PlayerPrefs.GetInt("DirtPrice");
            CartController.dirtMinCount += (Mathf.CeilToInt(dirtPrice / 5000) - 3f) / 20f;
            dirtTxt.text = "<b><i>Dirt count:</i></b> \n<color=yellow>Your clear money chance is " + Mathf.FloorToInt(CartController.dirtMinCount * 100) + "%.</color>\n";
            dirtPriceTxt.text = (dirtPrice != 40000) ? dirtPrice.ToString("00 000") + " $" : "";

            for (int i = 0; i < _imageDirt.Length; i++)
            {
                _imageDirt[i].color = (i <= Mathf.CeilToInt(dirtPrice / 5000) - 4) ? Color.white : Color.gray;
            }
        }

        if (PlayerPrefs.HasKey("LuckPrice") == true)
        {
            luckPrice = PlayerPrefs.GetInt("LuckPrice");
            mistakeCount += (Mathf.CeilToInt(luckPrice / 5000) - 3) * 100;
            luckTxt.text = "<b><i>Luck:</i></b> \n<color=yellow>You can collect up to " + mistakeCount + " dirty money.</color>\n";
            luckPriceTxt.text = (luckPrice != 40000) ? luckPrice.ToString("00 000") + " $" : "";

            for (int i = 0; i < _imageLuck.Length; i++)
            {
                _imageLuck[i].color = (i <= Mathf.CeilToInt(luckPrice / 5000) - 4) ? Color.white : Color.gray;
            }
        }
        //**End load Upgrade & progress

        if (PlayerPrefs.HasKey("LastTimes") == true)
        {
            TimeSpan ts = DateTime.Now - DateTime.Parse(PlayerPrefs.GetString("LastTimes"));
            timeLife = PlayerPrefs.GetFloat("timeLifePrefs") - ((ts.Days * 86400) + (ts.Hours * 3600) + (ts.Minutes * 60) + ts.Seconds);
            if (timeLife > 0)
            {
                TimerLifes();

                lifes -= Mathf.CeilToInt(timeLife / 300);
                for (int i = 0; i < 3; i++)
                {
                    _imageLife[i].sprite = (i <= lifes - 1) ? _fullHP : _lowHP;
                }
                _play.interactable = (lifes == 0) ? false : true;
            }
            else
            {
                timeLife = 0;
            }
        }
        else
        {
            _timerLifeTxt.text = "Time: 00:00";
        }

        //Ads
        Advertisement.Initialize("3021529", false);

        Advertisement.Banner.Load("BannerDown");
        Invoke("ShowAd", 0.5f);
        //** End Ads

        timerAchive = PlayerPrefs.GetInt("TimerAchive");
        if (timerAchive < 3600)
        {
            TimerAchive();
        }
        
    }
    //Banner Ads
    public void ShowAd()
    {
        if (Advertisement.Banner.isLoaded)
        {
            Advertisement.Banner.Show("BannerDown");
        }
        else
        {
            Invoke("ShowAd", 1f);
        }
    }

    public void TimerAchive()
    {
        timerAchive++;

        if (timerAchive == 900) GetAchive(achive6);
        if (timerAchive >= 3600) GetAchive(achive7);

        Invoke("TimerAchive", 1f);
    }

#if UNITY_STANDALONE || UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            PlayerPrefs.DeleteAll();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            ScreenCapture.CaptureScreenshot("Screenshot.png", 1);
        }
    }
#endif
    //Video Ads
    public void AdsShow()
    {
        if (Advertisement.isSupported && Advertisement.IsReady("rewardedVideo"))
        {
            Advertisement.Show("rewardedVideo");
            timeLife = (timeLife != 0) ? Mathf.Clamp(timeLife - 300f, 0, 900) : timeLife;
            lifes = (lifes != 3) ? Mathf.Clamp(lifes + 1, 0, 3) : lifes;

            _imageLife[lifes - 1].sprite = _fullHP;
            _play.interactable = true;
            _timerLifeTxt.text = "Time: " + ((timeLife == 0) ? "00:00" : "");
        }
    }

    private void TimerLifes()
    {
        if (timeLife != 0)
        {
            timeLife--;
            int minute = (int)timeLife / 60;
            float second = timeLife - (minute * 60f);
            _timerLifeTxt.text = "Time: " + ((minute < 10) ? "0" + minute : "" + minute) + ":" + ((second < 10) ? "0" + second : "" + second);
            Invoke("TimerLifes", 1f);
            if (timeLife == 0 || timeLife == 300 || timeLife == 600)
            {
                lifes++;
                _imageLife[lifes - 1].sprite = _fullHP;
                _play.interactable = true;
            }
        }
    }

    public void Restart()
    {
        if (lifes > 0)
        {
            GetAchive(achive0);
            _imageLife[lifes - 1].sprite = _lowHP;
            _timer.rectTransform.sizeDelta = new Vector2(720f, 5f);
            Invoke("EndRestard", 0.1f);

            //Life
            lifes--;
            _play.interactable = (lifes == 0) ? false : true;
            timeLife += 300f;
            if (IsInvoking("TimerLifes") == false)
            {
                TimerLifes();
            }
        }
    }

    private void EndRestard()
    {
        menuPanel.SetActive(false);
    }

    private void Timer()
    {
        _timer.rectTransform.sizeDelta = Vector2.MoveTowards(_timer.rectTransform.sizeDelta, new Vector2(0f, 5f), 5f);
        if(_timer.rectTransform.sizeDelta.x == 0)
        {
            CancelInvoke("Timer");
            menuPanel.SetActive (true);
            for (int i = 0; i < 5; i++)
            {
                menuPanel.transform.GetChild(i).gameObject.SetActive(true);
            }
            //Add leaderboards
            Social.ReportScore(_money, _leaderboards, (bool access) => { });
        }
        else
        {
            Invoke("Timer", 0.02f);
        }
    }

    public void NextCart()
    {
        cardsFalse.Add(cards[0]);
        cards.Remove(cards[0]);
        cardsFalse[cardsFalse.Count - 1].SetSiblingIndex(7);
    }

    public void CartReady()
    {
        cards.Add(cardsFalse[0]);
        cardsFalse.Remove(cardsFalse[0]);
    }

    public void Score(int i)
    {
        _money += i;
        _scoreTxt.text = "Score: " + _money + "$";

        CancelInvoke("Timer");
        _timer.rectTransform.sizeDelta = new Vector2(720f, 5f);
        Timer();

        //Get achives
        if (_money >= 5000) GetAchive(achive1);
        if (_money >= 10000) GetAchive(achive2);
        if (_money >= 100000) GetAchive(achive3);
        if (_money >= 500000) GetAchive(achive4);
        if (_money >= 1000000) GetAchive(achive5);
        //**End achive
    }

    public void MistakeScore(int i)
    {
        mistake = (mistake < mistakeCount) ? mistake + i : 0;
        arrestPanel.SetActive((mistake >= mistakeCount) ? true : false);
        menuPanel.SetActive((mistake >= mistakeCount) ? true : false);
    }

    
    public void Bribe()
    {
        _money -= _money / 100 * payMistake;
        _scoreTxt.text = "Score: " + _money + "$";
        arrestPanel.SetActive(false);

        //Add leaderboards
        Social.ReportScore(_money, _leaderboards, (bool access) => { });
    }

    public void Leaderboards()
    {
        Social.ShowLeaderboardUI();
    }

    public void ShowAchive()
    {
        Social.ShowAchievementsUI();
    }

    public void AudioSettings(Toggle toogle)
    {
        int i = (toogle.isOn == false) ? 1 : 0;
        listener.enabled = (i == 0) ? true : false;
        PlayerPrefs.SetInt("Sound", i);
        print(i);
    }

    //Upgrade
    public void WashingMachineUpgrade()
    {
        if (_money >= machinePrice && machinePrice < 40000)
        {
            machinePrice += 5000;
            _money -= machinePrice - 5000;
            casheBack -= 2;
            _scoreTxt.text = "Score: " + _money + "$";
            machineTxt.text = "<b><i>Washing machine:</i></b> \n<color=yellow>Every " + casheBack + " currency will be returned to you.</color>\n";
            machinePriceTxt.text = (machinePrice != 40000) ? machinePrice.ToString("00 000") + " $" : "";

            for (int i = 0; i < _imageMachine.Length; i++)
            {
                _imageMachine[i].color = (i <= Mathf.CeilToInt(machinePrice / 5000) - 4) ? Color.white : Color.gray;
            }

            PlayerPrefs.SetInt("MachinePrice", machinePrice);
            PlayerPrefs.SetInt("Money", _money);

            //Get achives
            if (machinePrice == 20000) GetAchive(achive8);
            else if (machinePrice == 40000 && safePrice == 40000 && dirtPrice == 40000 && luckPrice == 40000) GetAchive(achive9);
            //**End achive
        }
    }

    public void SafeUpgrade()
    {
        if (_money >= safePrice && safePrice < 40000)
        {
            safePrice += 5000;
            _money -= safePrice - 5000;
            payMistake -= 10;
            _scoreTxt.text = "Score: " + _money + "$";
            safeTxt.text = "<b><i>Safe:</i></b> \n<color=yellow>You save up to " + (100 - payMistake) + "% of the total amount.</color>\n";
            safePriceTxt.text = (safePrice != 40000) ? safePrice.ToString("00 000") + " $" : "";

            for (int i = 0; i < _imageSafe.Length; i++)
            {
                _imageSafe[i].color = (i <= Mathf.CeilToInt(safePrice / 5000) - 4) ? Color.white : Color.gray;
            }

            PlayerPrefs.SetInt("SafePrice", safePrice);
            PlayerPrefs.SetInt("Money", _money);

            //Get achives
            if (safePrice == 20000) GetAchive(achive8);
            else if (machinePrice == 40000 && safePrice == 40000 && dirtPrice == 40000 && luckPrice == 40000) GetAchive(achive9);
            //**End achive
        }
    }

    public void DirtUpgrade()
    {
        if (_money >= dirtPrice && dirtPrice < 40000)
        {
            dirtPrice += 5000;
            _money -= dirtPrice - 5000;
            CartController.dirtMinCount += 0.05f;
            _scoreTxt.text = "Score: " + _money + "$";
            dirtTxt.text = "<b><i>Dirt count:</i></b> \n<color=yellow>Your clear money chance is " + Mathf.FloorToInt(CartController.dirtMinCount * 100) + "%.</color>\n";
            dirtPriceTxt.text = (dirtPrice != 40000) ? dirtPrice.ToString("00 000") + " $" : "";

            for (int i = 0; i < _imageDirt.Length; i++)
            {
                _imageDirt[i].color = (i <= Mathf.CeilToInt(dirtPrice / 5000) - 4) ? Color.white : Color.gray;
            }

            PlayerPrefs.SetInt("DirtPrice", dirtPrice);
            PlayerPrefs.SetInt("Money", _money);

            //Get achives
            if (dirtPrice == 20000) GetAchive(achive8);
            else if (machinePrice == 40000 && safePrice == 40000 && dirtPrice == 40000 && luckPrice == 40000) GetAchive(achive9);
            //**End achive
        }
    }

    public void LuckUpgrade()
    {
        if (_money >= luckPrice && luckPrice < 40000)
        {
            luckPrice += 5000;
            _money -= luckPrice - 5000;
            mistakeCount += 100;
            _scoreTxt.text = "Score: " + _money + "$";
            luckTxt.text = "<b><i>Luck:</i></b> \n<color=yellow>You can collect up to " + mistakeCount + " dirty money.</color>\n";
            luckPriceTxt.text = (luckPrice != 40000) ? luckPrice.ToString("00 000") + " $" : "";

            for (int i = 0; i < _imageLuck.Length; i++)
            {
                _imageLuck[i].color = (i <= Mathf.CeilToInt(luckPrice / 5000) - 4) ? Color.white : Color.gray;
            }

            PlayerPrefs.SetInt("LuckPrice", luckPrice);
            PlayerPrefs.SetInt("Money", _money);

            //Get achives
            if (luckPrice == 20000) GetAchive(achive8);
            else if (machinePrice == 40000 && safePrice == 40000 && dirtPrice == 40000 && luckPrice == 40000) GetAchive(achive9);
            //**End achive
        }
    }
    //**End upgrade

    //Exite app
    private void OnApplicationFocus(bool focus)
    {
        if (arrestPanel.activeSelf == true)
        {
            Bribe();
        }

        PlayerPrefs.SetString("LastTimes", DateTime.Now.ToString());
        PlayerPrefs.SetFloat("timeLifePrefs", timeLife);
        PlayerPrefs.SetInt("TimerAchive", timerAchive);
        PlayerPrefs.SetInt("Money", _money);
    }

    private void OnApplicationPause(bool pause)
    {
        if (arrestPanel.activeSelf == true)
        {
            Bribe();
        }

        PlayerPrefs.SetString("LastTimes", DateTime.Now.ToString());
        PlayerPrefs.SetFloat("timeLifePrefs", timeLife);
        PlayerPrefs.SetInt("TimerAchive", timerAchive);
        PlayerPrefs.SetInt("Money", _money);
    }

    private void OnApplicationQuit()
    {
        if (arrestPanel.activeSelf == true && _lawerButton.interactable == false)
        {
            Bribe();
        }

        PlayerPrefs.SetString("LastTimes", DateTime.Now.ToString());
        PlayerPrefs.SetFloat("timeLifePrefs", timeLife);
        PlayerPrefs.SetInt("TimerAchive", timerAchive);
        PlayerPrefs.SetInt("Money", _money);
    }
    //**End Exite app
}
