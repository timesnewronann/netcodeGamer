using UnityEngine;
using Unity.Netcode;
using Cinemachine;

public class PlayerNetwork : NetworkBehaviour
{
    [SerializeField] private Transform spawnedObjectPrefab;
    [SerializeField] private CinemachineVirtualCamera vc;
    [SerializeField] private AudioListener listener;

    public override void OnNetworkSpawn() {
        if (IsOwner) {
            // Enable the listener
            listener.enabled = true;

            // Set camera priority
            vc.Priority = 1;

            // Enable the PlayerMovement script
            var playerMovement = GetComponent<PlayerMovement>();
            if(playerMovement != null) {
                playerMovement.enabled = true;
            }

            // Hide the cursor
            Cursor.lockState = CursorLockMode.Locked;
        }
        else {
            // Set camera priority low for non-local players
            vc.Priority = 0;

            // Disable the PlayerMovement script
            var playerMovement = GetComponent<PlayerMovement>();
            if(playerMovement != null) {
                playerMovement.enabled = false;
            }
        }
    }
}
