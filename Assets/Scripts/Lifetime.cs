using UnityEngine;
using UnityEngine.Events;

public class Lifetime : MonoBehaviour
{
    [SerializeField, Tooltip("Time until the object is removed")] 
    float lifetime;
    float currentLife = 0.0f;

    [SerializeField, Tooltip("Whether to start the lifetime onStart")]
    bool onStart;
    bool hasStarted;

    [SerializeField, Tooltip("An event that will be completed on lifetime end.")]
    UnityEvent lifetimeEndsEvent;

    void Start()
    {
        if (onStart) StartLifetime();
    }

    void Update()
    {
        if (!hasStarted) return;
        if(currentLife < lifetime)
        {
            currentLife += Time.deltaTime;
        }
        else
        {
            lifetimeEndsEvent.Invoke();
            Destroy(gameObject);
        }
    }

    public void StartLifetime()
    {
        hasStarted = true;
    }
}
