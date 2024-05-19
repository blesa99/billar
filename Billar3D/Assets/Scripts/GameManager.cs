using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.VersionControl;
using UnityEngine;

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

    [SerializeField] TextMeshProUGUI player1BallsText;
    [SerializeField] TextMeshProUGUI player2BallsText;
    [SerializeField] TextMeshProUGUI currentTurnText;
    [SerializeField] TextMeshProUGUI popUp;

    [SerializeField] GameObject restartButton;
    [SerializeField] Transform headPosition;

    // Start is called before the first frame update
    void Start()
    {
        currentPlayer = CurrentPlayer.Player1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    bool Scratch()
    {
        if(currentPlayer == CurrentPlayer.Player1)
        {
            if (isWinningShotForPlayer1)
            {
                ScratchOnWinningShot("Jugador 1");
                return true;
            }
        }
        else
        {
            if(isWinningShotForPlayer2)
            {
                ScratchOnWinningShot("Jugador 2");
                return true;
            }
        }
        NextPlayerTurn();
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
                if(isWinningShotForPlayer2)
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
            if(ball.IsBallRed())
            {
                player1BallsRemaining--;
                if (player1BallsRemaining <= 0)
                {
                    isWinningShotForPlayer1 = true;
                }
                if(currentPlayer != CurrentPlayer.Player1)
                {
                    NextPlayerTurn();
                }
            }
            else
            {
                player2BallsRemaining--;
                if(player2BallsRemaining <= 0)
                {
                    isWinningShotForPlayer2 = true;
                }
                if (currentPlayer != CurrentPlayer.Player2)
                {
                    NextPlayerTurn();
                }
            }
        }

        return true;
    }

    void Lose(string message)
    {
        popUp.gameObject.SetActive(true);
        popUp.text = message;
        restartButton.SetActive(true);
    }

    void Win(string player)
    {
        popUp.gameObject.SetActive(true);
        popUp.text = player + " ha ganado";
        restartButton.SetActive(true);
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
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Ball")
        {
            if(CheckBall(other.gameObject.GetComponent<Ball>()))
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
