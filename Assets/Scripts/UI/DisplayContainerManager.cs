using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayContainerManager : MonoBehaviour
{
    public static DisplayContainerManager instance { get; private set; }

    void Start()
    {
        instance = this;
    }

    public void UpdateHealthBar(Slider healthBar, float curAmount, float maxAmount)
    {
        healthBar.value = curAmount / maxAmount;
    }

    public void UpdateHealthBar(Image healthBar, float curAmount, float maxAmount)
    {
        healthBar.fillAmount = curAmount / maxAmount;
    }

    public void UpdateEnergyBar(Image energyBar, float curAmount, float maxAmount)
    {
        energyBar.fillAmount = curAmount / maxAmount;
    }

    public void UpdateCoinText(Text coinText, float curAmount)
    {
        coinText.text = curAmount.ToString();
    }
}
