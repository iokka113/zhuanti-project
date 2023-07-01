using ZhuanTiNanMin.Singleton;
using ZhuanTiNanMin.Mathematics;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour, IManager
{
    public static InventoryManager Instance => Singleton<InventoryManager>.Instance;

    public bool InitHasFinished { get; private set; }

    private void Awake()
    {
        Singleton<InventoryManager>.Instance = this;
        DontDestroyOnLoad(Instance.gameObject);
        InitHasFinished = true;
    }

    [SerializeField]
    private List<PlayerCharacterData> _playerCharacterDatas = null;
    private int _playerCharacterIndex;

    public PlayerCharacterData PlayerCharacter { get; private set; }

    private void SetPlayerCharacter()
    {
        PlayerCharacter = _playerCharacterDatas[_playerCharacterIndex];
        UIManager.Instance.UpdateDataInSceneCharacter(PlayerCharacter);
        _pointData = new PointData(PlayerCharacter.OriginalHp, PlayerCharacter.OriginalMp);
        _potionData = new PotionData() { PotionCountHP = 10 };
    }

    public void ChangePlayerCharacter(bool reverse = false)
    {
        int value = _playerCharacterIndex;
        if (!reverse) { value++; } else { value--; }
        int max = _playerCharacterDatas.Count - 1;
        if (value > max) { _playerCharacterIndex = 0; }
        else if (value < 0) { _playerCharacterIndex = max; }
        else { _playerCharacterIndex = value; }
        SetPlayerCharacter();
    }

    public void OnSceneCharacterLoaded()
    {
        SetPlayerCharacter();
    }

    private PointData _pointData;

    public class PointData
    {
        public float HpMax { get => _hpMax; set => _hpMax = Mathf.Clamp(value, 1f, float.MaxValue); }
        private float _hpMax = 1f;
        public float Hp { get => _hp; set => _hp = Mathf.Clamp(value, 0f, _hpMax); }
        private float _hp = 1f;

        public float MpMax { get => _mpMax; set => _mpMax = Mathf.Clamp(value, 1f, float.MaxValue); }
        private float _mpMax = 1f;
        public float Mp { get => _mp; set => _mp = Mathf.Clamp(value, 0f, _mpMax); }
        private float _mp = 1f;

        public PointData(float hp, float mp)
        {
            Hp = HpMax = hp;
            Mp = MpMax = mp;
        }
    }

    private PotionData _potionData;

    public class PotionData
    {
        public int PotionCountHP { get; set; }
        public int PotionCountMP { get; set; }
    }

    public void OnSceneLevelMapLoaded()
    {
        UIManager.Instance.MainUI.DisplayProfile(PlayerCharacter.ProfileImageHead);
        UIManager.Instance.MainUI.DisplayPoint(_pointData);
        UIManager.Instance.MainUI.DisplayPotion(_potionData);
        Transform tr = GameObject.FindGameObjectWithTag("SpawnPoint").GetComponent<Transform>();
        Instantiate(PlayerCharacter.CharacterPrefab, tr.position, Quaternion.identity);
    }

    public void PlayerDamageHP(float value)
    {
        _pointData.Hp -= value;
        UIManager.Instance.MainUI.DisplayPoint(_pointData);
        if (_pointData.Hp > 0f && _pointData.Hp <= _pointData.HpMax * 0.5f) { UIManager.Instance.MainUI.TutorialTextUpdate(Funclib.AddColorToString("好難受…點撃滑鼠中鍵使用恢復藥水…", Color.red)); }
        if (_pointData.Hp == 0f) { PlayerCtrl.Instance.Die(); UIManager.Instance.MainUI.TutorialTextUpdate("死掉了……點撃ESC鍵回到首頁…"); }
    }

    public void PlayerHealingHP()
    {
        if (!PlayerCtrl.Instance.IsDead)
        {
            if (_potionData.PotionCountHP > 0)
            {
                if (_pointData.Hp < _pointData.HpMax * 1.0f)
                {
                    PlayerDamageHP(-20f);
                    _potionData.PotionCountHP--;
                    UIManager.Instance.MainUI.DisplayPotion(_potionData);
                    UIManager.Instance.MainUI.PlaySound(MainUI.SoundType.DrinkPotion);
                }
                else
                {
                    UIManager.Instance.MainUI.TutorialTextUpdate(Funclib.AddColorToString("目前不需要使用恢復藥水。", Color.green));
                }
            }
            else
            {
                UIManager.Instance.MainUI.TutorialTextUpdate(Funclib.AddColorToString("沒有恢復藥水了…", Color.yellow));
            }
        }
    }

    public void PlayerDamageMP(float value)
    {
        _pointData.Mp -= value;
        UIManager.Instance.MainUI.DisplayPoint(_pointData);
    }

    public void PlayerHealingMP(float value)
    {
        if (!PlayerCtrl.Instance.IsDead)
        {
            _pointData.Mp += value;
            UIManager.Instance.MainUI.DisplayPoint(_pointData);
        }
    }

    public float GetCurrentMP()
    {
        return _pointData.Mp;
    }

    public void PotionHpGET(int count)
    {
        _potionData.PotionCountHP += count;
        UIManager.Instance.MainUI.DisplayPotion(_potionData);
        UIManager.Instance.MainUI.PlaySound(MainUI.SoundType.PickDrop);
        UIManager.Instance.MainUI.TutorialTextUpdate(Funclib.AddColorToString($"獲得{count}瓶恢復藥水。", Color.green));
    }

    private void Update()
    {
        DebugModeCheck();
    }

    private bool _debugSwitch;

    private void DebugModeCheck()
    {
        if (Input.GetKey(KeyCode.Tab) && Input.GetKey(KeyCode.CapsLock) && Input.GetKeyUp(KeyCode.Return))
        {
            _debugSwitch = !_debugSwitch;
            if (UIManager.Instance.MainUI)
            {
                if (_debugSwitch) { UIManager.Instance.MainUI.TutorialTextUpdate(Funclib.AddColorToString("Switch To Debug Mode", Color.blue)); }
                else { UIManager.Instance.MainUI.TutorialTextUpdate(Funclib.AddColorToString("Switch To Player Mode", Color.blue)); }
            }
        }

        if (PlayerCtrl.Instance && !PlayerCtrl.Instance.IsDead)
        {
            if (_debugSwitch && _pointData.Hp <= _pointData.HpMax * 0.3f)
            {
                _pointData.Hp = _pointData.HpMax;
            }
            if (_debugSwitch && Input.GetKeyDown(KeyCode.Backspace))
            {
                PlayerCtrl.Instance.transform.position = GameObject.Find("Portal").transform.position;
            }
        }
    }
}
