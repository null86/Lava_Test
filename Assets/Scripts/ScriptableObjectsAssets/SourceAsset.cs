using UnityEngine;

[CreateAssetMenu(fileName = "New Resource Source Settings", menuName = "Resource Source Settings")]
public class SourceAsset : ScriptableObject {
    public int _hitsToCooldown = 6;
    public float _miningRate = 1;
    public int _oneHitDropCount = 1;
    public float _cooldownDelay = 8;
    public int _hitCountToDestroy = 12;
}
