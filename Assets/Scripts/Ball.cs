using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Ball : MonoBehaviour
{
    public GameObject UI_Text;
    public int value = 2;
    public int hits = 0;
    public Color[] ball_colors;
    public Color[] text_colors;

    private int ballScaleState = 0;
    private Vector3 ballOriginalScale;
    public Vector3 ballAdditionalScale = new Vector3(0.2f, 0.2f, 0.2f);
    private float ballScaleSpeed = 10.0f;

    private int ballMergeState = 0;
    private float ballMergeSpeed = 10.0f;
    private GameObject ballMergeTarget;

    void Awake() {
        ballOriginalScale = gameObject.transform.localScale;
        gameObject.transform.localScale = Vector3.zero;
    }

    void Start()
    {
        hits = 0;

        // 10% chance of spawning a 4
        if(Random.Range(0, 9) == 0) {
            hits = 1;
            value = 4;
        }

        gameObject.GetComponent<SpriteRenderer>().color = ball_colors[hits];
        gameObject.GetComponent<Ball>().UI_Text.GetComponent<TextMeshPro>().color = text_colors[hits];
        UI_Text.GetComponent<TextMeshPro>().text = gameObject.GetComponent<Ball>().value.ToString();

    }

    void Update()
    {
        switch(ballScaleState) 
        {
            case 0: 
            {
                gameObject.transform.localScale = Vector3.MoveTowards(gameObject.transform.localScale, ballOriginalScale + ballAdditionalScale, Time.deltaTime * ballScaleSpeed);

                if(gameObject.transform.localScale == ballOriginalScale + ballAdditionalScale) {
                    ballScaleState++;
                }
                break;
            }
            case 1: 
            {
                gameObject.transform.localScale = Vector3.Lerp(gameObject.transform.localScale, ballOriginalScale, Time.deltaTime * ballScaleSpeed);

                if(gameObject.transform.localScale == ballOriginalScale) {
                    ballScaleState++;
                }
                break;
            }
            default: 
            {
                break;
            }
        }

        switch(ballMergeState)
        {
            case 1: 
            {
                transform.GetComponent<CircleCollider2D>().enabled = false;
                transform.GetComponent<Rigidbody2D>().Sleep();

                transform.position = Vector3.MoveTowards(transform.position, ballMergeTarget.transform.position, Time.deltaTime * ballMergeSpeed);

                if(transform.position == ballMergeTarget.transform.position)
                {
                    foreach(GameObject ball in PlayerController.playerController.balls) 
                    {
                        if(ball.GetComponent<Ball>().ballMergeTarget == gameObject) {
                            ball.GetComponent<Ball>().ballMergeTarget = ballMergeTarget;
                        }
                    }

                    Destroy(gameObject);
                    PlayerController.playerController.balls.Remove(gameObject);
                }
                break;
            }
            default: 
            {
                break;
            }
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        // Sum two balls if they collide with each other
        if(col.transform.tag == "Ball" && PlayerController.playerController.gameOver == false) 
        {
            if(value == col.gameObject.GetComponent<Ball>().value)
            {
                if(gameObject == PlayerController.playerController.lastBall && PlayerController.playerController.hitNewBall == false) {
                    return;
                }

                GameObject hitBall = col.gameObject;

                hitBall.GetComponent<Ball>().value *= 2;

                // Update player's score
                PlayerController.playerController.currentScore += hitBall.GetComponent<Ball>().value;
                PlayerController.playerController.UI_currentScore.GetComponent<Text>().text = PlayerController.playerController.currentScore.ToString();

                if(PlayerController.playerController.currentScore > PlayerController.playerController.highscore)
                {
                    PlayerController.playerController.highscore = PlayerController.playerController.currentScore;
                    PlayerPrefs.SetInt("highscore", PlayerController.playerController.currentScore);
                    PlayerController.playerController.UI_highscore.GetComponent<Text>().text = PlayerController.playerController.highscore.ToString();
                }

                hitBall.GetComponent<Ball>().UI_Text.GetComponent<TextMeshPro>().text = hitBall.GetComponent<Ball>().value.ToString();

                hitBall.GetComponent<Ball>().hits++;
                hitBall.GetComponent<SpriteRenderer>().color = ball_colors[hitBall.GetComponent<Ball>().hits];
                hitBall.GetComponent<Ball>().UI_Text.GetComponent<TextMeshPro>().color = text_colors[hitBall.GetComponent<Ball>().hits];

                ballMergeTarget = col.gameObject;
                ballMergeState  = 1;

                PlayerController.playerController.combo++;
                PlayerController.playerController.powerBarComboColor.a = 1.0f;
                PlayerController.playerController.UI_powerBarCombo.GetComponent<Text>().text = PlayerController.playerController.combo.ToString() + "X";

                float powerUseMultiplier = PlayerController.playerController.powerUseMultiplier;
                PlayerController.playerController.power += (0.08f * powerUseMultiplier) + (hitBall.GetComponent<Ball>().hits * (0.01f * powerUseMultiplier)) + ((PlayerController.playerController.combo - 1) * (0.025f * powerUseMultiplier));

                if(PlayerController.playerController.power > 1.0f)
                {
                    PlayerController.playerController.power = 1.0f;
                }

                PlayerController.playerController.ShakeCamera(0.1f, col.relativeVelocity.magnitude);
            }
        }
    }

    void OnCollisionStay2D(Collision2D col)
    {
        // Stop balls from sticking to walls
        if(col.transform.tag == "Wall") {
            Vector2 colisionPoint = col.GetContact(0).point;
            Vector2 ballPoint = transform.position;

            gameObject.GetComponent<Rigidbody2D>().AddForce((ballPoint - colisionPoint) * 5);
        }
    }
}
