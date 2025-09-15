using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class EnemyHealthUI : NetworkBehaviour
{
    [Header("Referencias")]
    [SerializeField] private EnemyAI enemyScript; 
    [SerializeField] private Slider healthSlider; 

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    public override void OnNetworkSpawn()
    {
        enemyScript.Health.OnValueChanged += OnHealthChanged;

        OnHealthChanged(0, enemyScript.Health.Value);
    }

    public override void OnNetworkDespawn()
    {
        enemyScript.Health.OnValueChanged -= OnHealthChanged;
    }


    private void OnHealthChanged(int previousValue, int newValue)
    {
        Debug.Log($"EVENTO UI: La vida cambió de {previousValue} a {newValue}. Actualizando slider.");

        healthSlider.value = (float)newValue / enemyScript.maxHealth;
    }

    // Usamos LateUpdate para que la UI se actualice después de que la cámara se haya movido
    private void LateUpdate()
    {
        if (mainCamera != null)
        {
            // Hacemos que la barra de vida siempre mire a la cámara (billboarding)
            transform.LookAt(transform.position + mainCamera.transform.forward);
        }
    }

}