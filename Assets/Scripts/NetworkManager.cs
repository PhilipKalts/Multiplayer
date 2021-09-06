using UnityEngine;
using Photon.Pun;


public class NetworkManager : MonoBehaviourPunCallbacks
{
	public static NetworkManager instance;

	void Awake()
	{
		if (instance != null && instance != this) Destroy(gameObject);
		else
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
	}

	void Start()
	{
		PhotonNetwork.ConnectUsingSettings();
	}



	public void CreateRoom(string roomName)
    {
		PhotonNetwork.CreateRoom(roomName);
    }

    public override void OnCreatedRoom()
    {
		print("Created Room: " + PhotonNetwork.CurrentRoom.Name);
    }

    public void JoinRoom(string roomName)
	{
		PhotonNetwork.JoinRoom(roomName);
	}

	[PunRPC]
	public void ChangeScene (string sceneName)
	{
		PhotonNetwork.LoadLevel(sceneName);
	}

}
