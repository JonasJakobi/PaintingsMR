using UnityEngine;

public class ThumbEffect : MonoBehaviour
{
    [Header("Thumb")]
    [SerializeField] private float speed;
    [SerializeField] private float thumbDestroyDelay;
    [SerializeField] private GameObject thumUpGo, thumbDownGo;

    [Header("Star")]
    [SerializeField] private float starRandomOffset;
    [SerializeField] private float starSpawnDelay, starDestroyDelay;
    [SerializeField] private Star starPrefab;

    private void Awake()
    {
        InvokeRepeating(nameof(SpawnStar), starSpawnDelay, starSpawnDelay);
        Invoke(nameof(SetDespawnTrigger), thumbDestroyDelay);
    }

    private void SpawnStar()
    {
        Vector3 randomOffset = new(Random.Range(-starRandomOffset, starRandomOffset), Random.Range(-starRandomOffset, starRandomOffset), Random.Range(-starRandomOffset, starRandomOffset));
        Instantiate(starPrefab, transform.position + randomOffset, Quaternion.identity).SetDestroyDelay(starDestroyDelay);
    }

    public void Setup(bool thumbsUp, Transform centerEyeTransform)
    {
        thumUpGo.SetActive(thumbsUp);
        thumbDownGo.SetActive(!thumbsUp);
    }

    private void SetDespawnTrigger()
    {
        GetComponent<Animator>().SetTrigger("Despawn");
    }
}
