using System;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour {

	public float lookRadius = 10f;
	public GameObject loot;

	Transform target;
	NavMeshAgent agent;

	public Slider healthBar;
	
	public Transform player;

	int health = 3;

	void Start()
	{
		target = player;
		agent = GetComponent<NavMeshAgent>();
	}

	void Update ()
	{
		healthBar.value = health;
		
		// Get the distance to the player
		float distance = Vector3.Distance(target.position, transform.position);

		// If inside the radius
		if (distance <= lookRadius)
		{
			// Move towards the player
			agent.SetDestination(target.position);
			if (distance <= agent.stoppingDistance)
			{
				// Attack
				FaceTarget();
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

	// Point towards the player
	void FaceTarget ()
	{
		Vector3 direction = (target.position - transform.position).normalized;
		Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
		transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
	}

	void OnDrawGizmosSelected ()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, lookRadius);
	}

}
