using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using ZhuanTiNanMin.Mathematics;

public class MainUI : MonoBehaviour
{
    [SerializeField]
    private Image _profile = null;

    public void DisplayProfile(Sprite profileImageHead)
    {
        _profile.sprite = profileImageHead;
    }

    //[SerializeField]
    //private Image _hpBack = null;
    [SerializeField]
    private Image _mpBack = null;

    [SerializeField]
    private Image _hpFill = null;
    [SerializeField]
    private Image _mpFill = null;

    public void DisplayPoint(InventoryManager.PointData point)
    {
        _hpFill.fillAmount = point.Hp / point.HpMax;
        _mpFill.fillAmount = point.Mp / point.MpMax;
    }

    [SerializeField]
    private Text _hpPotionText = null;
    [SerializeField]
    private Text _mpPotionText = null;

    public void DisplayPotion(InventoryManager.PotionData potion)
    {
        _hpPotionText.text = potion.PotionCountHP.ToString();
        _mpPotionText.text = potion.PotionCountMP.ToString();
    }

    [System.Serializable]
    public class Sound
    {
        public AudioClip AttackSword;
        public AudioClip AttackBow;
        public AudioClip AttackMagic;
        public AudioClip AttackRare;
        public AudioClip PickDrop;
        public AudioClip DrinkPotion;
        public AudioClip DoorLock;
        public AudioClip DoorOpen;
    }

    [SerializeField]
    private Sound _sound = null;
    [SerializeField]
    private AudioSource _audioPlayer = null;

    public enum SoundType
    {
        AttackSword,
        AttackBow,
        AttackMagic,
        AttackRare,
        PickDrop,
        DrinkPotion,
        DoorLock,
        DoorOpen,
    }

    public void PlaySound(SoundType type)
    {
        switch (type)
        {
            case SoundType.AttackSword:
                _audioPlayer.PlayOneShot(_sound.AttackSword);
                break;
            case SoundType.AttackBow:
                _audioPlayer.PlayOneShot(_sound.AttackBow);
                break;
            case SoundType.AttackMagic:
                _audioPlayer.PlayOneShot(_sound.AttackMagic);
                break;
            case SoundType.AttackRare:
                _audioPlayer.PlayOneShot(_sound.AttackRare);
                break;
            case SoundType.PickDrop:
                _audioPlayer.PlayOneShot(_sound.PickDrop);
                break;
            case SoundType.DrinkPotion:
                _audioPlayer.PlayOneShot(_sound.DrinkPotion);
                break;
            case SoundType.DoorLock:
                _audioPlayer.PlayOneShot(_sound.DoorLock);
                break;
            case SoundType.DoorOpen:
                _audioPlayer.PlayOneShot(_sound.DoorOpen);
                break;
            default:
                return;
        }
    }

    [SerializeField]
    private AudioSource _runningSound = null;

    public void PlayRunningSound(bool openSound)
    {
        if (openSound) { _runningSound.mute = false; }
        else { _runningSound.mute = true; }
    }

    [SerializeField]
    private List<Text> _tutorialText = null;

    public void TutorialTextUpdate(string text)
    {
        for (int index = _tutorialText.Count - 1; index > 0; index--)
        {
            _tutorialText[index].text = _tutorialText[index - 1].text;
        }
        _tutorialText[0].text = text;
        //_tutorialText[9].text = _tutorialText[8].text;
        //_tutorialText[8].text = _tutorialText[7].text;
        //_tutorialText[7].text = _tutorialText[6].text;
        //_tutorialText[6].text = _tutorialText[5].text;
        //_tutorialText[5].text = _tutorialText[4].text;
        //_tutorialText[4].text = _tutorialText[3].text;
        //_tutorialText[3].text = _tutorialText[2].text;
        //_tutorialText[2].text = _tutorialText[1].text;
        //_tutorialText[1].text = _tutorialText[0].text;
        //_tutorialText[0].text = text;
    }

    [SerializeField]
    private Image _torchMaskImage = null;

    [SerializeField]
    private Transform _torchMaskTrans = null;
    [SerializeField]
    private Transform _torchNoiseTrans = null;

