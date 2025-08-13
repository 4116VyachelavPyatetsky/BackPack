using UnityEngine;

public class BombItemInFight : ItemInFight
{
    public EnemyScript enemy;

    private bool readyToUse = true;

    void Start()
    {
        base.Start();
        enemy = GameObject.FindWithTag("Enemy").GetComponent<EnemyScript>();
    }

    public override void Loaded()
    {
        readyToUse = true;
        EndReload();
    }

    public void Use()
    {
        if (readyToUse)
        {
            enemy.OnHit(0.5f);
            StartReload();
            readyToUse = false;
        }
    }
}
