using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ToggleActiveOnPress : MonoBehaviour
{
    public KeyCode keyCode;
    public GameObject gameObjectToToggle;

    private void Update()
    {
        if (Input.GetKeyDown(keyCode)) gameObjectToToggle.SetActive(!gameObjectToToggle.activeSelf);
    }
}
