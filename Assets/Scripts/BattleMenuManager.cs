using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BattleMenuManager : MonoBehaviour
{
    public static BattleMenuManager Instance;
    [Header("Menus")]
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject battleSelect;
    [SerializeField] private GameObject attackSelect;
    [SerializeField] private GameObject itemsSelect;
    [SerializeField] private GameObject statusSelect;
    [SerializeField] private GameObject bulletHellSelect;
    [SerializeField] private GameObject enemyTextPanel;
    [SerializeField] private TextMeshProUGUI enemyText;
    [SerializeField] private GameObject actionRow;
    [SerializeField] private GameObject itemHolder;

    [SerializeField] public TextMeshProUGUI description;
    [SerializeField] private GameObject itemHolderPrefab;

    [Header("Status menu")]
    [SerializeField] private GameObject playerBleed;
    [SerializeField] private GameObject playerPrimed;
    [SerializeField] private TextMeshProUGUI playerFear;

    [SerializeField] private GameObject enemyBleed;
    [SerializeField] private GameObject enemyHazard;
    [SerializeField] private GameObject enemySpecial;

    [Header("Attack menu")]
    [SerializeField] private TextMeshProUGUI weaponAttackTXT;
    [SerializeField] private TextMeshProUGUI blindFireTXT;
    [SerializeField] private TextMeshProUGUI aimDownsightTXT;

    [SerializeField] private GameObject primeBTN;
    [SerializeField] private GameObject patternIndicator;

    [Header("Settings")]
    [SerializeField] private KeyCode backToMainMenuKey;

    [SerializeField] GameObject[] menus;

    private List<GameObject> spawnedItemHolders = new List<GameObject>();
    private List<GameObject> spawnedEffectsHolders = new List<GameObject>();

    private void Awake()
    {
        Instance = this;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Start()
    {
        EnableEnemyTextScreen();
    }

    void Update()
    {
        if (Input.GetKeyDown(backToMainMenuKey)) EnableBattleSelectMenu();
    }

    private void DisableAllMenus()
    {
        for (int i = 0; i < menus.Length; i++) menus[i].SetActive(false);
        DestroyItemsHolders();
    }

    public void EnableBattleSelectMenu()
    {
        DisableAllMenus();
        battleSelect.SetActive(true);
        animator.Play("BattleSelectSlide");
    }

    public void EnableAttackSelectMenu()
    {
        DisableAllMenus();
        attackSelect.SetActive(true);
        SetupButtons();
    }

    public void EnableItemsMenu()
    {
        DisableAllMenus();
        itemsSelect.SetActive(true);
        SpawnItemsHolders();
    }

    public void EnableStatusMenu()
    {
        DisableAllMenus();
        statusSelect.SetActive(true);
        SetupStatus();
    }

    public void EnableBulletHell()
    {
        DisableAllMenus();
        bulletHellSelect.SetActive(true);
    }

    public void EnableEnemyTextScreen()
    {
        DisableAllMenus();
        enemyTextPanel.SetActive(true);
        enemyText.SetText(CombatManager.Instance.enemySettings.enemyStartText);

        StartCoroutine(WaitForKeyPress());
    }

    private void SpawnItemsHolders()
    {
        for (int i = 0; i < CombatManager.Instance.playerItemsOnInventory.Length; i++)
        {
            GameObject item = Instantiate(itemHolderPrefab, Vector3.zero, Quaternion.identity);
            item.transform.SetParent(itemHolder.transform);
            item.transform.position = Vector3.zero;
            item.transform.localScale = Vector3.one;

            spawnedItemHolders.Add(item);

            ItemDisplayer displayer = item.GetComponent<ItemDisplayer>();
            displayer.itemIndex = i;
            displayer.weaponImage.sprite = CombatManager.Instance.playerItemsOnInventory[i].weaponSprite;
            displayer.weaponName.SetText(CombatManager.Instance.playerItemsOnInventory[i].itemName);
            if (i == CombatManager.Instance.playerItemIdex) displayer.equipTXT.SetText("Eqquiped");
            else displayer.equipTXT.SetText("Equip");

            if (i == 0) displayer.descriptionChangeBTN.Select();
        }
    }

    private void SetupStatus()
    {
        DisableAllStatusDisplayers();
        if (CombatManager.Instance.enemyBleed) enemyBleed.SetActive(true);
        if (CombatManager.Instance.primed) playerPrimed.SetActive(true);
        playerFear.SetText($"Fear: {CombatManager.Instance.fear}");

        if (CombatManager.Instance.enemyBleed) enemyBleed.SetActive(true);
        if (CombatManager.Instance.hazard) enemyHazard.SetActive(true);
        if (CombatManager.Instance.special) enemySpecial.SetActive(true);
    }

    private void DisableAllStatusDisplayers()
    {
        playerPrimed.SetActive(false);
        playerBleed.SetActive(false);
        enemyBleed.SetActive(false);
        enemyBleed.SetActive(false);
        enemyHazard.SetActive(false);
        enemySpecial.SetActive(false);
    }

    private void DestroyItemsHolders()
    {
        if (spawnedItemHolders.Count == 0) return;
        for (int i = 0; i < spawnedItemHolders.Count; i++)
        {
            Destroy(spawnedItemHolders[i]);
            spawnedItemHolders.Remove(spawnedItemHolders[i]);
        }
    }

    public void EnableDisableActionRow(bool state)
    {
        actionRow.SetActive(state);
    }

    public void SetupButtons()
    {
        if (CombatManager.Instance.playerItemsOnInventory[CombatManager.Instance.playerItemIdex].weaponType != WeaponType.Ranged) { primeBTN.SetActive(false); patternIndicator.SetActive(false); }
        else { primeBTN.SetActive(true); patternIndicator.SetActive(true); }

        weaponAttackTXT.SetText(CombatManager.Instance.playerItemsOnInventory[CombatManager.Instance.playerItemIdex].itemName);
        blindFireTXT.SetText(CombatManager.Instance.playerItemsOnInventory[CombatManager.Instance.playerItemIdex].blindFireText);
        aimDownsightTXT.SetText(CombatManager.Instance.playerItemsOnInventory[CombatManager.Instance.playerItemIdex].aimText);
    }

    private IEnumerator WaitForKeyPress()
    {
        yield return new WaitForSeconds(1);
        while (!Input.anyKeyDown) yield return null;
        EnableBattleSelectMenu();
    }
}
