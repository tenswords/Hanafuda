using UnityEngine;
//using System.Collections;

public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour {
	
	private static T _instance;

	public static T Instance {
		
		get {
			if (_instance == null) {
				
				//シーン上からを取得する
				_instance = FindObjectOfType<T> ();
				
				if (_instance == null) {
					Debug.LogError (typeof(T) + " is nothing");
				}
			}
			
			return _instance;
		}
	}
}
