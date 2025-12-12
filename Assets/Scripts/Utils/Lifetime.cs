using UnityEngine;

public class Lifetime : MonoBehaviour
{
    [SerializeField] float lifetime = 2f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

}
