using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;

public class SaveManager : MonoBehaviour {
    List<ItemsGroup> _items;
    BinaryFormatter _formatter;
    string _saveFile;
    FileStream stream;

    private void Start() {
        _saveFile = Application.persistentDataPath + "/data.save";
        _formatter = new BinaryFormatter();
    }

    public void SaveData(List<ItemsGroup> _items) {
        stream = new FileStream(_saveFile, FileMode.Create);
        _formatter.Serialize(stream, _items);
        stream.Close();
    }

    public void LoadData(Action<List<ItemsGroup>> _callback) {
        if (File.Exists(_saveFile)) {
            stream = new FileStream(_saveFile, FileMode.Open);
            _items = _formatter.Deserialize(stream) as List<ItemsGroup>;
            stream.Close();
            foreach (ItemsGroup _item in _items) {
                _item.SetItemPrefab(Resources.Load(_item.GetItemPrefabName()) as GameObject);
            }
            _callback(_items);
        } else {
            Debug.Log("Save file doesn't exist");
        }
    }
}
