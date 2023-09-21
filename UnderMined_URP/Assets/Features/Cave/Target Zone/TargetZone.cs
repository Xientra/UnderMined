using System;
using UnityEngine;

public class TargetZone : MonoBehaviour
{
    public event EventHandler DrillReachedZone; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Drill"))
            DrillReachedZone?.Invoke(this, EventArgs.Empty);
    }
}
