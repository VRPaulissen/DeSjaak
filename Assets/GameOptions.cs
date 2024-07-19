using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace DeSjaak
{
    public class GameOptions : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown endConditionDropdown;
        
        private void Awake()
        {
            endConditionDropdown.onValueChanged.AddListener(OnEndConditionChanged);

            var savedIndex = PlayerPrefs.GetInt("EndCondition", 0);
            endConditionDropdown.value = savedIndex;
            endConditionDropdown.RefreshShownValue();
        }

        private void OnDestroy()
        {
            endConditionDropdown.onValueChanged.RemoveListener(OnEndConditionChanged);
        }

        private void OnEndConditionChanged(int index)
        {
            var selectedCondition = (GameManager.GameEndCondition)index;
            GameManager.SetEndCondition(selectedCondition);

            PlayerPrefs.SetInt("EndCondition", index);
            PlayerPrefs.Save();
        }
    }
}
