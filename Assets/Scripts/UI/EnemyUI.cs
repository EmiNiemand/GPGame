using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUI : MonoBehaviour
{
    public Slider hpBar;

    // Start is called before the first frame update
    public void Setup()
    {
        hpBar = transform.Find("HP").GetComponent<Slider>();
    }

    public void SetMaxHP(int maxHP) 
    { 
        hpBar.maxValue = maxHP; 
        hpBar.value = maxHP;
        hpBar.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(maxHP*0.2f, 0.2f);
    }

    public void UpdateHP(int currentHP)
    {
        hpBar.value = currentHP;
    }
}
