using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    private Slider playerHP;
    private int healthMax = 5;
    // Start is called before the first frame update
    public void Setup()
    {
        playerHP = transform.Find("PlayerHP").GetComponent<Slider>();
    }

    public void SetMaxHealth(int maxHealth)
    {
        healthMax = maxHealth;
        playerHP.maxValue = maxHealth;
        playerHP.value = maxHealth;
        playerHP.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(maxHealth*40, 40);
    }
    
    public void UpdateHealth(int currentHealth)
    {
        playerHP.value = currentHealth;
        // if(currentHealth*2 < healthMax)
        //     playerHP.color = new Color(1.0f, 0.0f, 0.4f);
        // else
        //     playerHP.color = new Color(0.0f, 1.0f, 0.4f);
    }
}
