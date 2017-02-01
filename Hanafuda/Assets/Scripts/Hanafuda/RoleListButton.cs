using UnityEngine;
using System.Collections;

public class RoleListButton : MonoBehaviour {

    [SerializeField]
    private GameObject dialogCanvas;
    [SerializeField]
    private GameObject hideBlcakImage;
    [SerializeField]
    private GameObject roleList;


    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    /// <summary>
    /// 役一覧ボタンを押したときの処理
    /// </summary>
    public void OnRoleListButton() {
        dialogCanvas.SetActive(true);
        hideBlcakImage.SetActive(true);
        roleList.SetActive(true);
    }

    /// <summary>
    /// 役一覧を閉じるボタンを押したときの処理
    /// </summary>
    public void OnRoleListCloseButton() {
        dialogCanvas.SetActive(false);
        hideBlcakImage.SetActive(false);
        roleList.SetActive(false);
    }
}
