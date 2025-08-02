using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    [Header("Customer Data")]
    public TMP_Text happyCustText;
    public TMP_Text normalCustText;
    public TMP_Text sadCustText;
    public int happyCustomers = 0;
    public int normalCustomers = 0;
    public int sadCustomers = 0;

    [Header("Score Display")]
    public TextMeshProUGUI scoreText;
    public int score = 0;

    [Header("End Screen")]

    public bool isEnding = false;
    public GameObject endScreen;

    void Start()
    {
        endScreen.SetActive(false);
        UpdateText();
    }

    void Update()
    {
        CheckCustomerLeft();
    }

    public void AddScore(int points)
    {
        score += points;
        UpdateText();
    }

    public void DecreaseScore(int amount)
    {
        score -= amount;
        UpdateText();
    }

    public void CheckCustomerLeft()
    {
        if (GameObject.FindGameObjectsWithTag("Customer").Length == 0 && isEnding)
        {
            endScreen.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    private void UpdateText()
    {
        scoreText.text = score.ToString();
        happyCustText.text = happyCustomers.ToString();
        normalCustText.text = normalCustomers.ToString();
        sadCustText.text = sadCustomers.ToString();
    }
}
