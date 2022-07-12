using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
//using UserDataManagement;

public class mainMenuScript : MonoBehaviour
{
    public GameObject skinSelectorParent;
    public AudioMixer MasterMixer;
    public Slider MasterVolSlider;
    public Slider MusicVolSlider;
    public Slider SFXVolSlider;
    public TMP_Dropdown FpsLockDropdown;
    //public InterstitialAdExample ads = new InterstitialAdExample();
    public TMP_Text VersionNum;
    private const string RegisterUrl = "https://www.schnapsen66.eu/index.php?r=site%2Fsignup";
    

    [Header("UI panels")]
    public GameObject MainMenuUI;
    public GameObject SettingsUI;

    public void startGame()
    {
        Debug.Log("Starting game");
        SceneManager.LoadScene("Game", LoadSceneMode.Single);
    }

    public void odpriRegistraciojo()
    {
        Application.OpenURL(RegisterUrl);
    }

    public GameObject OnlineWebWarning;
    public void startOnlineGame()
    {
        if(Application.platform == RuntimePlatform.WebGLPlayer)
        {
            OnlineWebWarning.SetActive(true);
            return;
        }
        SceneManager.LoadScene("LobbyScene", LoadSceneMode.Single);
    }

    public void showAD()
    {
        //ads.ShowAd();
    }
    private void Start()
    {
        /*if (UserData.instance.isLoggedIn)
        {
            OnLoadingLogIn();
            //OnSuccessfulLogIn();
            UserData.instance.RefreshUserData();
        }
        else
            OnUnsuccessfulLogIn("");
*/
        //ads.InitServices();
        
        //LoginErrorMSG.text = "";
        Input.multiTouchEnabled = false;

        if (!PlayerPrefs.HasKey("izbranSkinKart"))
        {
            PlayerPrefs.SetString("izbranSkinKart", "Skin01");
        }
        if (!PlayerPrefs.HasKey("MasterVol"))
        {
            PlayerPrefs.SetFloat("MasterVol", 1f);
        } else
            MasterVolSlider.value = PlayerPrefs.GetFloat("MasterVol");
        if (!PlayerPrefs.HasKey("MusicVol"))
        {
            PlayerPrefs.SetFloat("MusicVol", 1f);
        }
        else
            MusicVolSlider.value = PlayerPrefs.GetFloat("MusicVol");
        if (!PlayerPrefs.HasKey("SFXVol"))
        {
            PlayerPrefs.SetFloat("SFXVol", 1f);
        }
        else
            SFXVolSlider.value = PlayerPrefs.GetFloat("SFXVol");

        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            GameObject.Find("ExitBTN").SetActive(false);
        }
        if (!PlayerPrefs.HasKey("FPSLock"))
        {
            PlayerPrefs.SetInt("FPSLock", 1);
            nastaviFPSLock(1);
        }
        else
            nastaviFPSLock(PlayerPrefs.GetInt("FPSLock"));
    }

    public void zapriNastavitve()
    {
        Animator animator = SettingsUI.GetComponent<Animator>();
        //SettingsUI.SetActive(false);
        animator.SetBool("open", false);
        animator = MainMenuUI.GetComponent<Animator>();
        animator.SetBool("closed", false);
        //MainMenuUI.SetActive(true);
    }

    public void odpriNastavitve()
    {
        
        Animator animator = SettingsUI.GetComponent<Animator>();
        animator.SetBool("open", true);
        animator = MainMenuUI.GetComponent<Animator>();
        animator.SetBool("closed", true);
        //SettingsUI.SetActive(true);
        //MainMenuUI.SetActive(false);


        //izberi pravo vrednost v skin selectorju
        GameObject.Find(PlayerPrefs.GetString("izbranSkinKart")).GetComponent<Toggle>().isOn = true;
        MasterVolSlider.value = PlayerPrefs.GetFloat("MasterVol");
        MusicVolSlider.value = PlayerPrefs.GetFloat("MusicVol");
        SFXVolSlider.value = PlayerPrefs.GetFloat("SFXVol");
        FpsLockDropdown.value = PlayerPrefs.GetInt("FPSLock");
        VersionNum.text = "Version: " + Application.version;
    }
    public void spremeniIzbranSkin(string imeSkina)
    {
        PlayerPrefs.SetString("izbranSkinKart", imeSkina);
        Debug.Log(imeSkina);
    }

    public void spremeniMasterVol(float vol)
    {
        Debug.Log(vol);
        MasterMixer.SetFloat("MasterVol", Mathf.Log(vol) * 20);
        PlayerPrefs.SetFloat("MasterVol", vol);
    }
    public void spremeniMusicVol(float vol)
    {
        MasterMixer.SetFloat("MusicVol", Mathf.Log(vol) * 20);
        PlayerPrefs.SetFloat("MusicVol", vol);
    }
    public void spremeniSFXVol(float vol)
    {
        MasterMixer.SetFloat("SFXVol", Mathf.Log(vol) * 20);
        PlayerPrefs.SetFloat("SFXVol", vol);
    }

    public void nastaviFPSLock(int fps)
    {
        PlayerPrefs.SetInt("FPSLock", fps);
        Debug.Log(fps);
        switch (fps)
        {
            case 0:
                Application.targetFrameRate = 30;
                break;
            case 1:
                Application.targetFrameRate = 60;
                break;
            case 2:
                Application.targetFrameRate = 120;
                break;
        }
    }

    [Header("Login Prompt")]
    public GameObject LoginWindow;
    public InputField username;
    public InputField password;
    public TMP_Text LoginErrorMSG;
    public GameObject UserInfoLoggedIn;
    public GameObject UserInfoLoading;
    public GameObject UserInfoSignedOut;
    public GameObject UserInfoNoInternet;

    public RawImage Pfpic;
    public TMP_Text usernameDisplay;
    public TMP_Text ELODisplay;
    public TMP_Text WinsDisplay;
    public TMP_Text LossesDisplay;


    public void login()
    {
        /*if (username.text == "" || password.text == "") return;
        ServerLogin.instance.Login(username.text, password.text, true);
        UserInfoLoading.SetActive(true);
        UserInfoSignedOut.SetActive(false);
        UserInfoNoInternet.SetActive(false);*/
    }
    public void OnSuccessfulLogIn()
    {
        UpdateUserInfoUI();
        LoginWindow.SetActive(false);
        UserInfoLoading.SetActive(false);
        UserInfoSignedOut.SetActive(false);
        UserInfoLoggedIn.SetActive(true);
        UserInfoNoInternet.SetActive(false);
    }

    public void OnLoadingLogIn()
    {
        LoginWindow.SetActive(false);
        UserInfoLoading.SetActive(true);
        UserInfoSignedOut.SetActive(false);
        UserInfoLoggedIn.SetActive(false);
        UserInfoNoInternet.SetActive(false);
    }

    public void OnUnsuccessfulLogIn(string error)
    {
        LoginErrorMSG.text = error;
        UserInfoLoading.SetActive(false);
        UserInfoSignedOut.SetActive(true);
        UserInfoLoggedIn.SetActive(false);
        UserInfoNoInternet.SetActive(false);
    }

    public void OnNoNetworkConnection()
    {
        UserInfoLoading.SetActive(false);
        UserInfoSignedOut.SetActive(false);
        UserInfoLoggedIn.SetActive(false);
        UserInfoNoInternet.SetActive(true);
    }

    public void OnNetworkConnection()
    {
        /*UserInfoNoInternet.SetActive(false);
        if (UserData.instance.isLoggedIn)
        {
            OnSuccessfulLogIn();
            UserData.instance.RefreshUserData();
        } else
        {
            OnUnsuccessfulLogIn("");
        }*/
    }

    public void OnLogOut()
    {
        UserInfoLoading.SetActive(false);
        UserInfoSignedOut.SetActive(true);
        UserInfoLoggedIn.SetActive(false);
        UserInfoNoInternet.SetActive(false);
    }

    public void UpdateUserInfoUI()
    {
        /*if(UserData.instance.ProfilePic!=null)
            Pfpic.texture = UserData.instance.ProfilePic;
        usernameDisplay.text = UserData.instance.Username;
        ELODisplay.text = "ELO: "+UserData.instance.Elo;
        WinsDisplay.text = "Wins: "+UserData.instance.Wins;
        LossesDisplay.text = "Losses: "+UserData.instance.Losses;*/
    }

    public void zapriLoginPrompt()
    {
        LoginWindow.SetActive(false);
    }
    public void odpriLoginPrompt()
    {
        LoginWindow.SetActive(true);
    }

    public void LogOut()
    {
        //ServerLogin.instance.LogOut();
    }

    public void closeGame()
    {
        Application.Quit();
    }
}
