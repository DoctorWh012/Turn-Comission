using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectBTNOnActivate : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Button buttonToSelect;

    private void OnEnable()
    {
        buttonToSelect.Select();
    }
}
