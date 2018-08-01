using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;

namespace YouTubeLive {
  public class Chat {
    public class Msg {
      public string text;
      public string name;
      public string img;
    }
    public List<Msg> msgs = new List<Msg>();
    public string pageToken;
  }

  [Serializable]
  public class Access {
    [Header("YouTube")]
    public string id = "_video_uri";

    [Header("OAuth")]
    public string clientId = "_client_id";
    public string clientSecret = "_client_secret";

    [Header("Options")]
    public string redirectUri = "http://localhost:8080";
    public string grantType = "authorization_code";
    public string accessType = "offline";
    public string scope = "https://www.googleapis.com/auth/youtube.readonly";
    public string code = "";
    public string token = "";
  }

  public class YtlClient : MonoBehaviour {
    public Access access { set; get; }

    public string AuthUrl() {
      return "https://accounts.google.com/o/oauth2/v2/auth?response_type=code"
        + "&client_id=" + access.clientId
        + "&redirect_uri=" + access.redirectUri
        + "&scope=" + access.scope
        + "&access_type=" + access.accessType;
    }

    public void GetToken(Action<string,string> OnSuccess, Action<string> OnFailure) {
      var url = "https://www.googleapis.com/oauth2/v4/token";
      var content = new Dictionary<string,string> () {
        { "code", access.code },
        { "client_id", access.clientId },
        { "client_secret", access.clientSecret },
        { "redirect_uri", access.redirectUri },
        { "grant_type", access.grantType },
        { "access_type", access.accessType },
      };

      StartCoroutine (Post (url, content, text => {
        var json = JSON.Parse(text);
        var accessToken = json["access_token"].RawString();
        var refreshToken = json["refresh_token"].RawString();
        OnSuccess(accessToken, refreshToken);
      }, OnFailure));
    }

    public void GetLiveChatId(string id, Action<string> OnSuccess, Action<string> OnFailure) {
      var url = "https://www.googleapis.com/youtube/v3/liveBroadcasts?part=snippet";
      url += "&id=" + id;

      StartCoroutine (Get (url, text => {
        var json = JSON.Parse(text);
        var chatId = json["items"][0]["snippet"]["liveChatId"].RawString();
        OnSuccess(chatId);
      }, OnFailure));
    }

    public void GetChatMessages(string chatId, string pageToken, Action<Chat> OnSuccess, Action<string> OnFailure) {
      var url = "https://www.googleapis.com/youtube/v3/liveChat/messages?part=snippet,authorDetails";
      url += "&liveChatId=" + chatId;
      url += pageToken == "" ? "" : "&pageToken=" + pageToken;

      StartCoroutine (Get (url, text => {
        var chat = new Chat();

        var json = JSON.Parse(text);
        var items = json["items"];

        foreach (var item in items) {
          var snip = item.Value["snippet"];
          var author = item.Value["authorDetails"];
          chat.msgs.Add(new Chat.Msg() {
            text = snip["displayMessage"].RawString(),
            name = author["displayName"].RawString(),
            img = author["profileImageUrl"].RawString()
          });
        }

        var next = json["nextPageToken"];
        chat.pageToken = next;

        OnSuccess(chat);
      }, OnFailure));
    }

    IEnumerator Get(string uri, Action<string> OnSuccess, Action<string> OnFailure) {
      var content = new Dictionary<string,string> () {
        { "Authorization", "Bearer " + access.token }
      };
      yield return Get (uri, content, OnSuccess, OnFailure);
    }

    IEnumerator Get(string uri, Dictionary<string,string> content, Action<string> OnSuccess, Action<string> OnFailure) {
      UnityWebRequest request = UnityWebRequest.Get (uri);
      foreach (var d in content) {
        request.SetRequestHeader (d.Key, d.Value);
      }
      yield return request.SendWebRequest();

      if (request.isNetworkError) {
        OnFailure("[Network Error]");
      } else if (request.isHttpError) {
        OnFailure(request.downloadHandler.text);
      } else {
        OnSuccess(request.downloadHandler.text);
      }
    }

    IEnumerator Post(string uri, Dictionary<string,string> content, Action<string> OnSuccess, Action<string> OnFailure) {
      UnityWebRequest request = UnityWebRequest.Post (uri, content);
      yield return request.SendWebRequest();

      if (request.isNetworkError) {
        OnFailure("[Network Error]");
      } else if (request.isHttpError) {
        OnFailure(request.downloadHandler.text);
      } else {
        OnSuccess(request.downloadHandler.text);
      }
    }

  }

  public static class SimpleJsonUtility {
    public static string RawString(this JSONNode node) {
      var len = node.ToString ().Length - 2;
      return node.ToString ().Substring (1, len);
    }
  }
}
