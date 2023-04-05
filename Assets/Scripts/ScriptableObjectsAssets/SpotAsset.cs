using UnityEngine;

[CreateAssetMenu(fileName = "New Spot Settings", menuName = "Spot Settings")]
public class SpotAsset : ScriptableObject {
    public GameObject _inItemPrefab;
    public GameObject _outItemPrefab;
    public int _inItemsCount = 1;
    public int _outItemsCount = 1;
    public float _outItemsDropDelay = 1;
    public float _itemDropRate = 0.1f;
}