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
            menuPanel.SetActive(true); // Ensure the menu panel is hidden at start
            Time.timeScale = 0f;
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
