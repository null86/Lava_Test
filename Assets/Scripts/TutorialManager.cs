using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;

public class TutorialManager : MonoBehaviour {
    [SerializeField] TutorialAsset _tutorialSettings;
    [SerializeField] RectTransform pointerRectTransform;
    [SerializeField] GameObject _arrowAboveObjectPrefab;
    [SerializeField] Transform _player;
    private Vector3[] _tutorialPositions;
    private Vector3 targetPosition;
    private RectTransform objectPointer;
    private Image pointerImage;
    private int _positionsCount = -1;
    List<RectTransform> _arrowsAboveObjects = new List<RectTransform>();
    bool _offScreenFlag = false, _onScreenFlag = false;
    float borderSize = 50f;

    private void Start() {
        _tutorialPositions = _tutorialSettings._tutorialStepsPositions;
        if (PlayerPrefs.HasKey("TutorialPassed")) { Destroy(gameObject); }
        foreach (Vector3 tutorialPos in _tutorialPositions) {
            _arrowsAboveObjects.Add(Instantiate(_arrowAboveObjectPrefab, tutorialPos, Quaternion.identity).transform.GetChild(0).GetChild(0).GetComponent<RectTransform>());
        }
        pointerImage = pointerRectTransform.GetComponent<Image>();
        ShowNext();
    }

    private void Update() {
        Vector3 targetPositionScreenPoint = Camera.main.WorldToScreenPoint(targetPosition);
        objectPointer = _arrowsAboveObjects[_positionsCount];
        bool isOffScreen = targetPositionScreenPoint.x <= borderSize || targetPositionScreenPoint.x >= Screen.width - borderSize
            || targetPositionScreenPoint.y <= borderSize || targetPositionScreenPoint.y >= Screen.height - borderSize;
        if (isOffScreen) {
            RotatePointerTowardsTargetPosition();
            Vector3 cappedTargetScreenPosition = targetPositionScreenPoint;
            if (cappedTargetScreenPosition.x <= borderSize) cappedTargetScreenPosition.x = borderSize;
            if (cappedTargetScreenPosition.x >= Screen.width - borderSize) cappedTargetScreenPosition.x = Screen.width - borderSize;
            if (cappedTargetScreenPosition.y <= borderSize) cappedTargetScreenPosition.y = borderSize;
            if (cappedTargetScreenPosition.y >= Screen.height - borderSize) cappedTargetScreenPosition.y = Screen.height - borderSize;
            pointerRectTransform.anchoredPosition = new Vector3(cappedTargetScreenPosition.x, cappedTargetScreenPosition.y, 0f);
            if (!_offScreenFlag) {
                _offScreenFlag = true;
                _onScreenFlag = false;
                pointerImage.rectTransform.DOScale(new Vector3(1f, 1f, 1f), 0.1f);
                objectPointer.DOScale(Vector3.zero, 0.1f);
            }
        } else {
            if (Vector3.Distance(_player.position, targetPosition) < 1f) {
                objectPointer.DOScale(Vector3.zero, 0.1f);
                if (_tutorialPositions.Length - 1 > _positionsCount) {
                    ShowNext();
                } else {
                    PlayerPrefs.SetInt("TutorialPassed", 1);
                    PlayerPrefs.Save();
                    Destroy(gameObject);
                }
            }
            if (!_onScreenFlag) {
                _onScreenFlag = true;
                _offScreenFlag = false;
                pointerImage.rectTransform.DOScale(Vector3.zero, 0.1f);
                objectPointer.DOScale(new Vector3(1f, 1f, 1f), 0.1f);
                objectPointer.DOLocalMoveY(1f, 1f).SetLoops(-1, LoopType.Yoyo).SetDelay(0.1f).From(0f);
            }
        }
    }

    public void ShowNext() {
        _positionsCount++;
        targetPosition = _tutorialPositions[_positionsCount];

    }

    private void RotatePointerTowardsTargetPosition() {
        Vector3 toPosition = targetPosition;
        Vector3 fromPosition = _player.position;
        Vector3 dir = (toPosition - fromPosition).normalized;
        float angle = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;
        angle += 180f;
        if (angle < 0) angle += 360;
        pointerRectTransform.localEulerAngles = new Vector3(0, 0, angle);
    }
}
