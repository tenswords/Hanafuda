using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EstablishRoleImage : MonoBehaviour {

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private FieldManager fieldManager;

    [SerializeField]
    private Image image;
    //[SerializeField]
    //private Animation animation;

    private int establishRoleIndex;
    private int flushCount;
    private string roleName;

    public void SetEstablishRoleFlushCount(int count) {
        flushCount = count;
    }
    public void SetRoleName(string name) {
        roleName = name;
    }

    void OnEnable() {
        image.sprite = fieldManager.roleNameImage_Dic[roleName];
        //animation.Play();
    }
    void OnDisable() {
        
    }

    public void EndAnimation() {

        fieldManager.establishRoleIndex++;
        Debug.Log("役アニメーション終了");

        if (fieldManager.establishRoleIndex == flushCount) {
            fieldManager.EstablishRole_EndAnimation();

        } else {
            gameObject.SetActive(false);
            fieldManager.PlayEstablishRoleAnimation();
        }
    }
}
