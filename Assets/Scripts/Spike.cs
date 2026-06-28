using UnityEngine;

public class Spike : MonoBehaviour
{
    [SerializeField] private int damage = 0;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite enabledSpike;

    private float enableTimer = 1f;

    private void Awake()
    {
        Destroy(gameObject, 3f);
    }

    private void Update()
    {
        enableTimer -= Time.deltaTime;

        if (enableTimer <= 0)
        {
            ActivateSpike();
        }   
    }

    private void ActivateSpike()
    {
        GetComponent<BoxCollider2D>().enabled = true;
        spriteRenderer.sprite = enabledSpike;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            other.GetComponent<Entity>().TakeDamage(damage);
        }
    }
}
