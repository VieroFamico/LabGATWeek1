using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Health playerHP;
    [SerializeField] private Image maxHPBar;
    [SerializeField] private Image currentHealthBar;

    private void Awake()
    {
        maxHPBar.fillAmount = playerHP.MaxHP() / 10f;
    }
    private void Update()
    {
        currentHealthBar.fillAmount = playerHP.currentHealth / 10f;
    }
}
