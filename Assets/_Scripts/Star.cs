using UnityEngine;

public class Star : MonoBehaviour
{
    public void SetDestroyDelay(float destroyDelay)
    {
        Invoke(nameof(SetDespawnTrigger), destroyDelay);
    }

    private void SetDespawnTrigger()
    {
        GetComponent<Animator>().SetTrigger("Despawn");
    }
}
