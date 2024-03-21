using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using TMPro;

public class Health : NetworkBehaviour
{
    public NetworkVariable<int> HealthPoints = new NetworkVariable<int>();

    [SerializeField] private TextMeshProUGUI healthText;

    private const int maxHealth = 100;

    private void Start()
    {
        if (IsOwner)
        {
            HealthPoints.Value = maxHealth;
        }
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            HealthPoints.OnValueChanged += UpdateHealthUI;
            UpdateHealthUI(HealthPoints.Value, HealthPoints.Value);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void TakeDamageServerRpc(int damage)
    {
        if (IsServer)
        {
            HealthPoints.Value = Mathf.Max(HealthPoints.Value - damage, 0);
            if (HealthPoints.Value <= 0)
            {
                // Handle death here, e.g., disable the player gameObject
                // gameObject.SetActive(false);
                HandleDeath();
            }
        }
    }

    private void HandleDeath()
    {
        // Death handling logic goes here
        Debug.Log("Player has died.");
    }

    private void UpdateHealthUI(int oldHealth, int newHealth)
    {
        if (healthText != null)
        {
            healthText.text = $"Health: {newHealth} / {maxHealth}";
        }
    }

    private void OnDestroy()
    // Fixed the CS0114 error 
    {
        if (IsOwner && NetworkManager.Singleton.IsServer)
        {
            HealthPoints.OnValueChanged -= UpdateHealthUI;
        }
    }

}
