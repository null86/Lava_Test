using StarterAssets;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SourceScript : MonoBehaviour {
    [SerializeField] SourceAsset _sourceSettings;
    [SerializeField] GameObject _resourcePrefab;
    [SerializeField] float _spawnPower;
    [SerializeField] RawImage _halo;
    [SerializeField] Transform _sourceMesh, _heatDistortion;
    int _hitsToCooldown;
    float _miningRate;
    int _oneHitDropCount;
    float _cooldownDelay;
    int _hitCountToDestroy;
    bool _isAvailableForMining = true;
    PlayerController _player;
    int _currentCycleHitsCount, _totalHitsCount;

    private void Start() {
        InitSettings();
        _halo.DOFade(1f, 1f).SetLoops(-1, LoopType.Yoyo);
    }

    private void InitSettings() {
        _hitsToCooldown = _sourceSettings._hitsToCooldown;
        _miningRate = _sourceSettings._miningRate;
        _oneHitDropCount = _sourceSettings._oneHitDropCount;
        _cooldownDelay = _sourceSettings._cooldownDelay;
        _hitCountToDestroy = _sourceSettings._hitCountToDestroy;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.GetComponent<ThirdPersonControllerAI>()) {
            if (_player == null)
                _player = other.GetComponent<ThirdPersonControllerAI>().GetPlayerController();
            _player.ResourceSourceEnter(this);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.GetComponent<ThirdPersonControllerAI>()) {
            _player.ResourceSourceExit();
        }
    }

    public void GetHit() {
        if (!_isAvailableForMining) return;
        ShowVisualHitFeedback();
        DropResource(_oneHitDropCount);
        _currentCycleHitsCount++;
        _totalHitsCount++;
        if (_totalHitsCount >= _hitCountToDestroy) {
            _isAvailableForMining = false;
            _halo.DOKill();
            _halo.DOFade(0f, 1f);
            Loom.QueueOnMainThread(() => {
                Destroy(gameObject);
            }, 1f);
            return;
        }
        if (_currentCycleHitsCount >= _hitsToCooldown) {
            _currentCycleHitsCount = 0;
            _isAvailableForMining = false;
            Loom.QueueOnMainThread(() => {
                _isAvailableForMining = true;
                _halo.DOFade(0.5f, 0.5f);
                _halo.DOFade(1f, 1f).SetDelay(0.5f).SetLoops(-1, LoopType.Yoyo);
            }, _cooldownDelay);
            _halo.DOKill();
            _halo.DOFade(0f, 1f);
        }
    }

    void DropResource(int _count) {
        for (int i = 0; i < _count; i++) {
            Loom.QueueOnMainThread(() => {
                GameObject _resourceGo = Instantiate(_resourcePrefab, transform.position + new Vector3(0, 1f, 0), Quaternion.identity);
                Vector3 _playerPos = _player.transform.position;
                _resourceGo.GetComponent<ResourceScript>().SetPrefabRef(_resourcePrefab);
            }, i * 0.1f);
        }
    }

    void ShowVisualHitFeedback() {
        _sourceMesh.DOShakePosition(0.1f, 0.1f);
        _sourceMesh.DOScale(_sourceMesh.localScale.x - (1f / _hitCountToDestroy), 0.1f).SetEase(Ease.InOutBounce);
        if (_heatDistortion != null)
            _heatDistortion.DOScale(_heatDistortion.localScale.x - (0.2f / _hitCountToDestroy), 0.1f);
    }

    public bool GetAvailableForMiningState() {
        return _isAvailableForMining;
    }

    public float GetMiningRateValue() {
        return _miningRate;
    }
}
