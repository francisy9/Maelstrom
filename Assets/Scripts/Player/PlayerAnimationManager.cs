using Mirror;
using UnityEngine;
using Animation;
using System;

public class PlayerAnimationManager : NetworkBehaviour
{
    public void PlayAnimation(AnimationId animationId, Vector3 originPosition, Vector3[] affectedUnitPositions, Action onAnimationFinished) {
        AnimationManager.Instance.PlayAnimation(animationId, originPosition, affectedUnitPositions, onAnimationFinished);
    }
}
