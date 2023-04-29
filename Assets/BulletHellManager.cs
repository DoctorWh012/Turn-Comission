using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class BulletHellManager : MonoBehaviour
{
    public static BulletHellManager Instance;

    [Header("Menu Stuff")]
    [SerializeField] private TextMeshProUGUI enemyName;
    [SerializeField] private GameObject enemyBleed;
    [SerializeField] private GameObject enemyHazard;
    [SerializeField] private GameObject enemySpecial;

    [SerializeField] private TextMeshProUGUI playerHealth;
    [SerializeField] private GameObject playerBleed;
    [SerializeField] private GameObject playerPrimed;
    [SerializeField] private TextMeshProUGUI playerFear;

    [Header("Components")]
    [SerializeField] private SimplePlayerMovement simplePlayerMovement;
    [SerializeField] private Collider2D playerCol;
    [SerializeField] private Transform player;
    [SerializeField] private Transform enemy;
    [SerializeField] private GameObject enemyShield;
    [SerializeField] private Transform startingPlayerPos;
    [SerializeField] private Transform startingEnemyPos;
    [SerializeField] private Transform playerParticleSystemPos;
    [SerializeField] private Transform enemyParticleSystemPos;

    Coroutine enemyAttackCoroutine;
    Coroutine playerAttackCoroutine;
    ParticleSystem ps;

    private void Awake()
    {
        Instance = this;
    }

    public void SetupGame(ParticleSystem bulletHellParticles, bool playerAttacking)
    {
        if (playerAttacking)
        {
            SetupMenu();
            player.position = startingPlayerPos.position;
            enemy.position = startingEnemyPos.position;

            simplePlayerMovement.enabled = false;
            playerAttackCoroutine = StartCoroutine(PlayerAttack(bulletHellParticles));

        }
        else
        {
            SetupMenu();
            player.position = startingPlayerPos.position;
            enemy.position = startingEnemyPos.position;

            simplePlayerMovement.enabled = true;
            enemyAttackCoroutine = StartCoroutine(EnemyAttack(bulletHellParticles));
        }

    }

    public void SetupMenu()
    {
        ResetMenu();
        if (CombatManager.Instance.enemyBleed) enemyBleed.SetActive(true);
        if (CombatManager.Instance.hazard) enemyHazard.SetActive(true);
        if (CombatManager.Instance.special) enemySpecial.SetActive(true);

        playerHealth.SetText(CombatManager.Instance.playerHealth.ToString());
        if (CombatManager.Instance.playerBleed) playerBleed.SetActive(true);
        if (CombatManager.Instance.primed) playerPrimed.SetActive(true);
        playerFear.SetText(CombatManager.Instance.playerFear.ToString());
    }

    private void ResetMenu()
    {
        enemyBleed.SetActive(false);
        enemyHazard.SetActive(false);
        enemySpecial.SetActive(false);

        playerBleed.SetActive(false);
        playerPrimed.SetActive(false);
    }

    public void EndBattle(bool wasHit)
    {
        Destroy(ps.gameObject);
        if (wasHit) CombatManager.Instance.playerHealth -= CombatManager.Instance.enemySettings.enemyWeapon.itemDamage;

        BattleMenuManager.Instance.EnableBattleSelectMenu();
    }

    private IEnumerator EnemyAttack(ParticleSystem particleSystem)
    {
        enemyShield.SetActive(false);
        enemy.gameObject.SetActive(false);
        playerCol.enabled = true;
        ps = Instantiate(particleSystem, enemyParticleSystemPos.position, Quaternion.Euler(new Vector3(90, 0, 0)));

        ps.Play();

        while (ps.isEmitting) yield return null;

        EndBattle(false);
    }
    private IEnumerator PlayerAttack(ParticleSystem particleSystem)
    {
        playerCol.enabled = false;
        enemy.gameObject.SetActive(true);
        ps = Instantiate(particleSystem, playerParticleSystemPos.position, Quaternion.Euler(new Vector3(-90, 0, 0)));
        ps.Play();

        int AttackLandChace = CombatManager.Instance.playerItemsOnInventory[CombatManager.Instance.playerItemIdex].weaponType == WeaponType.Ranged ?
        (CombatManager.Instance.rangedProficiency * 4) : (CombatManager.Instance.meleeProficiency * 4);

        if (Random.Range(0, 100) < AttackLandChace)
        {
            enemyShield.SetActive(false);
            CombatManager.Instance.enemyHealth -= CombatManager.Instance.playerItemsOnInventory[CombatManager.Instance.playerItemIdex].itemDamage;
            if (CombatManager.Instance.playerItemsOnInventory[CombatManager.Instance.playerItemIdex].weaponType == WeaponType.Ranged)
            {
                CombatManager.Instance.rangedProficiency += CombatManager.Instance.proficiencyIncreaseWhenHit;
                CombatManager.Instance.primed = false;
            }
            else CombatManager.Instance.meleeProficiency += CombatManager.Instance.proficiencyIncreaseWhenHit;

            if (CombatManager.Instance.playerItemsOnInventory[CombatManager.Instance.playerItemIdex].canCauseBleed)
            {
                if (Random.Range(0, 100) < CombatManager.Instance.playerItemsOnInventory[CombatManager.Instance.playerItemIdex].bleedChance)
                {
                    CombatManager.Instance.enemyBleed = true;
                    CombatManager.Instance.enemyStartedBleedingTurn = CombatManager.Instance.turn;
                }
            }

            print("PLAYER HIT");
        }
        else
        {
            enemyShield.SetActive(true);
            if (CombatManager.Instance.playerItemsOnInventory[CombatManager.Instance.playerItemIdex].weaponType == WeaponType.Ranged)
            {
                CombatManager.Instance.rangedProficiency += CombatManager.Instance.proficiencyIncreaseWhenMiss;
                CombatManager.Instance.primed = false;
            }
            else CombatManager.Instance.meleeProficiency += CombatManager.Instance.proficiencyIncreaseWhenMiss;

            print("PLAYER MISS");
        }

        while (ps.isEmitting) yield return null;
        EndBattle(false);
    }
}
