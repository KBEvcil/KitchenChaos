using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestingCharacterSelectionUI : MonoBehaviour
{
    [SerializeField] private Button readyButton;

    private void Awake()
    {
        readyButton.onClick.AddListener(() =>
        {
            CharacterSelectReady.Instance.SetPlayerReady();
            readyButton.image.color = Color.green;
        });
    }
}
