using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KJY_PhotonRobby : MonoBehaviourPunCallbacks
{
    //Input Room Name
    public InputField inputRoomName;
    //Input Max Player
    public InputField inputMaxPlayer;
    //Input Password
    public InputField inputPassword;

    //�� ���� ��ư
    public Button btnJoinRoom;
    //�� ���� ��ư
    public Button btnCreateRoom;

    //RoomItem Prefab
    public GameObject roomItemFactory;
    //RoomListView -> Content -> RectTransform
    public RectTransform rtContent;

    //�� ���� ������ �ִ� Dictionary
    Dictionary<string, RoomInfo> roomCache = new Dictionary<string, RoomInfo>();

    void Start()
    {
        //�� ����, ���� ��Ȱ��ȭ
        btnJoinRoom.interactable = btnCreateRoom.interactable = false;
        //inputRoomName �� ������ ����� �� ȣ��Ǵ� �Լ�
        inputRoomName.onValueChanged.AddListener(OnValueChangedRoomName);
        //inputMaxPlayer �� ������ ����� �� ȣ��Ǵ� �Լ�
        inputMaxPlayer.onValueChanged.AddListener(OnValueChangedMaxPlayer);
    }

    //���� & ���� ��ư�� ����
    void OnValueChangedRoomName(string room)
    {
        //���� ��ư Ȱ��/ ��Ȱ��
        btnJoinRoom.interactable = room.Length > 0;
        //���� ��ư Ȱ�� / ��Ȱ��
        btnCreateRoom.interactable = room.Length > 0 && inputMaxPlayer.text.Length > 0;
    }

    //���� ��ư�� ����
    void OnValueChangedMaxPlayer(string max)
    {
        btnCreateRoom.interactable = max.Length > 0 && inputRoomName.text.Length > 0;
    }

    public void CreateRoom()
    {
        //�� �ɼ��� ���� (�ִ� �ο�)
        RoomOptions option = new RoomOptions();
        option.MaxPlayers = int.Parse(inputMaxPlayer.text);
        //�� ��Ͽ� ���̰� �ϳ�? ���ϳ�?
        option.IsVisible = true;
        //�濡 ������ �� �ִ�? ����?
        option.IsOpen = true;

        //custom ����
        ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();
        hash["room_name"] = inputRoomName.text;
        hash["map_idx"] = 2;

        //custom ������ option �� ����
        option.CustomRoomProperties = hash;

        //custom ������ Lobby ���� ����� �� �ְ� ����
        string[] customKeys = { "room_name", "map_idx"};
        option.CustomRoomPropertiesForLobby = customKeys;

        //Ư�� �κ� �� ���� ��û
        //TypedLobby typedLobby = new TypedLobby("Meta Lobby", LobbyType.Default);
        //PhotonNetwork.CreateRoom(inputRoomName.text, option, typedLobby);

        //�⺻ �κ� �� ���� ��û
        PhotonNetwork.CreateRoom(inputRoomName.text + inputPassword.text, option);
    }

    //�� ���� �Ϸ�� ȣ�� �Ǵ� �Լ�
    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        print("�� ���� �Ϸ�");
    }

    //�� ���� ���н� ȣ�� �Ǵ� �Լ�
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        print("�� ���� ���� : " + message);
    }

    public void JoinRoom()
    {
        //�� ���� ��û
        PhotonNetwork.JoinRoom(inputRoomName.text + inputPassword.text);
    }

    // �� ���� �Ϸ�� ȣ��Ǵ� �Լ�
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        print("�� ���� �Ϸ�");

        //GameScene ���� �̵�
        PhotonNetwork.LoadLevel("Room_KJY");
    }

    // �� ���� ���н� ȣ��Ǵ� �Լ�
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
        print("�� ���� ���� : " + message);
    }

    void RemoveRoomList()
    {
        //rtContent �� �ִ� �ڽ� GameObject �� ��� ����
        for (int i = 0; i < rtContent.childCount; i++)
        {
            Destroy(rtContent.GetChild(i).gameObject);
        }


        //foreach (Transform tr in rtContent)
        //{
        //    Destroy(tr.gameObject);
        //}
    }

    void UpdateRoomList(List<RoomInfo> roomList)
    {
        foreach (RoomInfo info in roomList)
        {
            //roomCache �� info �� ���̸� ���� �Ǿ��ִ� Key �� �����ϴ�?
            if (roomCache.ContainsKey(info.Name))
            {
                //���� �ؾ��ϴ�?
                if (info.RemovedFromList)
                {
                    roomCache.Remove(info.Name);
                }
                //����
                else
                {
                    roomCache[info.Name] = info;
                }
            }
            else
            {
                //�߰�
                roomCache[info.Name] = info;
            }
        }
    }

    void CreateRoomList()
    {
        foreach (RoomInfo info in roomCache.Values)
        {
            //roomItem prefab �� �̿��ؼ� roomItem �� �����. 
            GameObject goRoomItem = Instantiate(roomItemFactory, rtContent);
            ////������� roomItem �� �θ� scrollView -> Content �� transform ���� �Ѵ�.
            //goRoomItem.transform.parent = rtContent;

            //custom ���� �̾ƿ���.
            string roomName = (string)(info.CustomProperties["room_name"]);
            int mapIdx = (int)(info.CustomProperties["map_idx"]);

            //������� roomItem ���� RoomItem ������Ʈ �����´�.
            KJY_RoomItem roomItem = goRoomItem.GetComponent<KJY_RoomItem>();
            //������ ������Ʈ�� ������ �ִ� SetInfo �Լ� ����
            roomItem.SetInfo(roomName, InfoManagerKJY.instance.nickname ,info.PlayerCount, info.MaxPlayers);
            //RoomItem �� Ŭ�� �Ǿ��� �� ȣ��Ǵ� �Լ� ���
            roomItem.onChangeRoomName = OnChangeRoomNameField;

            //���ٽ� lamda 
            //roomItem.onChangeRoomName = (string roomName) => {
            //    inputRoomName.text = roomName;
            //};
        }
    }

    // ������ ���� ����ų� ���������� ȣ��Ǵ� �Լ�
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);

        //��ü �븮��ƮUI ����
        RemoveRoomList();
        //���� ���� �����ϴ� �븮��Ʈ ���� ����
        UpdateRoomList(roomList);
        //�븮��Ʈ ������ ������ UI �� �ٽ� ����
        CreateRoomList();
    }

    public void OnChangeRoomNameField(string roomName)
    {
        inputRoomName.text = roomName;
    }
}