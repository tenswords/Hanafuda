using UnityEngine;
using System.Collections;

public class NoLodingManager : MonoBehaviour {

    private bool isTap;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

        if (Input.GetMouseButtonUp(0) && !isTap) {
            isTap = true;
            FadeManager.Instance.LoadLevel(SceneName.SceneNameManager.SCENE_NAME_TITLE, GameManager.Instance.sceneChangeInterval);
        }
    }
}
