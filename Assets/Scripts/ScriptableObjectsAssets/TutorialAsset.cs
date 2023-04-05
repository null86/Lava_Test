using UnityEngine;

[CreateAssetMenu(fileName = "New Tutorial Settings", menuName = "Tutorial Settings")]
public class TutorialAsset : ScriptableObject {
    public Vector3[] _tutorialStepsPositions;
}