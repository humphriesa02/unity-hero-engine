using UnityEngine;

public class LockOnController : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private Vector3 lockOnExtents;
    [SerializeField] private float forwardOffset = 1f; // How far forward to check for lockOnTargets
    [SerializeField] private LayerMask lockOnMask;
    [SerializeField] private LayerMask obstructionMask;
    [SerializeField] private float maxLockOnDistance = 12f; // Distance a locked on target can get before dereg
    [SerializeField] private float distanceConeAngleDeg = 100f;
    
    public Transform AttemptLockOn()
    {
        Transform lockedOnTarget = FindBestLockTarget(
            player.gameObject.transform.position,
            player.gameObject.transform.forward,
            lockOnExtents,
            forwardOffset,
            lockOnMask
            );
        return lockedOnTarget;
    }

    public Transform FindBestLockTarget(
        Vector3 origin,
        Vector3 forward,
        Vector3 halfExtents,
        float forwardOffset,
        LayerMask targetMask)
    {
        forward.y = 0f;
        forward.Normalize();

        Vector3 center = origin + forward * forwardOffset;
        Quaternion orientation = Quaternion.LookRotation(forward);

        Collider[] hits = Physics.OverlapBox(center, halfExtents, orientation, targetMask);
        Transform best = null;
        float bestDistSqr = float.MaxValue;

        foreach (Collider hit in hits)
        {
            Vector3 to = hit.transform.position - origin;
            to.y = 0f;

            float distSqr = to.sqrMagnitude;
            if (distSqr < bestDistSqr)
            {
                bestDistSqr = distSqr;
                best = hit.transform;
            }
        }

        return best;
    }

    public bool CheckForTargetValidity(Transform target)
    {
        bool valid = IsValidLockOnTarget(
            target,
            player.transform.position
        );
        return valid;
    }

    public bool IsValidLockOnTarget(
        Transform target,
        Vector3 origin)
    {
        if (target == null)
            return false;

        Vector3 toTarget = target.position - origin;
        toTarget.y = 0f;

        float sqrDist = toTarget.sqrMagnitude;
        if (sqrDist > maxLockOnDistance * maxLockOnDistance)
            return false;

        if (sqrDist < 0.0001f)
            return true;

        Vector3 rayOrigin = origin + Vector3.up;
        Vector3 rayTarget = target.position + Vector3.up * 0.5f;
        Vector3 rayDir = rayTarget - rayOrigin;
        float rayDist = rayDir.magnitude;

        if (Physics.Raycast(rayOrigin, rayDir.normalized, out RaycastHit hit, rayDist, obstructionMask))
        {
            if (!hit.transform.IsChildOf(target) && hit.transform != target)
                return false;
        }

        return true;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Quaternion orientation = Quaternion.LookRotation(player.gameObject.transform.forward);
        Gizmos.matrix = Matrix4x4.TRS(player.gameObject.transform.position, orientation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.forward * forwardOffset, lockOnExtents * 2f);
    }
}
