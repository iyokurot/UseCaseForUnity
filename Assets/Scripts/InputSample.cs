using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputSample : MonoBehaviour {
	private Vector3 tapStartPos;
	private Vector3 tapEndPos;
	private GameObject stockTapObject;
	[Header ("ホールドした際のポジションずれの許容範囲")]
	[SerializeField]
	float holdLength = 15;
	[Header ("タップかスワイプかの判定距離")]
	[SerializeField]
	float tapswipeLength = 100;

	float holdTime = 0;
	// Start is called before the first frame update
	void Start () {

	}

	// Update is called once per frame
	void Update () {
		//たっぷした際
		if (Input.GetMouseButtonDown (0)) {
			tapStartPos = new Vector3 (Input.mousePosition.x,
				Input.mousePosition.y,
				Input.mousePosition.z);
			//タップされたオブジェクトの取得
			StockTapObject ();
			//エフェクト他
			return;
		} else if (Input.GetMouseButton (0)) {
			tapEndPos = new Vector3 (Input.mousePosition.x,
				Input.mousePosition.y,
				Input.mousePosition.z);

			//! 方向チェック(ホールド判定)
			GetDirection ();
			return;
		}

		//! リリースした際の処理
		if (Input.GetMouseButtonUp (0)) {
			tapEndPos = new Vector3 (Input.mousePosition.x,
				Input.mousePosition.y,
				Input.mousePosition.z);
			// Debug.Log(tapStartPos);
			// Debug.Log(tapEndPos);
			//タップorフリック判定
			GetTapOrSwipe ();
			//オブジェクトの各処理
			//targetObjectリセット
			//ホールド時間リセット
			Debug.Log (holdTime);
			holdTime = 0;
			//
			return;
		}
	}

	void GetDirection () {

		float dX = tapEndPos.x - tapStartPos.x;
		float dY = tapEndPos.y - tapStartPos.y;

		//! 左右チェック
		if (Mathf.Abs (dY) < Mathf.Abs (dX)) {
			float absdX = Mathf.Abs (dX);
			if (absdX > holdLength) {
				// inputState = INPUT_STATE.DRAG;
				// dragState = (Mathf.Sign (dX) > 0) ? DRAG_STATE.RIGHT : DRAG_STATE.LEFT;
			}
		}

		//! 上下チェック
		else if (Mathf.Abs (dX) < Mathf.Abs (dY)) {
			float absdY = Mathf.Abs (dY);
			if (absdY > holdLength) {
				// inputState = INPUT_STATE.DRAG;
				// dragState = (Mathf.Sign (dY) > 0) ? DRAG_STATE.UP : DRAG_STATE.DOWN;
			}
		}

		//! そうで無い場合はホールド
		else {
			//inputState = INPUT_STATE.HOLD;
			//Debug.Log ("hold");
			//ホールドカウント増加
			holdTime += Time.deltaTime;
			//
		}
	}

	//タップかスワイプか判定
	void GetTapOrSwipe () {
		float dX = tapEndPos.x - tapStartPos.x;
		float dY = tapEndPos.y - tapStartPos.y;
		if (Mathf.Abs (dX) < tapswipeLength && Mathf.Abs (dY) < tapswipeLength) {
			Debug.Log ("tap");
			return;
		}
		if (dY < -tapswipeLength) {
			Debug.Log ("swipeDown");
		} else {
			Debug.Log ("swipeUp");
		}
		if (dX < -tapswipeLength) {
			Debug.Log ("swipeLeft");
		} else {
			Debug.Log ("swipeRight");
		}
	}

	//タップしたオブジェクト取得(単体)
	void StockTapObject () {
		tapStartPos = Input.mousePosition;
		Ray ray = Camera.main.ScreenPointToRay (tapStartPos);
		RaycastHit hit = new RaycastHit ();
		if (Physics.Raycast (ray, out hit)) {
			stockTapObject = hit.collider.gameObject;
			Debug.Log (stockTapObject);
			Debug.Log (hit.collider.gameObject.tag);
		}
	}
	//タップしたオブジェクト取得（複数
	void StockTapObjects () {
		tapStartPos = Input.mousePosition;
		Ray ray = Camera.main.ScreenPointToRay (tapStartPos);
		//ヒットしたすべてのオブジェクト情報を取得
		foreach (RaycastHit hit in Physics.RaycastAll (ray)) {
			//ヒットしたオブジェクトの名前
			Debug.Log (hit.transform.name);
		}
	}
}