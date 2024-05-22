using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    enum CurrentPlayer
    {
        Player1,
        Player2
    }

    CurrentPlayer currentPlayer;
    bool isWinningShotForPlayer1;
    bool isWinningShotForPlayer2;
    int player1BallsRemaining = 7;
    int player2BallsRemaining = 7;
    bool isWaitingForBallMovementToStop = false;
    bool isGameOver = false;
    bool willSwapPlayer = false;
    [SerializeField] float shotTimer = 3f;
    private float currentTimer;
    [SerializeField] float movementThreshold;
    private bool ballPocketed = false;

    [SerializeField] TextMeshProUGUI player1BallsText;
    [SerializeField] TextMeshProUGUI player2BallsText;
    [SerializeField] TextMeshProUGUI currentTurnText;
    [SerializeField] TextMeshProUGUI popUp;

    [SerializeField] GameObject buttonRestart;
    [SerializeField] Transform headPosition;

    [SerializeField] Camera cueStickCamera;
    [SerializeField] Camera overheadCamera;

    Camera currentCamera;

    // Start is called before the first frame update
    void Start()
    {
        currentPlayer = CurrentPlayer.Player1;
        currentCamera = cueStickCamera;
        currentTimer = shotTimer;
    }

    // Update is called once per frame
    void Update()
    {
        if (isWaitingForBallMovementToStop && !isGameOver)
        {
            currentTimer -= Time.deltaTime;
            if (currentTimer > 0)
            {
                return;
            }
            bool allStopped = true;
            foreach (GameObject ball in GameObject.FindGameObjectsWithTag("Ball"))
            {
                if(ball.GetComponent<Rigidbody>().velocity.magnitude >= movementThreshold)
                {
                    Debug.Log(ball.GetComponent<Rigidbody>().velocity.magnitude);
                    allStopped = false;
                    break;
                }
            }
            if (allStopped)
            {
                isWaitingForBallMovementToStop = false;
                if (willSwapPlayer || !ballPocketed)
                {
                    NextPlayerTurn();
                }
                else
                {
                    SwitchCameras();
                }
                currentTimer = shotTimer;
                ballPocketed = false;
            }
        }
    }

    public void SwitchCameras()
    {
        if (currentCamera == cueStickCamera)
        {
            cueStickCamera.enabled = false;
            overheadCamera.enabled = true;
            currentCamera = overheadCamera;
            isWaitingForBallMovementToStop = true;
        }
        else
        {
            cueStickCamera.enabled = true;
            overheadCamera.enabled = false;
            currentCamera = cueStickCamera;
            currentCamera.gameObject.GetComponent<CameraController>().ResetCamera();
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
    }

    bool Scratch()
    {
        if (currentPlayer == CurrentPlayer.Player1)
        {
            if (isWinningShotForPlayer1)
            {
                ScratchOnWinningShot("Jugador 1");
                return true;
            }
        }
        else
        {
            if (isWinningShotForPlayer2)
            {
                ScratchOnWinningShot("Jugador 2");
                return true;
            }
        }
        willSwapPlayer = true;
        return false;
    }

    void EarlyEightBall()
    {
        if (currentPlayer == CurrentPlayer.Player1)
        {
            Lose("Jugador 1 ha metido la bola 8 muy pronto y ha perdido");
        }
        else
        {
            Lose("Jugador 2 ha metido la bola 8 muy pronto y ha perdido");
        }
    }

    void ScratchOnWinningShot(string player)
    {
        Lose(player + " ha metido la blanca en el tiro final y ha perdido");
    }

    void NoMoreBalls(CurrentPlayer player)
    {
        if (player == CurrentPlayer.Player1)
        {
            isWinningShotForPlayer1 = true;
        }
        else
        {
            isWinningShotForPlayer2 = true;
        }
    }

    bool CheckBall(Ball ball)
    {
        if (ball.IsCueBall())
        {
            if (Scratch())
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else if (ball.IsEightBall())
        {
            if (currentPlayer == CurrentPlayer.Player1)
            {
                if (isWinningShotForPlayer1)
                {
                    Win("Jugador 1");
                    return true;
                }
            }
            else
            {
                if (isWinningShotForPlayer2)
                {
                    Win("Jugador 2");
                    return true;
                }
            }
            EarlyEightBall();
        }
        else
        {
            // Lógica para cuando no es ni la bola blanca ni la 8
            if (ball.IsBallRed())
            {
                player1BallsRemaining--;
                player1BallsText.text = "Bolas restantes jugador 1: " + player1BallsRemaining;
                if (player1BallsRemaining <= 0)
                {
                    isWinningShotForPlayer1 = true;
                }
                if (currentPlayer != CurrentPlayer.Player1)
                {
                    willSwapPlayer = true;
                }
            }
            else
            {
                player2BallsRemaining--;
                player2BallsText.text = "Bolas restantes jugador 2: " + player2BallsRemaining;
                if (player2BallsRemaining <= 0)
                {
                    isWinningShotForPlayer2 = true;
                }
                if (currentPlayer != CurrentPlayer.Player2)
                {
                    willSwapPlayer = true;
                }
            }
        }

        return true;
    }

    void Lose(string message)
    {
        isGameOver = true;
        popUp.gameObject.SetActive(true);
        popUp.text = message;
        buttonRestart.SetActive(true);
    }

    void Win(string player)
    {
        isGameOver = true;
        popUp.gameObject.SetActive(true);
        popUp.text = player + " ha ganado";
        buttonRestart.SetActive(true);
    }

    void NextPlayerTurn()
    {
        if (currentPlayer == CurrentPlayer.Player1)
        {
            currentPlayer = CurrentPlayer.Player2;
            currentTurnText.text = "Turno de: Jugador 2";
        }
        else
        {
            currentPlayer = CurrentPlayer.Player1;
            currentTurnText.text = "Turno de: Jugador 1";
        }
        willSwapPlayer = false;
        SwitchCameras();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Ball")
        {
            ballPocketed = true;
            if (CheckBall(other.gameObject.GetComponent<Ball>()))
            {
                Destroy(other.gameObject);
            }
            else
            {
                other.gameObject.transform.position = headPosition.position;
                other.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                other.gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            }
        }
    }
}
