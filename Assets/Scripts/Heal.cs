using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

// El nombre de la clase ahora es HealthUI
public class HealthUI1 : NetworkBehaviour
{
    [Header("Referencias")]
    // MODIFICADO: Ahora la referencia es al script del jugador.
    [SerializeField] private SimplePlayerController playerScript;
    [SerializeField] private Slider healthSlider;

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    public override void OnNetworkSpawn()
    {
        // MODIFICADO: Nos suscribimos a la variable "health" del jugador.
        playerScript.health.OnValueChanged += OnHealthChanged;
        OnHealthChanged(0, playerScript.health.Value);
    }

    public override void OnNetworkDespawn()
    {
        playerScript.health.OnValueChanged -= OnHealthChanged;
    }

    private void OnHealthChanged(int previousValue, int newValue)
    {
        // MODIFICADO: Usamos la "maxHealth" del script del jugador.
        healthSlider.value = (float)newValue / playerScript.maxHealth;
    }

    private void LateUpdate()
    {
        if (mainCamera != null)
        {
            transform.LookAt(transform.position + mainCamera.transform.forward);
        }
    }
}