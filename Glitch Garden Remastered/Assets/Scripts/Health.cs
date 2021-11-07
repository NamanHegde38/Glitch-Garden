using System;
using UnityEngine;

public class Health : MonoBehaviour {

    [SerializeField] private int health = 100;
    [SerializeField] private GameObject deathVFX;

    private int _difficulty = 1;
    
    private void Start() {
        _difficulty = PlayerPrefsController.GetDifficulty();
        health = SetHealth(health);
    }

    private int SetHealth(float baseHealth) {
        switch (_difficulty) {
            case 1:
                baseHealth *= 1f;
                break;
            case 2:
                baseHealth *= 1.5f;
                break;
            case 3:
                baseHealth *= 2f;
                break;
        }

        return Mathf.RoundToInt(baseHealth);
    }
    
    public void DealDamage(int damage) {
        health -= damage;
        
        if (health > 0) return;
        TriggerDeathVFX();
        Destroy(gameObject);
    }
    
    private void TriggerDeathVFX() {
        if (!deathVFX) return;
        var deathVFXObject = Instantiate(deathVFX, transform.position, transform.rotation);
        Destroy(deathVFXObject, 1f);
    }
}
