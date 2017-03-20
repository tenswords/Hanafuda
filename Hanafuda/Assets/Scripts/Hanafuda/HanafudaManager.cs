using UnityEngine;
using System.Collections;

public class HanafudaManager : MonoBehaviour {

    [SerializeField]
    private GameObject hideBlackImage;

    [SerializeField]
    private Result result;

    [SerializeField]
    private FieldManager fieldManager;

    [SerializeField]
    private ScoreManager scoreManager;

    void Awake() {
        //セーブデータがある場合、保存されたデータを読み込む
        if (SaveLoadManager.Instance.CheckHasSaveData() &&
            SaveLoadManager.Instance.GetSaveScene() == SceneName.SceneNameManager.SCENE_NAME_HANAFUDA) {
            scoreManager.SetResumptionScore();

            if (PlayerPrefs.GetString(SaveLoadManager.Instance.SAVE_DATA_IS_RESULT) == "true") {
                hideBlackImage.SetActive(true);
                result.SetResumptionText();

                result.SetScoreText();
                result.SetFlushListText();
                result.gameObject.SetActive(true);
                fieldManager.state = FieldManager.STATE.RESULT;
            }

        } else {
            scoreManager.SetDefaultScore();
        }
    }

	// Use this for initialization
	void Start () {


    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
