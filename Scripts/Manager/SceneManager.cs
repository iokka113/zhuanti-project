using ZhuanTiNanMin.Singleton;
using System.Collections;
using UnityEngine;

public class SceneManager : MonoBehaviour, IManager
{
    public static SceneManager Instance => Singleton<SceneManager>.Instance;

    public bool InitHasFinished { get; private set; }

    private void Awake()
    {
        Singleton<SceneManager>.Instance = this;
        DontDestroyOnLoad(Instance.gameObject);
        InitHasFinished = true;
    }

    public void LoadScene(SceneType type)
    {
        switch (type)
        {
            case SceneType.Home:
                StartCoroutine(LoadSceneAsync("Home", false, () =>
                {
                    UIManager.Instance.OnSceneHomeLoaded();
                }));
                break;
            case SceneType.Character:
                StartCoroutine(LoadSceneAsync("Character", false, () =>
                {
                    UIManager.Instance.OnSceneCharacterLoaded();
                    InventoryManager.Instance.OnSceneCharacterLoaded();
                }));
                break;
            case SceneType.LevelMap:
                StartCoroutine(LoadSceneAsync(LevelManager.Instance.NextLevelMapName, true, () =>
                {
                    UIManager.Instance.OnSceneLevelMapLoaded();
                    InventoryManager.Instance.OnSceneLevelMapLoaded();
                    LevelManager.Instance.OnSceneLevelMapLoaded();
                    PrefabsManager.Instance.OnSceneLevelMapLoaded();
                }));
                break;
            case SceneType.Win:
                StartCoroutine(LoadSceneAsync("Win", false, () =>
                {
                    return;
                }));
                break;
            default:
                return;
        }
    }

    private IEnumerator LoadSceneAsync(string sceneName, bool displayLoadingUI, System.Action onSceneLoaded)
    {
        yield return new WaitForEndOfFrame();
        if (displayLoadingUI)
        {
            UIManager.Instance.DisplayLoadingUI(true, "Scene Loading...");
            yield return new WaitForSecondsRealtime(0.25f);
        }
        AsyncOperation asyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
        asyncOperation.allowSceneActivation = true;
        while (!asyncOperation.isDone)
        {
            if (displayLoadingUI)
            {
                UIManager.Instance.DisplayLoadingUI(true, $"Loading progress: {asyncOperation.progress * 100f:0.00}%");
            }
            yield return null;
        }
        if (displayLoadingUI)
        {
            UIManager.Instance.DisplayLoadingUI(true, "Loading finish.");
            yield return new WaitForSecondsRealtime(0.25f);
        }
        UIManager.Instance.DisplayLoadingUI(false);
        onSceneLoaded?.Invoke();
    }
}

public enum SceneType
{
    Home,
    Character,
    LevelMap,
    Win,
}
