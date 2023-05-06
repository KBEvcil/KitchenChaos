using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectCounterVisual : MonoBehaviour
{
    [SerializeField] private BaseCounter clearCounter;
    [SerializeField] private GameObject[] visualGameObjects;
    private void Start()
    {
        if(Player.LocalInstance)
        {
            Player.LocalInstance.OnSelectedCounterChanged += Player_OnSelectedCounterChanged;
        }
        else
        {
            Player.OnAnyPlayerSpawned += Player_OnAnyPlayerSpawned;
        }
    }

    private void Player_OnAnyPlayerSpawned(object sender, System.EventArgs e)
    {
        if (Player.LocalInstance)
        {
            Player.LocalInstance.OnSelectedCounterChanged -= Player_OnSelectedCounterChanged;
            Player.LocalInstance.OnSelectedCounterChanged += Player_OnSelectedCounterChanged;
        }
    }

    

    private void Player_OnSelectedCounterChanged(object sender, Player.OnSelectedCounterChangedEventArgs e)
    {
        if(e.selectedCounter == clearCounter)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    private void Show()
    {
        foreach(GameObject visualGameObject in visualGameObjects)
            visualGameObject.SetActive(true);
    }

    private void Hide()
    {
        foreach (GameObject visualGameObject in visualGameObjects)
            visualGameObject.SetActive(false);
    }
}
