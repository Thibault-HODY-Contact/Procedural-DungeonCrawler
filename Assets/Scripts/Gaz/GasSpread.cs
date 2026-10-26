
using UnityEngine;

public class GasSpread : MonoBehaviour
{
    [Header("Propagation")]
    [SerializeField] private float maxRadius = 15f;
    [SerializeField] private float spreadDuration = 30f;

    [Header("Particle System")]
    [SerializeField] private ParticleSystem gasParticles;

    private float elapsedTime;
    private bool isSpreading;

    private void Awake()
    {
        if (gasParticles == null)
            gasParticles = GetComponentInChildren<ParticleSystem>();

        if (gasParticles == null)
        {
            Debug.LogError("GasSpread : aucun Particle System trouvé.", this);
            enabled = false;
        }
    }

    private void Start()
    {
        StartSpread();
    }

    public void StartSpread()
    {
        if (gasParticles == null)
            return;

        elapsedTime = 0f;
        isSpreading = true;

        ParticleSystem.ShapeModule shape = gasParticles.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.5f;

        gasParticles.Play(true);

        Debug.Log("Propagation du gaz démarrée.", this);
    }

    private void Update()
    {
        if (!isSpreading || gasParticles == null)
            return;

        elapsedTime += Time.deltaTime;

        float progress = spreadDuration > 0f
            ? Mathf.Clamp01(elapsedTime / spreadDuration)
            : 1f;

        ParticleSystem.ShapeModule shape = gasParticles.shape;
        shape.radius = Mathf.Lerp(0.5f, maxRadius, progress);

        if (progress >= 1f)
            isSpreading = false;
    }
}
