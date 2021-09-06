using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
{
	[HideInInspector]
	public int id;

	[Header("Info")]
	public float moveSpeed;
	public float jumpForce;
	public GameObject hatObject;

	[HideInInspector]
	public float curHatTime;

	[Header("Components")]
	public Rigidbody rigid;
	public Player photonPlayer;

	[PunRPC]
	public void Initialize(Player player)
	{
		photonPlayer = player;
		id = player.ActorNumber;

		GameManager.instance.players[id - 1] = this;

		//Give the hat to the first player
		if (id == 1) GameManager.instance.GiveHat(id, true);

		// if this isn't our local player, disable physics as that's
		// controlled by the user and synced to all other clients
		if(!photonView.IsMine)
			rigid.isKinematic = true;
	}

	private void Update()
	{
		if (PhotonNetwork.IsMasterClient)
        {
			if (curHatTime >= GameManager.instance.timeToWin && !GameManager.instance.hasGameEnded)
            {
				GameManager.instance.hasGameEnded = true;
				GameManager.instance.photonView.RPC("WinGame", RpcTarget.All, id);
            }
        }

		if (photonView.IsMine)
        {
			Move();
			if (Input.GetKeyDown(KeyCode.Space)) TryJump();

			// Track the amount of time we're wearing the hat
			if (hatObject.activeInHierarchy) curHatTime += Time.deltaTime;
        }
	}

	void Move()
	{
		float x = Input.GetAxis("Horizontal") * moveSpeed;
		float z = Input.GetAxis("Vertical") * moveSpeed;

		rigid.velocity = new Vector3(x, rigid.velocity.y, z);

	}

	void TryJump()
	{
		Ray ray = new Ray(transform.position, Vector3.down);

		if (Physics.Raycast(ray, 0.7f))
		{
			rigid.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
		}

	}


	public void SetHat(bool hasHat)
    {
		hatObject.SetActive(hasHat);
    }

    void OnCollisionEnter(Collision collision)
    {
		if (!photonView.IsMine) return;

		//Did we hit another player
		if (collision.gameObject.tag == "Player")
        {
			//Do they have the hat
			if (GameManager.instance.GetPlayer(collision.gameObject).id == GameManager.instance.playerWithHat)
            {
				// can we get the hat?
				if (GameManager.instance.CanGetHat())
                {
					//Give us the hat
					GameManager.instance.photonView.RPC("GiveHat", RpcTarget.All, id, false);
                }
            }
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
		if (stream.IsWriting) stream.SendNext(curHatTime);
		else if (stream.IsReading) curHatTime = (float)stream.ReceiveNext();
    }
}
