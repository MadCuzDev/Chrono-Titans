using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : MonoBehaviour
{
    public GameObject arrow;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Instantiate(arrow, gameObject.transform);
        }
    }
}
