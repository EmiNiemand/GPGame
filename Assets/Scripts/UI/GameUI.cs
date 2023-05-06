using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUI : MonoBehaviour
{
    [SerializeField] private Gradient gradientHP;
    
    private Animator animator;

    private (Slider bar, Image image, int healthMax) playerHP;
    private (TextMeshProUGUI text, float currentTime, bool active) levelTimer;

    private static readonly int HealthChangedHash = Animator.StringToHash("HealthChanged");

    // Setup is called by managing class
    public void Setup()
    {
        playerHP.bar = transform.Find("PlayerHP").GetComponent<Slider>();
        playerHP.image = playerHP.bar.transform.Find("Fill Area").GetChild(0).GetComponent<Image>();

        levelTimer.text = transform.Find("LevelTimer").GetComponentInChildren<TextMeshProUGUI>();

        animator = GetComponent<Animator>();
    }

    #region Health methods
    public void SetMaxHealth(int maxHealth)
    {
        playerHP.healthMax = maxHealth;
        playerHP.bar.maxValue = maxHealth;
        UpdateHealth(maxHealth);
        playerHP.bar.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(maxHealth*40, 40);
    }
    
    public void UpdateHealth(int currentHealth)
    {
        playerHP.bar.value = currentHealth;
        animator.SetTrigger(HealthChangedHash);
        playerHP.image.color = gradientHP.Evaluate((float)currentHealth / playerHP.healthMax);
    }
    #endregion

    #region LevelTimerMethods
    public void UpdateTimer(string value)
    {
        levelTimer.text.text = value;
    }
    #endregion
}
