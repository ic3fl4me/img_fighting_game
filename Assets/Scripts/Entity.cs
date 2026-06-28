using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Entity : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealth = 100;
    [SerializeField] public float currentHealth;

    [SerializeField] private float shakeDuration = 0.15f;
    [SerializeField] private float shakeStrength = 0.1f;

    private Vector3 originalPos;

    //test healthBar
    public Image healthBar;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void TakeDamage(int damage)
    {
        if (damage > currentHealth)
        {
            currentHealth = 0;
        } else
        {
            currentHealth -= damage;
        }
        StartCoroutine(Shake());

        healthBar.fillAmount = currentHealth / maxHealth;

        //Löschen der Dummys on death
        if (currentHealth == 0 && gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            Destroy(gameObject);
        }
        else if (currentHealth == 0 && gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            GetComponent<BoxCollider2D>().enabled = false;
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        }
    }

    private IEnumerator Shake()
    {
        originalPos = transform.localPosition;

        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float offsetX = Random.Range(-1f, 1f) * shakeStrength;
            float offsetY = Random.Range(-1f, 1f) * shakeStrength;

            transform.localPosition = originalPos + new Vector3(offsetX, offsetY, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPos;
    }
}
