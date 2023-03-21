using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpInteractableObject : MonoBehaviour
{
    [SerializeField] ItemData itemData;

    private void Start()
    {
        GetComponent<InteractableObject>().interact += PickUp;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GameObject.Find("RigidBodyFPSController").GetComponent<Inventory>().AddItem(itemData);
            Destroy(gameObject);
        }
    }

    public void PickUp(Inventory inventory)
    {
        if (itemData != null) inventory.AddItem(itemData);
        Destroy(gameObject);
    }
}
