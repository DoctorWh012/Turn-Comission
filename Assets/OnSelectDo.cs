using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OnSelectDo : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    private enum Action
    {
        AttackDescripttion,
        BlindFireDescription,
        ChangeImage,
    }

    [SerializeField] Action action;
    [SerializeField] TextMeshProUGUI descriptionText;
    [SerializeField] Image imageHolder;
    [SerializeField] Sprite image;

    public void OnSelect(BaseEventData eventData)
    {
        print("DFSF");
        switch (action)
        {
            case Action.AttackDescripttion:
                descriptionText.SetText(CombatManager.Instance.enemySettings.attackDescripttion);
                break;
            case Action.BlindFireDescription:
                descriptionText.SetText(CombatManager.Instance.enemySettings.blindAttackDescription);
                break;
            case Action.ChangeImage:
                imageHolder.sprite = image;
            break;
        }
    }

    public void OnDeselect(BaseEventData eventData)
    {
        print("ADSADSDASDA");
        descriptionText.SetText("");
    }   
}
