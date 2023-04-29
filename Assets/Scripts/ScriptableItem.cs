using UnityEngine;
public enum WeaponType
{
    Melee,
    Ranged,
}

[CreateAssetMenu(fileName = "ScriptableItem", menuName = "Turn Comission/ScriptableItem", order = 0)]
public class ScriptableItem : ScriptableObject
{
    [Header("Combat Settings")]
    public WeaponType weaponType;
    public int itemDamage;
    public ParticleSystem[] attacks;

    [TextArea] string attackText;

    public bool canCauseBleed;
    [Range(0, 100)] public int bleedChance;

    [Header("UI Settings")]
    public Sprite weaponSprite;
    public string itemName;
    public string blindFireText;
    public string aimText;
    [TextArea]
    public string itemDescription;
}