using ZhuanTiNanMin.Singleton;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class UIManager : MonoBehaviour, IManager
{
    public static UIManager Instance => Singleton<UIManager>.Instance;

    public bool InitHasFinished { get; private set; }

    private void Awake()
    {
        Singleton<UIManager>.Instance = this;
        DontDestroyOnLoad(Instance.gameObject);
        InitHasFinished = true;
    }

    [SerializeField]
    private GameObject _loadingUI = null;
    [SerializeField]
    private Text _loadingText = null;

    public void DisplayLoadingUI(bool isVisible, string uiText = null)
    {
        _loadingUI.SetActive(isVisible);
        if (uiText != null) { _loadingText.text = uiText; }
    }

    private bool _buttonLock =  true;

    public void OnButtonClick(UIButtonType type)
    {
        if (!_buttonLock)
        {
            if (type == UIButtonType.ExitTheApp)
            {
                _buttonLock = true;
                Application.Quit();
            }
            else if (type == UIButtonType.StartNewGame)
            {
                _buttonLock = true;
                SceneManager.Instance.LoadScene(SceneType.Character);
            }
            else if (type == UIButtonType.NextCharacter)
            {
                InventoryManager.Instance.ChangePlayerCharacter();
            }
            else if (type == UIButtonType.PreviousCharacter)
            {
                InventoryManager.Instance.ChangePlayerCharacter(true);
            }
            else if (type == UIButtonType.CharacterSelected)
            {
                _buttonLock = true;
                LevelManager.Instance.IntoNewLevel();
            }
            else if (type == UIButtonType.BackToHome)
            {
                _buttonLock = true;
                SceneManager.Instance.LoadScene(SceneType.Home);
            }
            else if (type == UIButtonType.AboutUsOpen)
            {
                FindObjectOfType<AboutUI>().ActivateUI(true);
            }
            else if (type == UIButtonType.AboutUsClose)
            {
                FindObjectOfType<AboutUI>().ActivateUI(false);
            }
        }
    }

    public void OnSceneHomeLoaded()
    {
        _buttonLock = false;
    }

    public void OnSceneCharacterLoaded()
    {
        _charImg = GameObject.FindGameObjectWithTag("CharImg").GetComponent<Image>();
        _charTxt = GameObject.FindGameObjectWithTag("CharTxt").GetComponent<Text>();
        _buttonLock = false;
    }

    private Image _charImg;
    private Text _charTxt;

    public void UpdateDataInSceneCharacter(PlayerCharacterData data)
    {
        _charImg.sprite = data.ProfileImage;
        _charTxt.text = data.ProfileBio;
    }

    [SerializeField]
    private GameObject _mainUIPrefab = null;

    public MainUI MainUI { get; private set; }

    public void OnSceneLevelMapLoaded()
    {
        MainUI = Instantiate(_mainUIPrefab).GetComponent<MainUI>();
        _buttonLock = false;
    }
}

public enum UIButtonType
{
    ExitTheApp = 0,
    ExitCheckingOpen = 1,
    ExitCheckingClose = 2,
    StartNewGame = 3,
    PreviousCharacter = 8,
    NextCharacter = 9,
    CharacterSelected = 7,
    BackToHome = 6,
    SettingOpen = 4,
    SettingClose = 5,
    AboutUsOpen = 10,
    AboutUsClose = 11,
}
