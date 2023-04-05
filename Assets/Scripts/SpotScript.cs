using UnityEngine;
using StarterAssets;
using TMPro;

public class SpotScript : MonoBehaviour {
    [SerializeField] SpotAsset _spotSettings;
    [SerializeField] GameObject[] ResourcePrefabPositions;
    [SerializeField] Transform _spawnPosition, inPrefabInfoPanelPos, outPrefabInfoPanelPos;
    [SerializeField] TextMeshProUGUI _inCountInfoPanelText, _outCountInfoPanelText;
    [SerializeField] float _spawnPower;
    GameObject _inItemPrefab, _outItemPrefab;
    int _inResourceCount, _outResourceCount;
    int currentInputResourceCount = 0;
    float _dropDelay, _dropRate;

    private void Start() {
        InitSettings();
        foreach (GameObject _resourcePos in ResourcePrefabPositions) {
            Vector3 _randomPrefabRot = new Vector3(Random.Range(-180, 180), Random.Range(-180, 180), 90f);
            GameObject go = Instantiate(_outItemPrefab, _resourcePos.transform.position, Quaternion.Euler(_randomPrefabRot), _resourcePos.transform);
            go.GetComponent<Rigidbody>().isKinematic = true;
            go.GetComponent<Collider>().enabled = false;
        }
        _inCountInfoPanelText.text = "x" + _inResourceCount;
        _outCountInfoPanelText.text = "x" + _outResourceCount;
        GameObject _inPrefab = Instantiate(_inItemPrefab, inPrefabInfoPanelPos.transform.position, Quaternion.identity, inPrefabInfoPanelPos.transform.parent);
        _inPrefab.GetComponent<Rigidbody>().isKinematic = true;
        _inPrefab.GetComponent<Collider>().enabled = false;
        _inPrefab.transform.GetChild(0).GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        GameObject outPrefab = Instantiate(_outItemPrefab, outPrefabInfoPanelPos.transform.position, Quaternion.identity, outPrefabInfoPanelPos.transform.parent);
        outPrefab.GetComponent<Rigidbody>().isKinematic = true;
        outPrefab.GetComponent<Collider>().enabled = false;
        outPrefab.transform.GetChild(0).GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

    }

    private void InitSettings() {
        _dropDelay = _spotSettings._outItemsDropDelay;
        _dropRate = _spotSettings._itemDropRate;
        _inItemPrefab = _spotSettings._inItemPrefab;
        _outItemPrefab = _spotSettings._outItemPrefab;
        _inResourceCount = _spotSettings._inItemsCount;
        _outResourceCount = _spotSettings._outItemsCount;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.GetComponent<ThirdPersonControllerAI>()) {
            other.GetComponent<ThirdPersonControllerAI>().GetPlayerController().SpotEnter(this);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.GetComponent<ThirdPersonControllerAI>()) {
            other.GetComponent<ThirdPersonControllerAI>().GetPlayerController().SpotExit();
        }
    }

    public void GetResource() {
        currentInputResourceCount++;
        if (currentInputResourceCount == _inResourceCount) {
            currentInputResourceCount = 0;
            Loom.QueueOnMainThread(() => {
                DropResource();
            }, _dropDelay);
        }
    }

    void DropResource() {
        for (int i = 0; i < _outResourceCount; i++) {
            Loom.QueueOnMainThread(() => {
                GameObject _resourceGo = Instantiate(_outItemPrefab, _spawnPosition.position, Quaternion.identity);
                _resourceGo.GetComponent<ResourceScript>().SetPrefabRef(_outItemPrefab);
                Vector3 _impulseTargetPosition = new Vector3(_spawnPosition.position.x - _spawnPower, 0f, transform.position.z + Random.Range(-_spawnPower, _spawnPower));
                Rigidbody _rigidbody = _resourceGo.GetComponent<Rigidbody>();
                Vector3 _direction = _impulseTargetPosition - _spawnPosition.position;
                _rigidbody.AddForce(_direction.normalized * _spawnPower, ForceMode.Impulse);
            }, i * _dropRate);
        }
    }

    public GameObject GetInItem() {
        return _inItemPrefab;
    }

    public int GetInItemCount() {
        return _inResourceCount;
    }
}
