using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(BoxCollider))]
public class Tutorial : MonoBehaviour
{

    [SerializeField] private GameObject TutorialText;

    private BoxCollider col;

    private void Awake()
    {
        col = GetComponent<BoxCollider>();
    }

    private void Update()
    {
        if (col.enabled == false)
        {
            TutorialText.SetActive(false);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        TutorialText.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        TutorialText.SetActive(false);
    }
}
