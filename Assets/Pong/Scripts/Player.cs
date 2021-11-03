using UnityEngine;

namespace Mirror.Examples.Pong
{
    public class Player : NetworkBehaviour
    {
        public float speed = 30;
        public Rigidbody2D rigidbody2d;

        private void Start()
        {
            if (gameObject.GetComponent<NetworkIdentity>().isLocalPlayer)
            {
                GameManager.Instance.localPlayerID = NetworkClient.localPlayer.netId;
            }
        }

        // need to use FixedUpdate for rigidbody
        void FixedUpdate()
        {
            // only let the local player control the racket.
            // don't control other player's rackets
            if (isLocalPlayer)
                rigidbody2d.velocity = new Vector2(0, Input.GetAxisRaw("Vertical")) * speed * Time.fixedDeltaTime;
        }

        [Command]
        public void Serve(uint servePlayerId)
        {
            Debug.Log("COMMANDING SERVER TO START SERVE");
            if (servePlayerId == GameManager.Instance.player1ID)
            {
                Debug.Log("SERVING RIGHT");
                ServerServe(0);
            }
            else if (servePlayerId == GameManager.Instance.player2ID)
            {
                Debug.Log("SERVING LEFT?");
                ServerServe(1);
            }
            GameManager.Instance.UpdateServerText(false);
        }

        [ClientRpc]
        private void ServerServe(int direction)
        {
            Debug.Log("SERVER SERVING BALL");
            if (direction == 0)
            {
                GameManager.Instance.ballObject.GetComponent<Ball>().UpdateServeDirection(true);
            }
            else
            {
                GameManager.Instance.ballObject.GetComponent<Ball>().UpdateServeDirection(false);
            }
        }
    }
}
