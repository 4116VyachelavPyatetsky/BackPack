using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    const float speed = 10f;
    public RectTransform target;

    void FixedUpdate()
    {
        if (target == null) return;
        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            speed
        );
        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            target.GetComponent<EnemyScript>().OnHit(Random.Range(0,1f));
            Destroy(gameObject);
        }
    }
}
