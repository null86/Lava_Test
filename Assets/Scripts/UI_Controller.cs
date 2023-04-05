using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class UI_Controller : MonoBehaviour {
    [SerializeField] GameObject _countTextPrefab;
    struct ItemView {
        public TextMeshProUGUI _countText;
        public string _itemName;
        public GameObject _itemObj;
        public ItemView(TextMeshProUGUI _text, string _name, GameObject itemObj) {
            _countText = _text;
            _itemName = _name;
            _itemObj = itemObj;
        }
    }
    List<ItemView> _itemsViews = new List<ItemView>();
    float aspectRatio;

    private void Start() {
        aspectRatio = (float)Screen.width / (float)Screen.height;
    }

    public void ShowNewItem(ItemsGroup item) {
        GameObject _tempItem = Instantiate(item.GetItemPrefab(), transform);
        GameObject _tempText = Instantiate(_countTextPrefab, transform);
        _tempItem.GetComponent<Rigidbody>().isKinematic = true;
        _tempItem.GetComponent<Collider>().enabled = false;
        _tempItem.transform.localRotation = Quaternion.Euler(-40f, 0f, 0f);
        _tempItem.transform.localScale = Vector3.zero;
        _tempItem.transform.GetChild(0).gameObject.layer = 5;
        _tempItem.transform.localPosition = new Vector3(aspectRatio * 20f - 2f, 18f - 2f * _itemsViews.Count, 3f);
        _tempItem.transform.DOScale(new Vector3(4, 4, 4), 0.5f);
        _tempItem.transform.DOLocalRotate(new Vector3(0, 360, 0), 2f, RotateMode.LocalAxisAdd).SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear);
        _tempText.transform.localPosition = new Vector3(aspectRatio * 20f - 3.5f, 18f - 2f * _itemsViews.Count, 3f);
        _tempText.transform.localScale = Vector3.zero;
        _tempText.transform.DOScale(new Vector3(1, 1, 1), 0.5f);
        _tempText.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = item.GetItemCount().ToString();
        _itemsViews.Add(new ItemView(_tempText.transform.GetChild(0).GetComponent<TextMeshProUGUI>(), item.GetItemName(), _tempItem));
    }

    public void UpdateItemsCount(ItemsGroup item) {
        ItemView _itemView = GetItemViewByName(item.GetItemName());
        _itemView._countText.text = item.GetItemCount().ToString();
        _itemView._countText.rectTransform.DOShakePosition(0.1f, 0.1f);
        _itemView._countText.rectTransform.DOShakeScale(0.1f, 0.2f);
        Loom.QueueOnMainThread(() => {
            _itemView._countText.rectTransform.anchoredPosition = Vector3.zero;
            _itemView._countText.rectTransform.localScale = new Vector3(1, 1, 1);
        }, 0.1f);
    }

    public void RemoveItem(ItemsGroup item) {
        ItemView _itemView = GetItemViewByName(item.GetItemName());
        _itemView._countText.rectTransform.DOKill();
        _itemView._itemObj.transform.DOKill();
        _itemView._itemObj.transform.DOScale(Vector3.zero, 0.1f);
        _itemView._countText.transform.DOScale(Vector3.zero, 0.1f);
        Loom.QueueOnMainThread(() => {
            Destroy(_itemView._itemObj);
            Destroy(_itemView._countText.transform.parent.gameObject);
        }, 0.1f);
        _itemsViews.Remove(_itemView);
        ResortItemOnUI();
    }

    public void ResortItemOnUI() {
        for (int i = 0; i < _itemsViews.Count; i++) {
            _itemsViews[i]._countText.transform.parent.DOLocalMove(new Vector3(aspectRatio * 20f - 3.5f, 18f - 2f * i, 3f), 0.1f);
            _itemsViews[i]._itemObj.transform.DOLocalMove(new Vector3(aspectRatio * 20f - 2f, 18f - 2f * i, 3f), 0.1f);
        }
    }

    private ItemView GetItemViewByName(string name) {
        return _itemsViews.Find(x => x._itemName.Contains(name));
    }
}
