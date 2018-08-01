using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace YouTubeLive {
  [RequireComponent(typeof(YtlClient))]
  [RequireComponent(typeof(YtlServer))]
  public class YtlController : MonoBehaviour {
    [SerializeField]
    Access access;

    [SerializeField]
    float interval = 3f;

    public event Action<Chat.Msg> OnMessage;

    IEnumerator Start() {
      OnMessage += _ => {};

      var client = GetComponent<YtlClient> ();
      client.access = access;
      yield return null;

      if (access.code == "") {
        var server = GetComponent<YtlServer>();

        server.Listen();
        server.OnReceiveCode += code => {
          access.code = code;
        };
        yield return null;

        Application.OpenURL (client.AuthUrl());
        yield return new WaitUntil (() => access.code != "");
        server.Stop ();
      }

      if (access.token == "") {
        client.GetToken((token,_) => {
          access.token = token;
        }, err => {
          Debug.Log("GetToken>" + err);
        });
        yield return new WaitUntil (() => access.token != "");
      }

      var chatId = "";
      client.GetLiveChatId (access.id, c => {
        chatId = c;
      }, err => {
        Debug.Log("GetLiveChatId>" + err);
      });
      yield return new WaitUntil (() => chatId != "");

      var pageToken = "";
      while (true) {
        client.GetChatMessages(chatId, pageToken, chat => {
          foreach (var msg in chat.msgs) {
            OnMessage(msg);
          }
          pageToken = chat.pageToken;
        }, err => Debug.Log("GetChatMessages>" + err));

        yield return new WaitForSeconds (interval);
      }
    }

  }

}
