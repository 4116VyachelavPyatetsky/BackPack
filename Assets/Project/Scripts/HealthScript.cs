using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class HealthScript : MonoBehaviour
{
    public Image healthImage;
    public TMP_Text healthText;

    public void Damage(int currentHealth)
    {
        healthImage.fillAmount = (float)currentHealth / 100;
        healthText.text = currentHealth.ToString();
    }
}
