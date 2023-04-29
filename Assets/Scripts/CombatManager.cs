using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance;
    public int turnPoints
    {
        get => _turnPoints;
        set
        {
            _turnPoints = value;
            UpdateTurnPointsValue();
        }
    }

    public int playerHealth
    {
        get => _playerHealth;
        set
        {
            _playerHealth = value;
            UpdateHealthValue();
        }
    }

    public int playerFear
    {
        get => _playerFear;
        set
        {
            _playerFear = value;
            UpdateFearValue();
        }
    }

    public int playerItemIdex
    {
        get => _playerItemIndex;
        set
        {
            _playerItemIndex = value;
            UpdateItemValue();
        }
    }

    public int enemyHealth
    {
        get => _enemyhealth;
        set
        {
            _enemyhealth = value;
            UpdateEnemyHealth();
        }
    }

    public int rangedProficiency
    {
        get => _rangedProficiency;
        set
        {
            _rangedProficiency = (value < 25) ? value : 25;
        }
    }

    public int meleeProficiency
    {
        get => _meleeProficiency;
        set
        {
            _meleeProficiency = (value < 25) ? value : 25;
        }
    }

    [Header("Components")]
    [SerializeField] private Animator animator;

    [Header("Player")]
    [SerializeField] private TextMeshProUGUI playerHealthTXT;
    [SerializeField] private TextMeshProUGUI playerFearTXT;
    [SerializeField] private TextMeshProUGUI playerItemTXT;
    [SerializeField] private TextMeshProUGUI turnPointsTXT;

    [Header("Player Combat")]
    [SerializeField] private int maxTurnPoints;
    [SerializeField] private int _turnPoints;
    [SerializeField] private int _playerHealth;
    [SerializeField] private int _playerFear;
    [SerializeField] private int _playerItemIndex;
    [SerializeField] public ScriptableItem[] playerItemsOnInventory;

    [Header("Enemy")]
    [SerializeField] private Image enemySprite;
    [SerializeField] private TextMeshProUGUI enemyNameTXT;
    [SerializeField] private TextMeshProUGUI enemyHealthTXT;
    [SerializeField] private TextMeshProUGUI enemyItemTXT;

    [Header("EnemyCombat")]
    [SerializeField] public ScriptableEnemy enemySettings;
    [HideInInspector] private int _enemyhealth;

    [Header("Battle")]
    [Range(0, 25)]
    [SerializeField] private int _meleeProficiency;
    [Range(0, 25)]
    [SerializeField] private int _rangedProficiency;

    [SerializeField] public int proficiencyIncreaseWhenHit;
    [SerializeField] public int proficiencyIncreaseWhenMiss;
    [SerializeField] public int fleeingChange; // in % so 50 is 50% chance of fleeing
    [SerializeField] public int attackCost;
    [SerializeField] public int primeWeaponCost;
    [SerializeField] public int useItemCost;

    [SerializeField] public int bleedLasts;
    [SerializeField] public int bleedDamage;

    public int turn = 0;
    public bool canFlee;

    public int playerStartedBleedingTurn;
    public int enemyStartedBleedingTurn;

    // Status
    // Player
    public bool primed;
    public int fear;
    public bool playerBleed;

    //Enemy
    public bool hazard;
    public bool special;
    public bool enemyBleed;


    private void Awake()
    {
        Instance = this;
        enemyHealth = enemySettings.enemyHealth;
    }

    private void Start()
    {
        SetupEnemy();
        SetupPlayer();
    }

    // UI
    private void UpdateTurnPointsValue()
    {
        turnPointsTXT.SetText($"Turn Points: {turnPoints}");
    }

    private void UpdateHealthValue()
    {
        playerHealthTXT.SetText($"Hp: {playerHealth}");
    }

    private void UpdateFearValue()
    {
        playerFearTXT.SetText($"Fear: {playerFear}");
    }

    private void UpdateItemValue()
    {
        playerItemTXT.SetText($"Item: {playerItemsOnInventory[playerItemIdex].itemName}");
    }

    public void UpdateEnemyHealth()
    {

        enemyHealthTXT.SetText($"Hp: {enemyHealth.ToString()}");
    }

    private void SetupPlayer()
    {
        UpdateTurnPointsValue();
        UpdateHealthValue();
        UpdateFearValue();
        UpdateItemValue();
    }

    private void SetupEnemy()
    {
        enemySprite.sprite = enemySettings.enemySprite;
        enemyNameTXT.SetText(enemySettings.enemyName);
        UpdateEnemyHealth();
        enemyItemTXT.SetText($"Item: {enemySettings.enemyWeapon.itemName}");
        canFlee = enemySettings.ableToFlee;
    }


    // Combat
    public void AttemptToFlee()
    {
        if (!canFlee || turnPoints - maxTurnPoints < 0) return;
        turnPoints -= maxTurnPoints;

        if (Random.Range(0, 100) < fleeingChange)
        {
            print("Was able to flee");
            // Your flee logic
        }
        else
        {
            print("Wasn't able to flee");
            turnPoints -= turnPoints;
        }
    }

    public void StartPlayerAttack()
    {
        if (turnPoints - attackCost < 0) { print("Unable to attack"); return; }
        if (playerItemsOnInventory[playerItemIdex].weaponType == WeaponType.Ranged && !primed) { print("Unable to attack cause weapon is not primed"); return; }

        turnPoints -= attackCost;

        int i = Random.Range(0, playerItemsOnInventory[playerItemIdex].attacks.Length);
        BattleMenuManager.Instance.EnableBulletHell();
        BulletHellManager.Instance.SetupGame(playerItemsOnInventory[playerItemIdex].attacks[i], true);

        // BattleMenuManager.Instance.EnableBattleSelectMenu();
        // StartCoroutine(PlayerAttack());
    }

    public void SkipTurn()
    {
        turn++;

        if (enemyBleed && turn > enemyStartedBleedingTurn + bleedLasts) enemyBleed = false;
        if (playerBleed && turn > playerStartedBleedingTurn + bleedLasts) playerBleed = false;

        if (enemyBleed) { enemyHealth -= bleedDamage; print("Appied bleed damage on enemy"); }
        if (playerBleed) { playerHealth -= bleedDamage; print("Applied bleed damage on player"); }

        // Set to Battle menu
        BattleMenuManager.Instance.EnableBattleSelectMenu();

        // Enemy turn logic
        // StartCoroutine(EnemyAttack());
        int i = Random.Range(0, enemySettings.enemyAttacks.Length);
        BattleMenuManager.Instance.EnableBulletHell();
        BulletHellManager.Instance.SetupGame(enemySettings.enemyAttacks[i], false);

        // Player Turn Start logic
        turnPoints = maxTurnPoints;
    }

    public void PrimeWeapon()
    {
        if (turnPoints - primeWeaponCost < 0 || primed) return;
        turnPoints -= primeWeaponCost;

        primed = true;

        print("Weapon Primed and ready to fire");
    }

    private IEnumerator PlayerAttack()
    {
        animator.Play("PlayerAttack");
        while (animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerAttack")) yield return null;
        yield return new WaitForSeconds(1);

        int AttackLandChace = playerItemsOnInventory[playerItemIdex].weaponType == WeaponType.Ranged ? (rangedProficiency * 4) : (meleeProficiency * 4);

        if (Random.Range(0, 100) < AttackLandChace)
        {
            enemyHealth -= playerItemsOnInventory[playerItemIdex].itemDamage;
            if (playerItemsOnInventory[playerItemIdex].weaponType == WeaponType.Ranged)
            {
                rangedProficiency += proficiencyIncreaseWhenHit;
                primed = false;
            }
            else meleeProficiency += proficiencyIncreaseWhenHit;

            if (playerItemsOnInventory[playerItemIdex].canCauseBleed)
            {
                if (Random.Range(0, 100) < playerItemsOnInventory[playerItemIdex].bleedChance) { enemyBleed = true; enemyStartedBleedingTurn = turn; }
            }

            print("PLAYER HIT");
        }
        else
        {
            if (playerItemsOnInventory[playerItemIdex].weaponType == WeaponType.Ranged)
            {
                rangedProficiency += proficiencyIncreaseWhenMiss;
                primed = false;
            }
            else meleeProficiency += proficiencyIncreaseWhenMiss;

            print("PLAYER MISS");
        }
    }

    private IEnumerator EnemyAttack()
    {
        BattleMenuManager.Instance.EnableDisableActionRow(false);
        animator.Play("EnemyAttack");
        while (animator.GetCurrentAnimatorStateInfo(0).IsName("EnemyAttack")) yield return null;
        yield return new WaitForSeconds(1);



        if (Random.Range(0, 100) < 70) // TO be ajusted
        {
            playerHealth -= enemySettings.enemyWeapon.itemDamage;

            if (enemySettings.enemyWeapon.canCauseBleed)
            {
                if (Random.Range(0, 100) < enemySettings.enemyWeapon.bleedChance) { playerBleed = true; playerStartedBleedingTurn = turn; }
            }

            print("Enemy HIT");
        }
        else
        {
            print("Enemy MISS");
        }
        BattleMenuManager.Instance.EnableDisableActionRow(true);
    }
}
