using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class EnemyHealthUI : NetworkBehaviour
{
    [Header("Referencias")]
    [SerializeField] private EnemyAI enemyScript; // Arrastra el script del enemigo aqu�
    [SerializeField] private Slider healthSlider; // Arrastra el componente Slider aqu�

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    public override void OnNetworkSpawn()
    {
        // Nos suscribimos al evento de cambio de valor de la vida
        enemyScript.Health.OnValueChanged += OnHealthChanged;

        // Actualizamos la barra una vez al aparecer
        OnHealthChanged(0, enemyScript.Health.Value);
    }

    public override void OnNetworkDespawn()
    {
        // Nos desuscribimos para evitar errores
        enemyScript.Health.OnValueChanged -= OnHealthChanged;
    }

    // Dentro de EnemyHealthUI.cs

    private void OnHealthChanged(int previousValue, int newValue)
    {
        // --- DEBUG 4: Para ver si la UI recibe la actualizaci�n ---
        Debug.Log($"EVENTO UI: La vida cambi� de {previousValue} a {newValue}. Actualizando slider.");

        // Actualizamos el valor del slider (normalizado entre 0 y 1)
        healthSlider.value = (float)newValue / enemyScript.maxHealth;
    }

    // Usamos LateUpdate para que la UI se actualice despu�s de que la c�mara se haya movido
    private void LateUpdate()
    {
        if (mainCamera != null)
        {
            // Hacemos que la barra de vida siempre mire a la c�mara (billboarding)
            transform.LookAt(transform.position + mainCamera.transform.forward);
        }
    }

}