using Mirror;
using Mirror.Examples.Pong;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    [SerializeField]
    private TMP_Text P1ScoreText;
    [SerializeField]
    private TMP_Text P2ScoreText;
    private int P1Score, P2Score = 0;
    public GameObject ballPrefab;
    [SyncVar]
    public GameObject ballObject;
    public Transform ballSpawn;
    public TMP_Text serveText;

    public uint localPlayerID;
    [SyncVar]
    public uint player1ID;
    [SyncVar]
    public uint player2ID;
    [SyncVar]
    public bool readyToServe = true;
    [SyncVar]
    public uint servePlayerId;

    

    public static GameManager Instance;

    private void Awake()
    {
        if (Instance)
        {
            throw new Exception("GameManager already present in scene!");
        }
        Instance = this;
    }

    private void OnEnable()
    {
        Ball.OnScore += PlayerScored;
    }

    private void OnDisable()
    {
        Ball.OnScore -= PlayerScored;
    }

    // Update is called once per frame
    void Update()
    {
        if (readyToServe)
        {
            Debug.Log($"server ID : {servePlayerId} and localPlayerID = {localPlayerID}");
            if (Input.GetKeyDown("space") && localPlayerID == servePlayerId)
            {
                Player localPlayer = NetworkClient.localPlayer.gameObject.GetComponent<Player>();
                localPlayer.Serve(servePlayerId);
                readyToServe = false;
            }
        }
    }

    [ClientRpc]
    private void PlayerScored(int playerOne)
    {
        if (playerOne == 0) //left scored
        {
            P1ScoreText.text = (++P1Score).ToString();
            servePlayerId = player1ID;
            ResetBall();
        }
        else //right scored 
        {
            P2ScoreText.text = (++P2Score).ToString();
            servePlayerId = player2ID;
            ResetBall();
        }
    }

    [ServerCallback]
    private void ResetBall()
    {
        if (ballObject != null)
        {
            //NetworkServer.Destroy(ballObject);
            ballObject.GetComponent<Ball>().StopBall();
            ballObject.transform.position = ballSpawn.position;
            readyToServe = true;
            UpdateServerText(true);
        }
    }

    [ClientRpc]
    public void UpdateServerText(bool enabled)
    {
        if (enabled)
        {
            if (servePlayerId == player1ID)
            {
                serveText.text = "Player 1 Is Serving";
            }
            else
            {
                serveText.text = "Player 2 Is Serving";
            }
            serveText.gameObject.SetActive(true);
        } else
        {
            serveText.gameObject.SetActive(false);
        }
    }
}
