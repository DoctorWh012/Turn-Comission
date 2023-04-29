using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableEnemy", menuName = "Turn Comission/ScriptableEnemy", order = 0)]
public class ScriptableEnemy : ScriptableObject
{
    public Sprite enemySprite;
    public string enemyName;
    [TextArea] public string enemyStartText;
    public int enemyHealth;
    public bool ableToFlee;
    public ScriptableItem enemyWeapon;
    [TextArea] public string attackDescripttion;
    [TextArea] public string blindAttackDescription;

    public ParticleSystem[] enemyAttacks;
}