using UnityEngine;
public class GasRenderer : MonoBehaviour
{
    [Header("Références")]
    [SerializeField] private GasSpread gas;
    [SerializeField] private Transform player;

    [Header("Affichage")]
    [SerializeField] private float renderRadius = 20f;
    [SerializeField] private int particlesPerUpdate = 30;
    [SerializeField] private float updateRate = 0.05f;

    private ParticleSystem ps;
    private float timer;

    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
    }
    public void Initialize(GasSpread spread, Transform targetPlayer)
    {
        gas = spread;
        player = targetPlayer;
    }
    private void Update()
    {
        if (gas == null || player == null)
            return;

        float distance = Vector3.Distance(player.position, gas.transform.position);

        if (distance > gas.CurrentRadius + renderRadius)
            return;

        timer += Time.deltaTime;

        if (timer >= updateRate)
        {
            timer = 0f;
            EmitParticles();
        }
    }

    private void EmitParticles()
    {
        ParticleSystem.EmitParams emit = new ParticleSystem.EmitParams();

        for (int i = 0; i < particlesPerUpdate; i++)
        {
            Vector2 p;

            do
            {
                Vector2 random = Random.insideUnitCircle * renderRadius;

                p = new Vector2(
                    player.position.x + random.x,
                    player.position.z + random.y);

            }
            while (Vector2.Distance(
                p,
                new Vector2(gas.transform.position.x, gas.transform.position.z))
                > gas.CurrentRadius);

            emit.position = new Vector3(
                p.x,
                gas.transform.position.y,
                p.y);

            ps.Emit(emit, 1);
        }
    }
}