using UnityEngine;

[CreateAssetMenu(fileName = "New Player Settings", menuName = "Player Settings")]
public class PlayerAsset : ScriptableObject {
    public float _pickUpItemsRange;
    public float _pickUpItemsDelay;
    public float _droppingItemsRange;
    public float _droppingItemsRate;
    public float _droppedItemGoToSpotRate;
}