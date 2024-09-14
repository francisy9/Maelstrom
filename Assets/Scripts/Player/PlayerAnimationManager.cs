using Mirror;
using UnityEngine;
using Animation;
using System.Collections.Generic;

public class PlayerAnimationManager : NetworkBehaviour
{
    [SerializeField] private AnimationManager animationManager;

    public void PlayAnimation(AnimationId animationId, Vector3 originPosition, Vector3[] affectedUnitPositions) {
        animationManager.PlayAnimation(animationId, originPosition, affectedUnitPositions);
    }
}
