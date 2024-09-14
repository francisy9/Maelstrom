using Mirror;
using UnityEngine;
using Animation;

public class PlayerAnimationManager : NetworkBehaviour
{
    [SerializeField] private AnimationManager animationManager;

    public void PlayAnimation(AnimationId animationId, int originBoardIndex, int? targetBoardIndex = null) {
        Vector3 origin = new Vector3(originBoardIndex, 0, 0);
        Vector3? target = targetBoardIndex.HasValue ? new Vector3(targetBoardIndex.Value, 0, 0) : (Vector3?)null;   
        animationManager.PlayAnimation(animationId, origin, target);
    }
}
