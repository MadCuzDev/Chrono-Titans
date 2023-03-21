using System;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine.UI;

public class SlimeController : MonoBehaviour {

	public float lookRadius = 10f;
	public GameObject loot;

	Transform target;

	public Slider healthBar;
	
	public Transform player;
	private Rigidbody rigidbody;
	
	int health = 3;
	
	private float cooldown = 0;

	void Start()
	{
		rigidbody = gameObject.GetComponent<Rigidbody>();
	}

	void Update()
	{
		cooldown -= Time.deltaTime;
		healthBar.value = health;
		
		// Get the distance to the player
		var distance = Vector3.Distance(player.position, transform.position);

		// If inside the radius
		if (distance <= lookRadius)
		{
			transform.LookAt(player);
			if (cooldown <= 0)
			{
				rigidbody.velocity = transform.forward * 3 + transform.up * 3;
				cooldown = 2;
			}
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (!collision.gameObject.CompareTag("Arrow")) return;
		Destroy(collision.gameObject);
		health--;
		if (health <= 0)
		{
			var lootGameObject = Instantiate(loot, gameObject.transform);
			lootGameObject.transform.parent = null;
			Destroy(gameObject);
		}
	}

	void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, lookRadius);
	}

}
