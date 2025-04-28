using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class ExampleBehaviour : MonoBehaviour
{
    public Action onUpdate;

    // Update is called once per frame
    void Update()
    {
        onUpdate?.Invoke();
    }
}
