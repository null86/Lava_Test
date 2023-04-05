using UnityEngine;

namespace StarterAssets {
    public class UICanvasControllerInput : MonoBehaviour {

        [SerializeField]
        GameObject targetPos, _player;

        private void Update() {
            if (Input.GetAxis("Horizontal") != 0f || Input.GetAxis("Vertical") != 0f) {
                targetPos.transform.position = new Vector3(_player.transform.position.x + Input.GetAxis("Horizontal"), _player.transform.position.y, _player.transform.position.z + Input.GetAxis("Vertical"));
            }
        }
    }

}
