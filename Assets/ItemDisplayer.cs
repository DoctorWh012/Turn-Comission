using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class ItemDisplayer : MonoBehaviour
{
    public int itemIndex;
    public Image weaponImage;
    public TextMeshProUGUI weaponName;
    public TextMeshProUGUI equipTXT;
    public Button descriptionChangeBTN;

    public void ChangeDescription()
    {
        BattleMenuManager.Instance.description.SetText(CombatManager.Instance.playerItemsOnInventory[itemIndex].itemDescription);
    }

    public void EquipItem()
    {
        CombatManager.Instance.playerItemIdex = itemIndex;
        BattleMenuManager.Instance.EnableBattleSelectMenu();
    }
}
