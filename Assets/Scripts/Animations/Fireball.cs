using UnityEngine;

public class Fireball : ProjectileAnimationBase
{
    private void Awake()
    {
        if (projectilePrefab == null)
        {
            projectilePrefab = CreateProjectilePrefab();
        }
        projectilePrefab.SetActive(false);
    }

    protected override GameObject CreateProjectilePrefab() {
        GameObject currentAnimationObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        currentAnimationObject.transform.localScale = new Vector3(20f, 20f, 2f);

        Renderer renderer = currentAnimationObject.GetComponent<Renderer>();
        renderer.material = new Material(Shader.Find("Standard"));
        Color fireballColor = new Color(1f, 0.3f, 0f, 0.7f);
        renderer.material.SetColor("_Color", fireballColor);
        renderer.material.EnableKeyword("_EMISSION");
        renderer.material.SetColor("_EmissionColor", fireballColor * 2f);

        TrailRenderer trail = currentAnimationObject.AddComponent<TrailRenderer>();
        trail.material = new Material(Shader.Find("Particles/Standard Unlit"));
        trail.startWidth = 40f;
        trail.endWidth = 35f;
        trail.time = 0.2f;
        trail.minVertexDistance = 0.05f;

        Gradient trailGradient = new Gradient();
        trailGradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(fireballColor, 0f), new GradientColorKey(Color.clear, 1f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0f, 1f) }
        );
        trail.colorGradient = trailGradient;

        currentAnimationObject.AddComponent<TrailRendererModifier>();

        Light light = currentAnimationObject.AddComponent<Light>();
        light.type = LightType.Point;
        light.color = fireballColor;
        light.intensity = 3f;
        light.range = 10f;

        ParticleSystem particles = currentAnimationObject.AddComponent<ParticleSystem>();
        var main = particles.main;
        main.startLifetime = 0.2f;
        main.startSpeed = 2f;
        main.startSize = 0.5f;
        main.simulationSpace = ParticleSystemSimulationSpace.Local;
        main.startColor = Color.grey;

        var emission = particles.emission;
        emission.rateOverTime = 20;

        var shape = particles.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.5f;

        var colorOverLifetime = particles.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient particleGradient = new Gradient();
        particleGradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(Color.grey, 0f), new GradientColorKey(Color.grey, 1f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0f, 1f) }
        );
        colorOverLifetime.color = particleGradient;

        var textureSheetAnimation = particles.textureSheetAnimation;
        textureSheetAnimation.enabled = true;
        textureSheetAnimation.numTilesX = 1;
        textureSheetAnimation.numTilesY = 1;

        var sizeOverLifetime = particles.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, 0f);
        var particleRenderer = particles.GetComponent<ParticleSystemRenderer>();
        particleRenderer.material = new Material(Shader.Find("Particles/Standard Unlit"));
        particleRenderer.material.color = Color.grey;

        return currentAnimationObject;
    }
    protected override GameObject GetProjectilePrefab()
    {
        return projectilePrefab;
    }
    public class TrailRendererModifier : MonoBehaviour
    {
        private TrailRenderer trail;
        private Vector3 lastPosition;

        void Start()
        {
            trail = GetComponent<TrailRenderer>();
            lastPosition = transform.position;
        }

        void Update()
        {
            Vector3 randomOffset = new Vector3(
                Random.Range(-8f, 8f),
                Random.Range(-0.2f, 0f),
                Random.Range(-8f, 8f)
            );
            trail.AddPosition(lastPosition + randomOffset);
            lastPosition = transform.position;
        }
    }
}
