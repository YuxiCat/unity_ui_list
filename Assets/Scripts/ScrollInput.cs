using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollInput : MonoBehaviour {

	public List<GameObject> ScrollObjects = new List<GameObject>(7);
	public int speed = 200;
	public RectTransform Center;
	
	private List<RectTransform> RectTransforms = new List<RectTransform>();
	private List<Text> Texts = new List<Text>();
	private List<Vector3> startPositions = new List<Vector3>(7);
	private List<Color> colors = new List<Color>(7);
	private float offset;
	private int startIndex = 0;
	private float timeToReachTarget = 0.3f;
	private float keyTime = 0.0f;
	private float mouseTime = 0.0f;
	private bool keyMove = false;
	private int keyDirection;
	private bool mouseMove = false;
	private int stringLength;
	private int elementHeight = 20;
	
	// scriptable object
    public StringScriptableObject StringData;

	// Use this for initialization
	void Start () {
		if (StringData.Strs == null) {
			Debug.LogError("Scriptable Object StringNames is empty.");
			return;
		}
		stringLength = StringData.Strs.Count;
		for (int i = 0; i < ScrollObjects.Count; i++) {
			// check ui panel
			if (ScrollObjects[i] == null) {
				Debug.LogError(string.Format("Scroll Object {0} should be Text{1}", i, i));
				return;
			}
			// check Rect Transform component
			RectTransform rt = ScrollObjects[i].GetComponent<RectTransform>();
			if (rt != null) {
				RectTransforms.Add(rt);
			}
			else {
				Debug.LogError(string.Format("Scroll Object {0} should have a RectTransform component.", i));
				return;
			}
			// check Text component
			Text t = ScrollObjects[i].GetComponent<Text>();
			if (t != null) {
				Texts.Add(t);
			}
			startPositions.Add(RectTransforms[i].anchoredPosition);
			colors.Add(Color.white);
		}
		// initialize strings
		for (int i = 0; i < Texts.Count; i++) {
			Texts[i].text = StringData.Strs[i];
			Texts[i].color = new Color(1.0f, 1.0f, 1.0f, (1 - Mathf.Abs((RectTransforms[i].anchoredPosition.y + 50)) / (elementHeight * 3)));
		}
		for(int i = 0; i < startPositions.Count; i++) {
			startPositions[i] = RectTransforms[i].anchoredPosition;
		}
		Center = RectTransforms[3];
	}
	
	// Update is called once per frame
	void Update () {
		// alpha
		for(int i = 0; i < startPositions.Count; i++) {
			startPositions[i] = RectTransforms[i].anchoredPosition;
			Texts[i].color = new Color(1.0f, 1.0f, 1.0f, (1 - Mathf.Abs((RectTransforms[i].anchoredPosition.y + 50)) / (elementHeight * 3)));
		}
		// get UP key
		if (Input.GetKeyDown(KeyCode.UpArrow)) {
			keyMove = true;
			keyDirection = -1;
		}
		// get DOWN key
		if (Input.GetKeyDown(KeyCode.DownArrow)) {
			keyMove = true;
			keyDirection = 1;
		}
		// animation for key input
		if (keyMove == true) {
			if (keyTime <= timeToReachTarget) {
				keyTime += Time.deltaTime;
				ScrollLerp(keyDirection, keyTime);
			}
			else {
				startIndex += keyDirection;
				for(int i = 0; i < ScrollObjects.Count; i++) {
					int index = (i+startIndex) % stringLength;
					if (index < 0) {
						index = index + stringLength;
					}
					RectTransforms[i].anchoredPosition = new Vector3(0.0f, getYPosition(i), 0.0f);
					Texts[i].text = StringData.Strs[index];
				}
				keyTime = 0f;
				keyMove = false;
			}
		}
		// get SCROLLWHEEL input
		float n = Input.GetAxis("Mouse ScrollWheel");
		if (n != 0) {
			UpdateMouse(n);
			mouseMove = true;
		}
		else { // n == 0
			if ((mouseTime <= timeToReachTarget) && (Center.anchoredPosition.y + 0.5 * elementHeight) % elementHeight != 0) {
				if (mouseMove == true) {
					mouseTime += Time.deltaTime;
					ScrollLerp(0, mouseTime);
				}
			}
			else {
				mouseTime = 0f;
				offset = 0;
				mouseMove = false;
			}
		}
	}

	void ScrollLerp(int n, float t) {
		for(int i = 0; i < ScrollObjects.Count; i++) {
			RectTransforms[i].anchoredPosition = Vector3.Lerp(startPositions[i], new Vector3(0.0f, getYPosition(i-n), 0.0f), t / timeToReachTarget);
		}
	}

	void UpdateMouse (float v) {
		offset -= v*speed;
		int newOffsetDivision = (int)offset / elementHeight;
		if ((newOffsetDivision) != 0) {
			if (newOffsetDivision > 0) {
				// move up
				startIndex += 1;
			}
			else if (newOffsetDivision < 0) {
				// move down
				startIndex -= 1;
			}
			for (int i = 0; i < Texts.Count; i++) {
				int index = (i+startIndex) % stringLength;
				if (index < 0) {
					index = index + stringLength;
				}
				Texts[i].text = StringData.Strs[index];
			}
		}
		offset = offset % 20;
		for(int i = 0; i < ScrollObjects.Count; i++) {
			RectTransforms[i].anchoredPosition = new Vector3(0.0f, getYPosition(i) + offset, 0.0f);
		}
	}

	int getYPosition(int i) {
		return 10-i*20;
	}
}
