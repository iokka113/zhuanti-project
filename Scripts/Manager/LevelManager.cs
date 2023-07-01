using ZhuanTiNanMin.Singleton;
using ZhuanTiNanMin.Mathematics;
using UnityEngine;

public class LevelManager : MonoBehaviour, IManager
{
    public static LevelManager Instance => Singleton<LevelManager>.Instance;

    public bool InitHasFinished { get; private set; }

    private int[] _levelOrder;
    private int _levelIndex;

    private void Awake()
    {
        Singleton<LevelManager>.Instance = this;
        DontDestroyOnLoad(Instance.gameObject);
        InitHasFinished = true;
    }

    public string NextLevelMapName => "LevelMap" + _levelOrder[_levelIndex].ToString();

    public void IntoNewLevel()
    {
        //_levelOrder = NonRepeating.RandomList(1, 5, 3).ToArray();
        _levelOrder = new int[] { 1, 2 };
        _levelIndex = -1;
        _gameForFirstTime = true;
        IntoNextLevel();
    }

    public void IntoNextLevel()
    {
        _levelIndex++;
        if (_levelIndex >= _levelOrder.Length)
        {
            SceneManager.Instance.LoadScene(SceneType.Win);
        }
        else
        {
            SceneManager.Instance.LoadScene(SceneType.LevelMap);
        }
    }

    [SerializeField]
    private GameObject _mainCamera = null;
    [SerializeField]
    private GameObject _directionPointer = null;

    private bool _gameForFirstTime;

    public void OnSceneLevelMapLoaded()
    {
        Instantiate(_mainCamera);
        Instantiate(_directionPointer);
        GameObject.Find("map").transform.localScale = new Vector3(2f, 2f, 1f);
        if (_gameForFirstTime)
        {
            UIManager.Instance.MainUI.TutorialTextUpdate("----------------------------------------------------");
            UIManager.Instance.MainUI.TutorialTextUpdate("通過探索這個地城來尋找出口");
            UIManager.Instance.MainUI.TutorialTextUpdate("路途中棲息於地城中的生物將會成為阻礙");
            UIManager.Instance.MainUI.TutorialTextUpdate("保持謹慎才能逃離地城");
            UIManager.Instance.MainUI.TutorialTextUpdate("----------------------------------------------------");
            _gameForFirstTime = false;
        }
        UIManager.Instance.MainUI.TutorialTextUpdate(Funclib.AddColorToString("點撃WASD或方向鍵控制角色移動…", Color.blue));
    }
}
