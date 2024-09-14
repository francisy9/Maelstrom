using UnityEngine;
using Animation;
using System.Collections;
using System;
public class AnimationManager : MonoBehaviour
{
    public static AnimationManager Instance;
    private Fireball fireballPlayer;

    private void Awake() {
        Instance = this;
        fireballPlayer = GetComponentInChildren<Fireball>();
    }

    public void PlayAnimation(AnimationId animationId, Vector3 originPosition, Vector3[] affectedUnitPositions, Action onAnimationFinished)
    {
        switch (animationId)
        {
            case AnimationId.Fireball:
                fireballPlayer.PlayAnimation(originPosition, affectedUnitPositions, onAnimationFinished);
                break;
            case AnimationId.LightningStrike:
                PlayLightningStrikeAnimation(originPosition, affectedUnitPositions, onAnimationFinished);
                break;
            case AnimationId.Heal:
                PlayHealAnimation(originPosition, affectedUnitPositions, onAnimationFinished);
                break;
            default:
                Debug.LogWarning($"Animation {animationId} not found");
                onAnimationFinished?.Invoke();
                break;
            // Add more cases for other animations
        }
    }

    private void PlayLightningStrikeAnimation(Vector3 originPosition, Vector3[] affectedUnitPositions, Action onAnimationFinished)
    {
        throw new NotImplementedException();
    }

    private void PlayHealAnimation(Vector3 originPosition, Vector3[] affectedUnitPositions, Action onAnimationFinished)
    {
        throw new NotImplementedException();
    }
}
