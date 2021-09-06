using TMPro;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;


public class Menu : MonoBehaviourPunCallbacks
{
	[Header("Screens")]
	public GameObject mainScreen;
	public GameObject lobbyScreen;

	[Header("Main Screen")]
	public Button createRoomButton;
	public Button joinRoomButton;

	[Header("Lobby Screen")]
	public TextMeshProUGUI playerListText;
	public Button startGameButton;



    private void Start()
    {
		createRoomButton.interactable = false;
		joinRoomButton.interactable = false;
    }


    public override void OnConnectedToMaster()
    {
		createRoomButton.interactable = true;
		joinRoomButton.interactable = true;
	}

	void SetScreen(GameObject screen)
    {
		mainScreen.SetActive(false);
		lobbyScreen.SetActive(false);

		screen.SetActive(true);
    }


	public void OnCreateRoomButton(TMP_InputField roomNameInput)
    {
		NetworkManager.instance.CreateRoom(roomNameInput.text);
    }

	public void OnJoinRoomButton (TMP_InputField roomNameInput)
    {
		NetworkManager.instance.JoinRoom(roomNameInput.text);
    }

	public void OnPlayerNameUpdate (TMP_InputField playerNameInput)
    {
		PhotonNetwork.NickName = playerNameInput.text;
    }

    public override void OnJoinedRoom()
    {
		SetScreen(lobbyScreen);

		//Since there's a new player in the lobby, tell everyone to
		//update the lobby
		photonView.RPC("UpdateLobbyUI", RpcTarget.All);
    }


    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
		UpdateLobbyUI();
    }



    [PunRPC]
	public void UpdateLobbyUI()
    {
		playerListText.text = "";

		//Display all players currently in the lobby
		foreach (Player player in PhotonNetwork.PlayerList)
        {
			playerListText.text += player.NickName + "\n";
        }

		//only trhe host can start the game
		if (PhotonNetwork.IsMasterClient) startGameButton.interactable = true;
		else startGameButton.interactable = false;

    }



	public void OnLeaveLobbyButton()
    {
		PhotonNetwork.LeaveRoom();
		SetScreen(mainScreen);
    }


	public void OnStartGameButton()
    {
		NetworkManager.instance.photonView.RPC("ChangeScene", RpcTarget.All, "Game");
    }



}
