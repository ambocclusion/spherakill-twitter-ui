using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Spacetronaut {

	public class TwitterHandler : MonoBehaviour {

		public GameObject IconTemplate;
		public GameObject FullNameTemplate;
		public GameObject UsernameTemplate;
		public GameObject ContentTemplate;

		public float AnimationTime = 1.0f;

		private List<GameObject> lastSetup = new List<GameObject>();

		private Tweet lastTweet;

		public void ShowTweet(Tweet tweetToShow, bool animate){

			Dictionary<string, GameObject> setup = new Dictionary<string, GameObject>(){
				{"icon", CreateObject(IconTemplate, Vector3.zero)},
				{"fullName", CreateObject(FullNameTemplate, Vector3.zero)},
				{"username", CreateObject(UsernameTemplate, Vector3.zero)},
				{"content", CreateObject(ContentTemplate, Vector3.zero)}
			};

			setup["icon"].GetComponent<Image>().sprite = tweetToShow.icon;
			setup["fullName"].GetComponent<Text>().text = tweetToShow.fullName;
			setup["username"].GetComponent<Text>().text = tweetToShow.userName;
			setup["content"].GetComponent<Text>().text = tweetToShow.content;

			if(animate){
				StartCoroutine(AnimateObjects(lastSetup, (List<GameObject>)setup.Values.ToList(), AnimationTime));
			}
			else{
				for(int i = 0; i < lastSetup.Count; i++){
					setup.Values.ToList()[i].transform.position = lastSetup[i].transform.position;
				}
				for(int i = 0; i < lastSetup.Count; i++){
					Destroy(lastSetup[i].gameObject);
				}
			}

			lastSetup = (List<GameObject>)setup.Values.ToList();
			lastTweet = tweetToShow;

		}

		private GameObject CreateObject(GameObject template, Vector3 startOffset){

			template.SetActive(true);

			GameObject obj = (GameObject)Instantiate(template, template.transform.position, template.transform.rotation, template.transform.parent);
			obj.transform.localScale = Vector3.one;
			((RectTransform)obj.transform).sizeDelta = ((RectTransform)template.transform).sizeDelta;

			obj.name = obj.name.Replace(" Template(Clone)", "");

			template.SetActive(false);

			return obj;

		}

		private IEnumerator AnimateObjects(List<GameObject> prevSetup, List<GameObject> newSetup, float time){

			float startTime = Time.time;

			float animDifference = 0.0f;

			List<float> newStartPos = new List<float>();
			List<Vector3> newDest = new List<Vector3>();

			foreach(GameObject n in newSetup){

				Vector3 dest = ((RectTransform)n.transform).anchoredPosition;
				newDest.Add(dest);
				((RectTransform)n.transform).anchoredPosition -= new Vector2(0, ((RectTransform)n.transform).rect.height);
				newStartPos.Add(Vector3.Distance(((RectTransform)n.transform).anchoredPosition, dest));

			}

			while(Vector2.Distance(((RectTransform)newSetup[newSetup.Count - 1].transform).anchoredPosition, newDest[newSetup.Count - 1]) > .01f){

				animDifference += Time.deltaTime;

				for(int i = 0; i < prevSetup.Count; i++){

					((RectTransform)prevSetup[i].transform).anchoredPosition = Vector2.Lerp(((RectTransform)prevSetup[i].transform).anchoredPosition, new Vector2(((RectTransform)prevSetup[i].transform).anchoredPosition.x, ((RectTransform)prevSetup[i].transform).rect.height), ((animDifference / time) - (i * -.15f)) * Time.deltaTime);					
					prevSetup[i].GetComponent<CanvasGroup>().alpha = Mathf.Lerp(prevSetup[i].GetComponent<CanvasGroup>().alpha, 0.0f, ((animDifference / (time / 4f)) - (i * -.15f)) * Time.deltaTime);

				}

				for(int i = 0; i < newSetup.Count; i++){

					((RectTransform)newSetup[i].transform).anchoredPosition = Vector2.Lerp(((RectTransform)newSetup[i].transform).anchoredPosition, newDest[i], ((animDifference / time) - (i * -.15f)) * Time.deltaTime);

				}

				yield return null;

			}

			foreach(GameObject p in prevSetup){

				Destroy(p.gameObject);

			}

		}

	}

	[System.Serializable]
	public class Tweet {

		public Sprite icon;
		public string userName;
		public string fullName;
		public string content;

	}

}