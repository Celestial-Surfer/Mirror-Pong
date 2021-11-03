using UnityEngine;
using System.Collections;
using TMPro;

namespace Mirror.Examples.Pong
{
    // Custom NetworkManager that simply assigns the correct racket positions when
    // spawning players. The built in RoundRobin spawn method wouldn't work after
    // someone reconnects (both players would be on the same side).
    [AddComponentMenu("")]
    public class NetworkManagerPong : NetworkManager
    {
        public Transform leftRacketSpawn;
        public Transform rightRacketSpawn;
        GameObject ball;

        public override void OnServerAddPlayer(NetworkConnection conn)
        {
            // add player at correct spawn position
            Transform start = numPlayers == 0 ? leftRacketSpawn : rightRacketSpawn;
            GameObject playerObject = Instantiate(playerPrefab, start.position, start.rotation);
            NetworkServer.AddPlayerForConnection(conn, playerObject);

            if(numPlayers == 1)
            {
                GameManager.Instance.player1ID = playerObject.GetComponent<NetworkIdentity>().netId;
                GameManager.Instance.servePlayerId = playerObject.GetComponent<NetworkIdentity>().netId;
            } else if(numPlayers == 2)
            {
                GameManager.Instance.player2ID = playerObject.GetComponent<NetworkIdentity>().netId;
            }

            NetworkIdentity netId = playerObject.GetComponent<NetworkIdentity>();

            // spawn ball if two players
            if (numPlayers == 2)
            {
                ball = Instantiate(spawnPrefabs.Find(prefab => prefab.name == "Ball"), GameManager.Instance.ballSpawn);
                NetworkServer.Spawn(ball);
                GameManager.Instance.ballObject = ball;
            }
        }

        public override void OnServerDisconnect(NetworkConnection conn)
        {
            // destroy ball
            if (ball != null)
                NetworkServer.Destroy(ball);
            Debug.Log("OnServerDisconnect");

            // call base functionality (actually destroys the player)
            base.OnServerDisconnect(conn);
        }   
    }
}
