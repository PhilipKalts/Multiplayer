using Photon.Pun;
using System.Linq;
using UnityEngine;
using Photon.Realtime;


public class GameManager : MonoBehaviourPunCallbacks
{
	public static GameManager instance;

	[Header("Stats")]
	public bool hasGameEnded;
	public float timeToWin;
	public float invinsibleDuration;

	float hatPickUpTime;


	[Header("Players")]
	public string playerPrefabLocation;
	public Transform[] spawnPoints;
	public PlayerController[] players;
	public int playerWithHat;
	int playersInGame;

    private void Awake()
    {
		instance = this;
    }

    private void Start()
    {
		players = new PlayerController[PhotonNetwork.PlayerList.Length];
		photonView.RPC("ImInGame", RpcTarget.AllBuffered);
	}

	[PunRPC]
	void ImInGame()
    {
		playersInGame++;

		// when all the players are in the scene - spawn the players
		if (playersInGame == PhotonNetwork.PlayerList.Length)
			SpawnPlayer();
	}

	void SpawnPlayer()
    {
		// instantiate the player across the network
		GameObject playerObj = PhotonNetwork.Instantiate(playerPrefabLocation, spawnPoints[Random.Range(0, spawnPoints.Length)].position, Quaternion.identity, 0);

		// get the player script
		PlayerController playerScript = playerObj.GetComponent<PlayerController>();

		// initialize the player
		playerScript.photonView.RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer);
	}

	public PlayerController GetPlayer (int playerId)
    {
		return players.First(x => x.id == playerId);
	}

	public PlayerController GetPlayer (GameObject playerObj)
    {
		return players.First(x => x.gameObject == playerObj);
    }



	//Called when a player hits the hatted player
	//giving them the hat
	[PunRPC]
	public void GiveHat(int playerId, bool initialGive)
    {
		// remove the hat from the currently hatted player
        if(!initialGive)
            GetPlayer(playerWithHat).SetHat(false);

        // give the hat to the new player
        playerWithHat = playerId;
        GetPlayer(playerId).SetHat(true);
        hatPickUpTime = Time.time;
    }

	//Is the player able to take the hat at this current time?
	public bool CanGetHat()
    {
		if (Time.time > hatPickUpTime + invinsibleDuration) return true;
		else return false;
    }

	[PunRPC]
	void WinGame(int playerId)
	{
		hasGameEnded = true;
		PlayerController player = GetPlayer(playerId);

		Invoke("GoBackToMenu", 3);
	}

	void GoBackToMenu()
    {
		PhotonNetwork.LeaveRoom();
		NetworkManager.instance.ChangeScene("Menu");
    }


}
