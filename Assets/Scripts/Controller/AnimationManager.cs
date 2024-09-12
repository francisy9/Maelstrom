using UnityEngine;
using Animation;
public class AnimationManager : MonoBehaviour
{
    public static AnimationManager Instance;

    private void Awake() {
        Instance = this;
    }

    public void PlayAnimation(AnimationId animationId, Vector3 originPosition, Vector3 targetPosition)
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

    private void PlayFireballAnimation(Vector3 originPosition, Vector3 targetPosition)
    {
        // Implement fireball animation logic here
    }

    private void PlayLightningStrikeAnimation(Vector3 originPosition, Vector3 targetPosition)
    {
        // Implement lightning strike animation logic here
    }

    private void PlayHealAnimation(Vector3 originPosition, Vector3 targetPosition)
    {
        // Implement heal animation logic here
    }
}