    private void Update()
    {
        TorchVFX.UPDATE(_torchMaskTrans, _torchNoiseTrans, _torchMaskImage);
        DisplaySmoothPoint();
        CheckGiveUpButton();
    }

    [SerializeField]
    private Image _hpSmoothFill = null;
    [SerializeField]
    private Image _mpSmoothFill = null;

    private void DisplaySmoothPoint()
    {
        _hpSmoothFill.fillAmount = SmoothPoint(_hpFill.fillAmount, _hpSmoothFill.fillAmount);
        _mpSmoothFill.fillAmount = SmoothPoint(_mpFill.fillAmount, _mpSmoothFill.fillAmount);
    }

    private float SmoothPoint(float value, float valueSmooth)
    {
        if (valueSmooth < value) { valueSmooth = value; }
        if (valueSmooth > value) { valueSmooth -= Time.deltaTime * 0.1f; }
        return valueSmooth;
    }

    public void HideMpSlider()
    {
        _mpBack.color = Color.clear;
        _mpFill.color = Color.clear;
        _mpSmoothFill.color = Color.clear;
    }

    private bool _giveUpConfirm;
    private float _giveUpConfirmCD;

    private void CheckGiveUpButton()
    {
        if (Time.time > _giveUpConfirmCD)
        {
            _giveUpConfirm = false;
        }
        if (!_giveUpConfirm && Input.GetKeyDown(KeyCode.RightControl))
        {
            _giveUpConfirm = true;
            _giveUpConfirmCD = Time.time + 2f;
            UIManager.Instance.MainUI.TutorialTextUpdate(Funclib.AddColorToString("再次點撃確認退出遊戲…", Color.red));
            return;
        }
        if (_giveUpConfirm && Input.GetKeyDown(KeyCode.RightControl))
        {
            UIManager.Instance.OnButtonClick(UIButtonType.BackToHome);
        }
    }

    //public int PoisoningCount { get; set; }

    //[SerializeField]
    //private Image[] _arms;
    //[SerializeField]
    //private Image[] _clothes;

    //private void Start()
    //{
    //    //GetPlayerClothes();
    //}

    //private void Update()
    //{
    //    if (Time.time > _computeNextTime)
    //    {
    //        _computeNextTime = Time.time + 1f;
    //        ComputeFixed();
    //    }
    //    SkillCD();
    //}

    //private float _computeNextTime;

    ///// <summary>
    ///// 每秒計算
    ///// </summary>
    //private void ComputeFixed()
    //{
    //    if (PoisoningCount > 0)
    //    {
    //        DamageHP(_hpMax / 100f);
    //        PoisoningCount--;
    //    }
    //}

    //[SerializeField]
    //private GameObject _potionHp;
    //[SerializeField]
    //private Text _potionHpText;
    //private int _potionHpNum;

    //private bool _skill_0_has;
    //private bool _skill_1_has;

    //private bool _skill_0_cd;
    //private bool _skill_1_cd;

    //[SerializeField]
    //private GameObject _skill_0_obj;
    //[SerializeField]
    //private Image _skill_0_fill;

    //[SerializeField]
    //private GameObject _skill_1_obj;
    //[SerializeField]
    //private Image _skill_1_fill;

    //public void Receive(string type)
    //{
    //    if (type == "PotionHP")
    //    {
    //        _potionHpNum++;
    //        Check();
    //    }
    //    if (type == "Skill_0" && !_skill_0_has)
    //    {
    //        _skill_0_has = true;
    //        _skill_0_obj.SetActive(true);
    //    }
    //    if (type == "Skill_1" && !_skill_1_has)
    //    {
    //        _skill_1_has = true;
    //        _skill_1_obj.SetActive(true);
    //    }
    //}

    //private float _cdPotionHP;

