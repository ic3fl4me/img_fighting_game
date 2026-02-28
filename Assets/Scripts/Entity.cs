using System.Collections;
using UnityEngine;

public class Entity : MonoBehaviour, IAttackable
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] public int currentHealth;

    [SerializeField] private float shakeDuration = 0.15f;
    [SerializeField] private float shakeStrength = 0.1f;

    private Vector3 originalPos;

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

        Debug.Log("Damage Taken");
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
