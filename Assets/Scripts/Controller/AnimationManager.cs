using UnityEngine;
using Animation;
using System.Collections;
public class AnimationManager : MonoBehaviour
{
    public static AnimationManager Instance;

    private void Awake() {
        Instance = this;
    }

    public void PlayAnimation(AnimationId animationId, Vector3 originPosition, Vector3? targetPosition = null)
    {
        switch (animationId)
        {
            case AnimationId.Fireball:
                PlayFireballAnimation(originPosition, targetPosition);
                break;
            case AnimationId.LightningStrike:
                PlayLightningStrikeAnimation(originPosition, targetPosition);
                break;
            case AnimationId.Heal:
                PlayHealAnimation(originPosition, targetPosition);
                break;
            // Add more cases for other animations
        }
    }

    private void PlayFireballAnimation(Vector3 originPosition, Vector3? targetPosition)
    {
        ValidateTargetPosition(targetPosition);
        // Create a fireball prefab at the origin position
        GameObject fireball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        fireball.transform.position = originPosition;
        fireball.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

        // Add a material with a fire-like color
        Renderer renderer = fireball.GetComponent<Renderer>();
        renderer.material = new Material(Shader.Find("Standard"));
        renderer.material.color = new Color(1f, 0.5f, 0f);

        // Add a trail renderer for a fire effect
        TrailRenderer trail = fireball.AddComponent<TrailRenderer>();
        trail.material = new Material(Shader.Find("Particles/Standard Unlit"));
        trail.startColor = Color.red;
        trail.endColor = Color.yellow;
        trail.time = 0.5f;
        trail.startWidth = 0.5f;
        trail.endWidth = 0.1f;

        // Animate the fireball
        StartCoroutine(AnimateFireball(fireball, targetPosition.Value));
    }

    private void PlayLightningStrikeAnimation(Vector3 originPosition, Vector3? targetPosition)
    {
        // Implement lightning strike animation logic here
    }

    private void PlayHealAnimation(Vector3 originPosition, Vector3? targetPosition)
    {
        // Implement heal animation logic here
    }

    private void ValidateTargetPosition(Vector3? targetPosition) {
        if (targetPosition == null) {
            Debug.LogError("Target position is null");
        }
    }

    private IEnumerator AnimateFireball(GameObject fireball, Vector3 targetPosition)
    {
        float duration = 1f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            fireball.transform.position = Vector3.Lerp(fireball.transform.position, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(fireball);
    }
}
