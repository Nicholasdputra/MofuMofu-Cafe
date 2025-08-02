using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuPopup : MonoBehaviour
{
    public Button menuButton;
    public GameObject menuPanel;

    void Start()
    {
        if (menuButton != null)
        {
            menuButton.onClick.AddListener(MenuPanel);
        }
        else
        {
            Debug.LogError("Menu Button is not assigned in the inspector.");
        }

        if (menuPanel != null)
        {
            menuPanel.SetActive(false);
        }
        else
        {
            Debug.LogError("Menu Panel is not assigned in the inspector.");
        }
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.M) && menuPanel != null)
        {
            MenuPanel();
        }
    }

    public void MenuPanel()
    {
        AudioManager.instance.PlaySFX("OpenMenuBook");
        Debug.Log("Menu button clicked. Toggling menu panel visibility.");
        if (menuPanel != null && !menuPanel.activeSelf)
        {
            menuPanel.SetActive(true);
        }
        else if (menuPanel != null && menuPanel.activeSelf)
        {
            menuPanel.SetActive(false);
            Time.timeScale = 1f; // Resume game time when closing the menu
        }
    }
}
