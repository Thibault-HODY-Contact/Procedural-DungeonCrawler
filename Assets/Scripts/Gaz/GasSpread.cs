using UnityEngine;

public class GasSpread : MonoBehaviour
{
    [Header("Propagation")]
    [SerializeField] private float maxRadius = 15f;
    [SerializeField] private float spreadDuration = 30f;

    [Header("Gaz")]
    [SerializeField] private GameObject gasEmitterPrefab;
    [SerializeField] private Transform player;
    [SerializeField] private float playerRenderRadius = 8f;
    [SerializeField] private float emitterRadius = 6f;

    private GameObject currentEmitter;
    private bool isSpreading = false;
    private float elapsedTime;

    public float CurrentRadius { get; private set; } = 0f;

    public void Initialize(Transform playerTransform)
    {
        player = playerTransform;
    }

    public void StartSpread()
    {
        elapsedTime = 0f;
        CurrentRadius = 0.5f;
        isSpreading = true;

        if (currentEmitter != null)
        {
            Destroy(currentEmitter);
            currentEmitter = null;
        }
    }

    public void StopSpread()
    {
        // Arrête la propagation sans détruire le gaz.
        isSpreading = false;
    }

    private void Update()
    {
        if (!isSpreading)
            return;

        elapsedTime += Time.deltaTime;

        float progress = spreadDuration > 0f
            ? Mathf.Clamp01(elapsedTime / spreadDuration)
            : 1f;

        CurrentRadius = Mathf.Lerp(0.5f, maxRadius, progress);

        UpdateRenderer();
    }

    private void UpdateRenderer()
    {
        if (player == null || gasEmitterPrefab == null)
            return;

        Vector3 direction = player.position - transform.position;
        float distance = direction.magnitude;

        bool shouldRender = distance <= CurrentRadius + playerRenderRadius;

        if (shouldRender)
        {
            Vector3 emitterPosition;

            if (distance <= CurrentRadius - emitterRadius)
            {
                emitterPosition = player.position;
            }
            else
            {
                direction.Normalize();
                emitterPosition = transform.position
                    + direction * (CurrentRadius - emitterRadius);
            }

            if (currentEmitter == null)
            {
                currentEmitter = Instantiate(
                    gasEmitterPrefab,
                    emitterPosition,
                    Quaternion.identity
                );

                ParticleSystem ps =
                    currentEmitter.GetComponent<ParticleSystem>();

                if (ps != null)
                {
                    ps.Clear(true);
                    ps.Play(true);
                }
            }

            currentEmitter.transform.position = Vector3.Lerp(
                currentEmitter.transform.position,
                emitterPosition,
                Time.deltaTime * 8f
            );
        }
        else if (currentEmitter != null)
        {
            Destroy(currentEmitter);
            currentEmitter = null;
        }
    }

    public bool IsInside(Vector3 position)
    {
        return isSpreading &&
            Vector3.Distance(transform.position, position) <= CurrentRadius;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        DrawCircle(transform.position, CurrentRadius);

        if (player != null)
        {
            Gizmos.color = Color.red;
            DrawCircle(player.position, playerRenderRadius);
        }
    }

    private void DrawCircle(Vector3 center, float radius, int segments = 64)
    {
        float angleStep = 360f / segments;
        Vector3 previousPoint = center + new Vector3(radius, 0f, 0f);

        for (int i = 1; i <= segments; i++)
        {
            float angle = angleStep * i * Mathf.Deg2Rad;
            Vector3 newPoint = center + new Vector3(
                Mathf.Cos(angle) * radius,
                0f,
                Mathf.Sin(angle) * radius);

            Gizmos.DrawLine(previousPoint, newPoint);
            previousPoint = newPoint;
        }
    }
}