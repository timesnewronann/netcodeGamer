using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using TMPro;


// Health system to try and take damage for players 
public class Health : NetworkBehaviour
{
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;
    public Image healthBarImage; // Assign this in the inspector
    public TextMeshProUGUI healthText; // Reference to the TextMeshProUGUI component

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            currentHealth = maxHealth;
            UpdateHealthUI();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void TakeDamageServerRpc(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        // Call an RPC to update the UI on the client
        UpdateHealthClientRpc(currentHealth);
        
        if (currentHealth <= 0)
        {
            // Handle death here
        }
    }

    [ClientRpc]
    private void UpdateHealthClientRpc(int newHealth)
    {
        // Update the health UI only on the client that owns this object
        if(IsOwner)
        {
            UpdateHealthUI();
        }
    }

    private void UpdateHealthUI()
    {
        if (healthBarImage != null)
        {
            healthBarImage.fillAmount = (float)currentHealth / maxHealth;
        }
        
        // Update the health text with the current health
        if (healthText != null)
        {
            healthText.text = $"Health: {currentHealth} / {maxHealth}";
        }
    }
}
