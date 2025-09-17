using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
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
        MenuPanel();
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
        if (GameObject.Find("Settings Panel") != null) return;
        AudioManager.instance.PlaySFX("OpenMenuBook");
        Debug.Log("Menu button clicked. Toggling menu panel visibility.");
        if (menuPanel != null && !menuPanel.activeSelf)
        {
            menuPanel.SetActive(true);
            Time.timeScale = 0f;
            Debug.Log("Menu panel opened.");
        }
        else if (menuPanel != null && menuPanel.activeSelf)
        {
            menuPanel.SetActive(false);
            Time.timeScale = 1f; // Resume game time when closing the menu
        }
    }
}
