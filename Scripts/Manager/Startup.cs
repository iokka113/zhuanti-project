using System.Collections;
using UnityEngine;

public class Startup : MonoBehaviour
{
    private static readonly object _lock = new object();
    private static bool _finished = false;

    private void Awake()
    {
        if (!_finished)
        {
            lock (_lock)
            {
                if (!_finished)
                {
                    _finished = true;
                    StartCoroutine(Drive());
                }
            }
        }
    }

    [SerializeField]
    private GameObject _sceneManager = null;
    [SerializeField]
    private GameObject _uiManager = null;
    [SerializeField]
    private GameObject _levelManager = null;
    [SerializeField]
    private GameObject _inventoryManager = null;
    [SerializeField]
    private GameObject _prefabsManager = null;

    private IEnumerator Drive()
    {
        Instantiate(_sceneManager);
        Instantiate(_uiManager);
        Instantiate(_levelManager);
        Instantiate(_inventoryManager);
        Instantiate(_prefabsManager);
        yield return null;

        yield return new WaitUntil(() => SceneManager.Instance.InitHasFinished);
        yield return new WaitUntil(() => UIManager.Instance.InitHasFinished);
        yield return new WaitUntil(() => LevelManager.Instance.InitHasFinished);
        yield return new WaitUntil(() => InventoryManager.Instance.InitHasFinished);
        yield return new WaitUntil(() => PrefabsManager.Instance.InitHasFinished);
        yield return null;

        SceneManager.Instance.LoadScene(SceneType.Home);
    }
}

public interface IManager
{
    bool InitHasFinished { get; }
}
