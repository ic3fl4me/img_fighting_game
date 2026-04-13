using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class HealthManager : MonoBehaviour
{
    public Image healthBar;
    public float healthAmount = 100f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Input Test
        /*
        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            Debug.Log("Q gedrückt - Schaden wird berechnet!");
            TakeDamage(20);
        }

        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            Debug.Log("R gedrückt - Dummy wird geheilt oder resettet!");
            Heal(5);
        }
        */
    }

    public void TakeDamage(float damage)
    {
        healthAmount -= damage;
        healthBar.fillAmount = healthAmount / 100f;
    }


    public void Heal(float healingAmount)
    {
        healthAmount += healingAmount;
        healthAmount = Mathf.Clamp(healthAmount, 0, 100);

        healthBar.fillAmount = healthAmount / 100f;
    }

}
