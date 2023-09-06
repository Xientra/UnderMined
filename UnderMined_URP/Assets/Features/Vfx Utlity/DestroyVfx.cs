using UnityEngine;

public class DestroyVfx : MonoBehaviour
{
    public float duration = 3f;

    private void Start()
    {
        Destroy(this.gameObject, duration);
    }
}
