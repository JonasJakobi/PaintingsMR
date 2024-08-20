using UnityEngine;

public class ThumbEffect : MonoBehaviour
{
    [Header("Thumb")]
    [SerializeField] private AnimationCurve speedCurve;
    [SerializeField] private float moveSpeed;
    //[SerializeField] private float thumbDestroyDelay;
    [SerializeField] private GameObject thumUpGo, thumbDownGo;

    [Header("Star")]
    [SerializeField] private float starRandomOffset;
    [SerializeField] private float starSpawnDelay, starDestroyDelay;
    [SerializeField] private Star starPrefab;

    public GameObject preferenceUpdatedText;

    private Vector3 target;

    private float maxDistanceToTarget;

    private bool spawned = false;

    private void Awake()
    {
        InvokeRepeating(nameof(SpawnStar), starSpawnDelay, starSpawnDelay);

        //Invoke(nameof(SetDespawnTrigger), thumbDestroyDelay);
    }

    private void Update()
    {
        MoveToTarget();
        if(Vector3.Distance(transform.position, target) < 0.15f){
            if(!spawned){
                SpawnPreferenceUpdatedText();
                spawned = true;
            }
        }
        if(Vector3.Distance(transform.position, target) < 0.1f)
        {
            SetDespawnTrigger();
        }
    }

    private void SpawnStar()
    {
        Vector3 randomOffset = new(Random.Range(-starRandomOffset, starRandomOffset), Random.Range(-starRandomOffset, starRandomOffset), Random.Range(-starRandomOffset, starRandomOffset));
        Instantiate(starPrefab, transform.position + randomOffset, Quaternion.identity).SetDestroyDelay(starDestroyDelay);
    }

    public void Setup(bool thumbsUp, Vector3 targetPos)
    {
        thumUpGo.SetActive(thumbsUp);
        thumbDownGo.SetActive(!thumbsUp);
        target = targetPos;
        maxDistanceToTarget = Vector3.Distance(transform.position, target);
    }

    public void MoveToTarget()
    {
        Vector3 direction = (target - transform.position).normalized;
        float currentDistance = Vector3.Distance(transform.position, target);
        float speed = speedCurve.Evaluate(currentDistance / maxDistanceToTarget) * moveSpeed;
        transform.position += speed * Time.deltaTime * direction;
    }

    private void SetDespawnTrigger()
    {
        GetComponent<Animator>().SetTrigger("Despawn");
    }
    private void SpawnPreferenceUpdatedText(){
        Instantiate(preferenceUpdatedText, transform.position, Quaternion.identity);
    }
}
