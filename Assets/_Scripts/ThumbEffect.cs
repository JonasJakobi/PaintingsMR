using UnityEngine;

public class ThumbEffect : MonoBehaviour
{
    [Header("Thumb")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float constMoveUpSpeed = 0.1f, thumbDestroyDelay;
    [SerializeField] private GameObject thumUpGo, thumbDownGo;

    [Header("Star")]
    [SerializeField] private float starRandomOffset;
    [SerializeField] private float starSpawnDelay, starDestroyDelay;
    [SerializeField] private Star starPrefab;

    private Vector3 eyePos;

    private void Awake()
    {
        InvokeRepeating(nameof(SpawnStar), starSpawnDelay, starSpawnDelay);
        Invoke(nameof(SetDespawnTrigger), thumbDestroyDelay);
    }

    private void Update()
    {
        MoveAwayFromEyePos();
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
        eyePos = centerEyeTransform.position;
    }

    public void MoveAwayFromEyePos()
    {
        Vector3 direction = (transform.position - eyePos).normalized;
        //always move up
        direction.y = constMoveUpSpeed;
        transform.position += moveSpeed * Time.deltaTime * direction;
    }

    private void SetDespawnTrigger()
    {
        GetComponent<Animator>().SetTrigger("Despawn");
    }
}
