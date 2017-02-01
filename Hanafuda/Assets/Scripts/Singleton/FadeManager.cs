using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

/// <summary>
/// シーン遷移時のフェードイン・アウトを制御するためのクラス .
/// </summary>
public class FadeManager : SingletonMonoBehaviour<FadeManager>
{
	/// <summary>
	/// デバッグモード .
	/// </summary>
	public bool DebugMode = true;
	/// <summary>フェード中の透明度</summary>
	private float fadeAlpha = 0;
	/// <summary>フェード中かどうか</summary>
	private bool isFading = false;
	/// <summary>フェード色</summary>
	public Color fadeColor = Color.black;

	private float defaultInterval = 0.5f;
	private Color defaultColor = new Color(0, 0, 0);


	public void Awake ()
	{
		if (this != Instance) {
			Destroy (this.gameObject);
			return;
		}
		
		DontDestroyOnLoad (this.gameObject);
	}

	public void OnGUI()
	{
		if (!this.isFading)
			return;

		//透明度を指定色に設定して描画
		fadeColor.a = fadeAlpha;
		GUI.color = fadeColor;
		GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
	}

	/// <summary>
	/// フェードしてシーン遷移(簡易)
	/// </summary>
	/// <param name="scene">シーンの名前</param>
	/// <param name="interval">シーン遷移の秒数</param>
	public void LoadLevel(string scene, float interval = 0.0f)
	{
		//フェード中なら実行しない
		if (isFading)
			return;

		//フェード時間が0秒なら
		interval = (interval == 0.0f) ? defaultInterval : interval;
		fadeColor = defaultColor;
		StartCoroutine(TransScene(scene, interval));
	}

	/// <summary>
	/// フェードしてシーン遷移(詳細)
	/// </summary>
	/// <param name="scene">シーンの名前</param>
	/// <param name="interval">シーン遷移の秒数</param>
	/// <param name="fcolor">フェード色</param>
	public void LoadLevel(string scene, float interval, Color fcolor)
	{
		//フェード中なら実行しない
		if (isFading)
			return;

		fadeColor = fcolor;
		StartCoroutine(TransScene(scene, interval));
	}

	/// <summary>
	/// シーン遷移用コルーチン .
	/// </summary>
	/// <param name='scene'>シーン名</param>
	/// <param name='interval'>暗転にかかる時間(秒)</param>
	private IEnumerator TransScene (string scene, float interval)
	{
		//だんだん暗く .
		this.isFading = true;
		float time = 0;
		while (time <= interval) {
			this.fadeAlpha = Mathf.Lerp (0f, 1f, time / interval);      
			time += Time.deltaTime;
			yield return 0;
		}
		
		//シーン切替 .
		//Application.LoadLevel (scene);

        SceneManager.LoadScene(scene);

		//だんだん明るく .
		time = 0;
		while (time <= interval) {
			this.fadeAlpha = Mathf.Lerp (1f, 0f, time / interval);
			time += Time.deltaTime;
			yield return 0;
		}
		
		this.isFading = false;
	}
}

