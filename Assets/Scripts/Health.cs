using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Health : NetworkBehaviour {

	public const int maxHealth = 100;
	public bool destroyOnDeath; 

	[SyncVar(hook = "OnHealthChange")]
	public int currentHealth = maxHealth;
	public RectTransform healthBar;

	private NetworkStartPosition[] spawnPoints;

	void Start(){

		if (isLocalPlayer) {
			spawnPoints = FindObjectsOfType<NetworkStartPosition> ();
		}
	}


	public void TakeDamage(int amount){

		if (!isServer) {
			return;
		}

		currentHealth -= amount;
		if (currentHealth <= 0) {
			if (destroyOnDeath) {
				Destroy (gameObject);
			} else { 
				//currentHealth = 0; 
				//add falling over and wait for seconds
				currentHealth = maxHealth;
				//called on the server but invoked on clients
				RpcRespawn ();
			}
		}
	}

	void OnHealthChange (int currentHealth){

		healthBar.sizeDelta = new Vector2 (currentHealth, healthBar.sizeDelta.y);
	}

	[ClientRpc]
	void RpcRespawn()
	{

		if (isLocalPlayer) {
			Vector3 spawnPoint = Vector3.zero;

			if (spawnPoints != null && spawnPoints.Length > 0) {

				spawnPoint = spawnPoints [Random.Range (0, spawnPoints.Length)].transform.position;
			}
			transform.position = spawnPoint;
		}
	}
}
