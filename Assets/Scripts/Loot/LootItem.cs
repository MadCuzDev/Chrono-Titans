using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootItem : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
