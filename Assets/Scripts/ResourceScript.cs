using UnityEngine;
using DG.Tweening;

public class ResourceScript : MonoBehaviour {
    [SerializeField] ResourceAsset _resourceSettings;
    [SerializeField] string _resourceName;
    GameObject _assetPrefabRef;
    bool isPickableByPlayer = true;
    float _spawnPower;

    private void Start() {
        _spawnPower = _resourceSettings._spawnPower;
        PushImpulse();
    }

    private void PushImpulse() {
        Vector3 _impulseTargetPosition = new Vector3(transform.position.x + Random.Range(-_spawnPower * 2f, _spawnPower * 2f), transform.position.y + 10f, transform.position.z + Random.Range(-_spawnPower * 2f, _spawnPower * 2f));
        Rigidbody _rigidbody = GetComponent<Rigidbody>();
        Vector3 _direction = _impulseTargetPosition - transform.position;
        _rigidbody.AddForce(_direction.normalized * _spawnPower, ForceMode.Impulse);
        _rigidbody.AddTorque(new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * _spawnPower);
    }

    public void GoToPlayerOrSpot(GameObject player, float time, float delay, SpotScript _spot = null) {
        isPickableByPlayer = false;
        transform.DOScale(0f, time).SetDelay(delay);
        Loom.QueueOnMainThread(() => {
            GetComponent<Collider>().enabled = false;
            transform.DOMove(player.transform.position + new Vector3(0, 1f, 0f), time);
        }, delay);
        Loom.QueueOnMainThread(() => {
            if (_spot != null)
                _spot.GetResource();
            Destroy(gameObject);
        }, delay + time + 0.2f);
    }

    public bool GetPickableState() {
        return isPickableByPlayer;
    }

    public void SetPickableState(bool state) {
        isPickableByPlayer = state;
    }

    public string GetResourceName() {
        return _resourceName;
    }

    public void SetPrefabRef(GameObject _ref) {
        _assetPrefabRef = _ref;
    }

    public GameObject GetPrefabRef() {
        return _assetPrefabRef;
    }
}
