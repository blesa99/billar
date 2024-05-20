using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSetup : MonoBehaviour
{

    int redBallsRemaining = 7;
    int blueBallsRemaining = 7;
    float ballRadius;
    float ballDiameter;

    [SerializeField] GameObject ballPrefab;
    [SerializeField] Transform cueBallPosition;
    [SerializeField] Transform headBallPosition;

    // Start is called before the first frame update
    private void Awake()
    {
        ballRadius = ballPrefab.GetComponent<SphereCollider>().radius * 100f;
        ballDiameter = ballRadius * 2f;
        PlaceAllBalls();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void PlaceAllBalls()
    {
        PlaceCueBall();
        PlaceRandomBalls();
    }

    void PlaceCueBall()
    {
        GameObject ball = Instantiate(ballPrefab, cueBallPosition.position, Quaternion.identity);
        ball.GetComponent<Ball>().MakeCueBall();
    }

    void PlaceEightBall(Vector3 position)
    {
        GameObject ball = Instantiate(ballPrefab, position, Quaternion.identity);
        ball.GetComponent<Ball>().MakeEightBall();
    }

    void PlaceRandomBalls()
    {
        int NumInThisRow = 1;
        int rand;
        Vector3 firstInRowPosition = headBallPosition.position;
        Vector3 currentPosition = firstInRowPosition;

        void PlaceRedBall(Vector3 position)
        {
            GameObject ball = Instantiate(ballPrefab, position, Quaternion.identity);
            ball.GetComponent<Ball>().BallSetup(true);
            redBallsRemaining--;
        }

        void PlaceBlueBall(Vector3 position)
        {
            GameObject ball = Instantiate(ballPrefab, position, Quaternion.identity);
            ball.GetComponent<Ball>().BallSetup(false);
            blueBallsRemaining--;
        }

        // El primer bucle son las 5 filas
        for(int i = 0; i < 5; i++)
        {
            // El segundo bucle son las bolas en cada fila
            for (int j = 0; j < NumInThisRow; j++)
            {
                // Comprobamos si esta es la posición de la bola 8
                if(i == 2 && j == 1)
                {
                    PlaceEightBall(currentPosition);
                }
                // Si quedan tanto bolas rojas como azules elije aleatoriamente
                else if(redBallsRemaining > 0 && blueBallsRemaining > 0)
                {
                    rand = Random.Range(0, 2);
                    if(rand == 0)
                    {
                        PlaceRedBall(currentPosition);
                    } else
                    {
                        PlaceBlueBall(currentPosition);
                    }
                } 
                // Si solo quedan bolas rojas pone una roja
                else if(redBallsRemaining > 0)
                {
                    PlaceRedBall(currentPosition);
                } 
                // Si solo quedan bolas azules pone una azul
                else
                {
                    PlaceBlueBall(currentPosition);
                }

                // Mueve la posición actual para la siguiente bola en esta fila para la derecha
                currentPosition += new Vector3(1, 0, 0).normalized * ballDiameter;
            }

            // Cuando todas las bolas de la fila están puestas, pasa a la siguiente fila
            firstInRowPosition += Vector3.back * (Mathf.Sqrt(3) * ballRadius) + Vector3.left * ballRadius;
            currentPosition = firstInRowPosition;
            NumInThisRow++;
        }
    }
}