    //public void Use(string type)
    //{
    //    if (type == "PotionHP")
    //    {
    //        if (_potionHpNum > 0 && Time.time > _cdPotionHP)
    //        {
    //            _cdPotionHP = Time.time + 3f;
    //            _hp += 20f;
    //            _potionHpNum--;
    //            Check();
    //        }
    //    }
    //    if (type == "Skill_0" && _skill_0_has && _mp > 0f && !_skill_0_cd)
    //    {
    //        _skill_0_cd = true;
    //        _skill_0_fill.fillAmount = 1f;
    //        _mp -= 10f;
    //        PlayerCtrl.Instance.SkillGo(0);
    //    }
    //    if (type == "Skill_1" && _skill_1_has && _mp > 0f && !_skill_1_cd)
    //    {
    //        _skill_1_cd = true;
    //        _skill_1_fill.fillAmount = 1f;
    //        _mp -= 10f;
    //        PlayerCtrl.Instance.SkillGo(1);
    //    }
    //}

    //private void SkillCD()
    //{
    //    if (_skill_0_cd)
    //    {
    //        _skill_0_fill.fillAmount -= Time.deltaTime / 5f;
    //        if (_skill_0_fill.fillAmount <= 0f)
    //        {
    //            _skill_0_fill.fillAmount = 0f;
    //            _skill_0_cd = false;
    //        }
    //    }
    //    if (_skill_1_cd)
    //    {
    //        _skill_1_fill.fillAmount -= Time.deltaTime / 5f;
    //        if (_skill_1_fill.fillAmount <= 0f)
    //        {
    //            _skill_1_fill.fillAmount = 0f;
    //            _skill_1_cd = false;
    //        }
    //    }
    //}

    //private void Check()
    //{
    //    if (_potionHpNum > 0)
    //    {
    //        _potionHp.SetActive(true);
    //        _potionHpText.text = _potionHpNum.ToString();
    //    }
    //    else
    //    {
    //        _potionHp.SetActive(false);
    //    }
    //}

    //public void PickUp(GameObject selected)
    //{
    //    SpriteRenderer original = selected.GetComponent<SpriteRenderer>();
    //    Sprite temp = original.sprite;
    //    original.sprite = _clothes[0].sprite;
    //    _clothes[0].sprite = temp;
    //    SetPlayerClothes();
    //}

    ///// <summary>
    ///// 顯示武器欄位
    ///// </summary>
    ///// <param name="major"></param>
    ///// <param name="minor"></param>
    //public void DisplayWeapon(Sprite major, Sprite minor)
    //{
    //    _arms[0].sprite = major;
    //    _arms[1].sprite = minor;
    //}

    ///// <summary>
    ///// 取得玩家物件現有裝備
    ///// </summary>
    //private void GetPlayerClothes()
    //{
    //    for (int i = 0; i < PlayerCtrl.Instance.clothes.Length; i++)
    //    {
    //        _clothes[i].sprite = PlayerCtrl.Instance.clothes[i].GetComponent<SpriteRenderer>().sprite;
    //    }
    //}

    ///// <summary>
    ///// 同步裝備精靈圖到玩家物件
    ///// </summary>
    //private void SetPlayerClothes()
    //{
    //    for (int i = 0; i < PlayerCtrl.Instance.clothes.Length; i++)
    //    {
    //        PlayerCtrl.Instance.clothes[i].GetComponent<SpriteRenderer>().sprite = _clothes[i].sprite;
    //    }
    //}
}

public static class TorchVFX
{
    private static float _cd;

    public static void UPDATE(Transform maskRect, Transform noiseRect, Image maskImage)
    {
        if (Time.time > _cd)
        {
            _cd = Time.time + Random.Range(0.3f, 0.5f);
            ShakeImage(maskRect);
            RandomVisible(maskImage);
        }
        ShakeImage(noiseRect);
    }

    private static void ShakeImage(Transform tf)
    {
        tf.localScale = new Vector3(Random.Range(1.01f, 1.03f), Random.Range(1.01f, 1.03f), 1f);
        tf.position = new Vector3(Random.Range(-0.005f, 0.005f), Random.Range(-0.005f, 0.005f), 0f);
    }

    private static void RandomVisible(Image img)
    {
        img.color = new Color(img.color.r, img.color.g, img.color.b, Random.Range(0.6f, 0.8f));
    }
}
