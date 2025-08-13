using UnityEngine;

public class HealingActivatable : ItemInFight
{
    private bool readyToUse = true;

    public override void Loaded()
    {
        readyToUse = true;
        EndReload();
    }

    public void Use()
    {
        if (readyToUse)
        {
            StageManager.Instance.HealPlayer(Random.Range(3,15));
            StartReload();
            readyToUse = false;
        }
    }
}
