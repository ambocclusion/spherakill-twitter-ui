using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Twitter;

namespace Spacetronaut{
	public class TweetLoop : MonoBehaviour {

		public bool initialized = false;

		public GameObject TwitterFeed;

		public float LoopTime = 10.0f;

		private List<Tweet> tweets = new List<Tweet>();

		void Start(){

			TwitterAuth.SetAuth();

			Dictionary<string, string> parameters = new Dictionary<string, string>();
			parameters["q"] = "#kentuckyfriedpixels";
			//parameters["user_id"] = 19738581.ToString();
			//parameters["count"] = 10.ToString();
			//parameters["exclude_replies"] = true.ToString();
			//parameters["include_rts"] = false.ToString();
			//StartCoroutine(Client.Get("statuses/user_timeline", parameters, this.Callback));
			StartCoroutine(Client.Get("search/tweets", parameters, this.Callback));

		}

		void Callback(bool success, string response) {
			if(success) {

				initialized = true;

				SearchTweetsResponse Response = JsonUtility.FromJson<SearchTweetsResponse>(response);
				System.IO.File.WriteAllText("Assets/test.json", response);
				UnityEditor.AssetDatabase.Refresh();

				List<Spacetronaut.Tweet> tw = new List<Spacetronaut.Tweet>();

				int i = 0;
				foreach(Twitter.Tweet t in Response.statuses) {
					Spacetronaut.Tweet newTweet = new Spacetronaut.Tweet();
					newTweet.userName = t.user.screen_name;
					newTweet.fullName = t.user.name;
					newTweet.content = t.text;
					StartCoroutine(GetIcon(t.user.profile_image_url, i));
					tw.Add(newTweet);
					i++;
				}

				tweets = tw;

                System.IO.File.WriteAllText("Assets/testing.json", response);

				StartCoroutine(TweetRunner(LoopTime));
			}
			else {
				Debug.Log(response);
			}
		}

		private void OnReceiveAuthURL(string url){

			Debug.Log(url);

			//initialized = true;

		}

		IEnumerator TweetRunner(float loopTime) {

			yield return new WaitForSeconds(1.0f);

			TwitterFeed.SetActive(true);
			TwitterFeed.GetComponent<CanvasGroup>().alpha = 0.0f;

			while(true) {

				foreach(Tweet t in tweets) {

					TwitterFeed.GetComponent<TwitterHandler>().ShowTweet(t, true);

					while(TwitterFeed.GetComponent<CanvasGroup>().alpha != 1.0f) {
						TwitterFeed.GetComponent<CanvasGroup>().alpha = Mathf.MoveTowards(TwitterFeed.GetComponent<CanvasGroup>().alpha, 1.0f, 1.5f * Time.deltaTime);
						yield return null;
					}
					yield return new WaitForSeconds(loopTime);
				}

			}

		}

		IEnumerator GetIcon(string url, int tweetToChange) {

			UnityWebRequest wr = UnityWebRequestTexture.GetTexture(url);

			yield return wr.Send();

			Texture2D downloaded = DownloadHandlerTexture.GetContent(wr);

			tweets[tweetToChange].icon = Sprite.Create(downloaded, new Rect(0, 0, downloaded.width, downloaded.height), Vector2.zero);

		}

		void Update(){

			if(!initialized){
				TwitterFeed.SetActive(false);
			}

		}

	}
}