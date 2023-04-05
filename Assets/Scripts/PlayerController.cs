using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour {
    [SerializeField] PlayerAsset _playerSettings;
    [SerializeField] UI_Controller _uiController;
    [SerializeField] SaveManager _saveManager;
    [SerializeField] float _resourceSpawnPower;
    ThirdPersonControllerAI _controllerAI;
    SourceScript _tempSource;
    List<ItemsGroup> _items = new List<ItemsGroup>();
    float _miningRateTempValue;
    float _pickUpItemsDelay, _droppingItemsRange,
        _droppingItemsRate, _droppedItemGoToSpotRate;
    private bool _hitCooldown = false;

    private void Start() {
        InitSettings();
        _controllerAI = transform.parent.GetComponent<ThirdPersonControllerAI>();
        StartCoroutine(LoadData());
    }

    private void InitSettings() {
        _droppingItemsRange = _playerSettings._droppingItemsRange;
        _droppingItemsRate = _playerSettings._droppingItemsRate;
        _pickUpItemsDelay = _playerSettings._pickUpItemsDelay;
        _droppedItemGoToSpotRate = _playerSettings._droppedItemGoToSpotRate;
        GetComponent<SphereCollider>().radius = _playerSettings._pickUpItemsRange;
    }

    private void FixedUpdate() {
        if (_tempSource != null) {
            if (_tempSource.GetAvailableForMiningState() && !_hitCooldown) {
                _hitCooldown = true;
                Loom.QueueOnMainThread(() => { _hitCooldown = false; }, _miningRateTempValue);
                MiningTargetSource();
            }
        }
        if (_isSpotEntered && _controllerAI.GetSpeed() < 0.1f) {
            _isSpotEntered = false;
            ItemsGroup _tempItem = GetItemByName(_tempSpot.GetInItem().GetComponent<ResourceScript>().GetResourceName());
            if (_tempItem != null) {
                if (_tempItem.GetItemCount() >= _tempSpot.GetInItemCount()) {
                    DropResource(_tempItem, _tempSpot.GetInItemCount(), _tempSpot.gameObject);
                }
            }
        }
    }

    #region Source Interaction
    public void ResourceSourceEnter(SourceScript _src) {
        _tempSource = _src;
        _miningRateTempValue = _src.GetMiningRateValue();
        _controllerAI.GetAnimator().SetFloat("AttackSpeed", 1 / _miningRateTempValue * 2.267f);
    }

    public void ResourceSourceExit() {
        _tempSource = null;
        _controllerAI.GetAnimator().Play("Idle Walk Run Blend");
        _saveManager.SaveData(_items);
    }

    private void MiningTargetSource() {
        if (_controllerAI.GetSpeed() > 0.1f) return;
        _controllerAI.GetAnimator().Play("Attack");
        Vector3 lookPos = _tempSource.transform.position - transform.parent.position;
        lookPos.y = 0;
        transform.parent.rotation = Quaternion.LookRotation(lookPos);
    }

    public void OnAttack() {
        _tempSource.GetHit();
    }
    #endregion

    #region Resource Interaction

    private void OnTriggerEnter(Collider other) {
        if (other.GetComponent<ResourceScript>()) {
            if (!other.GetComponent<ResourceScript>().GetPickableState())
                return;
            ResourceScript item = other.GetComponent<ResourceScript>();
            item.GoToPlayerOrSpot(gameObject, 0.2f, _pickUpItemsDelay);
            Loom.QueueOnMainThread(() => { PickUpItem(item); }, _pickUpItemsDelay);
        }
    }

    private void PickUpItem(ResourceScript item) {
        if (GetItemByName(item.GetResourceName()) != null) {
            ItemsGroup _tempItem = GetItemByName(item.GetResourceName());
            _tempItem.Increase();
            _uiController.UpdateItemsCount(_tempItem);
            _saveManager.SaveData(_items);
            return;
        }
        ItemsGroup _newTempItem = new ItemsGroup(item.GetResourceName(), item.GetPrefabRef());
        _items.Add(_newTempItem);
        _uiController.ShowNewItem(_newTempItem);
        _saveManager.SaveData(_items);
    }

    private void DropResource(ItemsGroup _resource, int count, GameObject _targetPosition) {
        for (int i = 0; i < count; i++) {
            Loom.QueueOnMainThread(() => {
                GameObject _resourceGo = Instantiate(_resource.GetItemPrefab(), transform.position + new Vector3(0, 1.5f, 0f), Quaternion.identity);
                _resourceGo.GetComponent<Collider>().enabled = true;
                _resourceGo.GetComponent<ResourceScript>().SetPickableState(false);
                _resourceGo.GetComponent<ResourceScript>().GoToPlayerOrSpot(_targetPosition, 0.2f, _droppedItemGoToSpotRate * i, _tempSpot);
                _resourceGo.transform.localScale = Vector3.zero;
                _resourceGo.transform.DOScale(new Vector3(1, 1, 1), 0.5f);
                if (_resource.Decrease()) {
                    _uiController.RemoveItem(_resource);
                    _items.Remove(_resource);
                } else {
                    _uiController.UpdateItemsCount(_resource);
                }
                _saveManager.SaveData(_items);
            }, i * _droppingItemsRate);
        }
    }
    #endregion

    #region Spot Interaction

    SpotScript _tempSpot;
    bool _isSpotEntered = false;
    public void SpotEnter(SpotScript _spot) {
        _isSpotEntered = true;
        _tempSpot = _spot;
    }

    public void SpotExit() {
        _isSpotEntered = false;
        _saveManager.SaveData(_items);
    }
    #endregion

    #region Save / Load
    IEnumerator LoadData() {
        yield return new WaitForSeconds(0.2f);
        _saveManager.LoadData(SetData);
    }

    private void SetData(List<ItemsGroup> _data) {
        _items = _data;
        foreach (ItemsGroup _item in _items)
            _uiController.ShowNewItem(_item);
    }
    #endregion

    private ItemsGroup GetItemByName(string name) {
        foreach (ItemsGroup _item in _items) {
            if (_item.GetItemName() == name)
                return _item;
        }
        return null;
    }
}

[Serializable]
public class ItemsGroup {
    [NonSerialized] GameObject itemPrefab;
    [SerializeField] string prefabName;
    [SerializeField] string itemName;
    [SerializeField] int itemCount = 1;

    public ItemsGroup(string _name, GameObject _go) {
        prefabName = _go.name;
        itemPrefab = _go;
        itemName = _name;
    }

    public void Increase() {
        itemCount++;
    }

    public bool Decrease() {
        itemCount--;
        if (itemCount > 0) {
            return false;
        } else
            return true;
    }

    public string GetItemName() {
        return itemName;
    }

    public GameObject GetItemPrefab() {
        return itemPrefab;
    }

    public void SetItemPrefab(GameObject go) {
        itemPrefab = go;
    }

    public string GetItemPrefabName() {
        return prefabName;
    }

    public int GetItemCount() {
        return itemCount;
    }
}

