using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameStartCountdownUI : MonoBehaviour
{
    private const string NUMBER_POPUP_TRIGGER = "NumberPopup";

    [SerializeField] private TextMeshProUGUI countdownText;

    private int previousCountdownNumber;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    private void Start()
    {
        KitchenGameManager.Instance.OnStateChange += KitchenGameManager_OnStateChange;

        Hide();
    }

    private void KitchenGameManager_OnStateChange(object sender, System.EventArgs e)
    {
        if(KitchenGameManager.Instance.IsCountdownToStartActive())
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    private void Update()
    {
        int countdownNumber = Mathf.CeilToInt(KitchenGameManager.Instance.GetCountdownToStartTimer());
        countdownText.text = countdownNumber.ToString();

        if(previousCountdownNumber != countdownNumber)
        {
            previousCountdownNumber = countdownNumber;
            animator.SetTrigger(NUMBER_POPUP_TRIGGER);
            SoundManager.Instance.PlayCountdownSound();
        }
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
