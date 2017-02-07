using UnityEngine;
using System.Collections;

public class Manager : SingletonMonoBehaviour<Manager> {
    void Awake() {
        if (this != Instance) {
            Destroy(this.gameObject);
            return;
        }

        DontDestroyOnLoad(this.gameObject);

        Debug.Log("フレームレート 変更前" + Application.targetFrameRate);
        Application.targetFrameRate = 60;
        Debug.Log("フレームレート 変更後" + Application.targetFrameRate);
    }
}
