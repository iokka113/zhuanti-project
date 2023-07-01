using UnityEngine;

[CreateAssetMenu()]
public class PlayerCharacterData : ScriptableObject
{
    [SerializeField]
    private GameObject _characterPrefab = null;
    public GameObject CharacterPrefab { get => _characterPrefab; }

    [Header("Character Profile")]

    [SerializeField]
    private Sprite _profileImage = null;
    public Sprite ProfileImage { get => _profileImage; }

    [SerializeField]
    private Sprite _profileImageHead = null;
    public Sprite ProfileImageHead { get => _profileImageHead; }

    [Multiline]
    [SerializeField]
    private string _profileBio = null;
    public string ProfileBio { get => _profileBio; }

    [Header("Character Setting")]

    [SerializeField]
    private float _originalHp = 0;
    public float OriginalHp { get => _originalHp; }

    [SerializeField]
    private float _originalMp = 0;
    public float OriginalMp { get => _originalMp; }
}
