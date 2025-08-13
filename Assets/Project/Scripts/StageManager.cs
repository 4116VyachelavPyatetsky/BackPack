using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance { get; private set; }

    public PlayerStats playerStats;
    public TMP_Text money;
    public HealthScript health;
    private PlayerStats runtimeStats;
    public GameObject backPackStage;
    public ItemsCreator creator;

    private void Awake()
    {
        // Устанавливаем единственный экземпляр
        Instance = this;

        runtimeStats = Instantiate(playerStats);
        money.text = runtimeStats.money.ToString();
    }

    public void HealPlayer(int amount)
    {
        runtimeStats.Heal(amount);
        health.Damage(runtimeStats.health);
    }

    public void DamagePlayer(int damage)
    {
        runtimeStats.TakeDamage(damage);
        health.Damage(runtimeStats.health);
    }

    public bool SpendMoney(int amount)
    {
        if (runtimeStats.money >= amount)
        {
            runtimeStats.RemoveMoney(amount);
            money.text = runtimeStats.money.ToString();
            return true;
        }
        else
        {
            return false;
        }
    }

    public void StartFight()
    {

    }

    public void EndFight()
    {
        backPackStage.SetActive(true);
        creator.SpawnItems();
    }
}

