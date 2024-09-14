using UnityEngine;
using Animation;
using System.Collections;
public class AnimationManager : MonoBehaviour
{
    public static AnimationManager Instance;
    private Fireball fireballPlayer;

    private void Awake() {
        Instance = this;
        fireballPlayer = GetComponentInChildren<Fireball>();
    }

    public void PlayAnimation(AnimationId animationId, Vector3 originPosition, Vector3[] affectedUnitPositions)
    {
        switch (animationId)
        {
            case AnimationId.Fireball:
                fireballPlayer.PlayAnimation(originPosition, affectedUnitPositions);
                break;
            case AnimationId.LightningStrike:
                PlayLightningStrikeAnimation(originPosition, affectedUnitPositions);
                break;
            case AnimationId.Heal:
                PlayHealAnimation(originPosition, affectedUnitPositions);
                break;
            default:
                Debug.LogWarning($"Animation {animationId} not found");
                break;
            // Add more cases for other animations
        }
    }

    private void PlayLightningStrikeAnimation(Vector3 originPosition, Vector3[] affectedUnitPositions)
    {
        // Implement lightning strike animation logic here
    }

    private void PlayHealAnimation(Vector3 originPosition, Vector3[] affectedUnitPositions)
    {
        // Implement heal animation logic here
    }
}
