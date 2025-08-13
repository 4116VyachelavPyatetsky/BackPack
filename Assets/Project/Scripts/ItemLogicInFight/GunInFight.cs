using UnityEngine;
using UnityEngine.UI;

public class GunInFight : ItemInFight
{
    public RectTransform enemy, startPos;
    public GameObject projectile;

    void Start()
    {
        base.Start();
        enemy = GameObject.FindWithTag("Enemy").GetComponent<RectTransform>();
        startPos = GameObject.FindWithTag("Player").GetComponent<RectTransform>();
    }

    public override void Loaded()
    {
        GameObject proj = Instantiate(projectile, startPos.position, Quaternion.identity, startPos.parent);
        proj.GetComponent<ProjectileScript>().target = enemy;
    }
}
