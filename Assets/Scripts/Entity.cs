using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Entity : MonoBehaviour, IDamageable
{


    [SerializeField] private int maxHealth = 100;
    [SerializeField] public int currentHealth;

    [SerializeField] private float shakeDuration = 0.15f;
    [SerializeField] private float shakeStrength = 0.1f;

    private Vector3 originalPos;

    //test healthBar
    public Image healthBar;
    public float healthAmount = 100f;

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

        //Debug.Log("Damage Taken");
        Debug.Log(currentHealth);
        
        //dmg der die HealthBar kleiner macht (Copy paste vom HealtManager)
        healthAmount -= damage;
        healthBar.fillAmount = healthAmount / 100f;

        //Löschen der Dummys on death
        if (currentHealth == 0)
        {
            Destroy(gameObject);
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
