using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using GooglePlayGames;
using UnityEngine.SocialPlatforms;

public class PlayerController : MonoBehaviour
{
    // prefabs
    public GameObject ball;
    public GameObject lastBall;
    public GameObject poolCue;
    public GameObject cueLine;

    // config
    public float powerUseMultiplier = 1.0f;
    public float power = 1.0f;
    public float hitPower = 0.75f;
    public float hitPowerMultiplier = 100.0f;
    public float minimumVelocityBeforeNewSpawn = 2.5f;
    public float minimumRadiusBeforeNewSpawn = 2.5f;

    // scores
    public int currentScore;
    public int highscore;

    // UI
    public GameObject UI_powerSlider;
    public GameObject UI_powerBar;

    public GameObject UI_powerBarCombo;
    public Color powerBarComboColor;
    public int combo;

    public GameObject UI_currentScore;
    public GameObject UI_highscore;

    public GameObject UI_restart;
    public GameObject UI_gameOver;

    // hitting/dragging
    public bool gameOver = false;
    private Vector3 mouseStartPositionWorld;
    private Vector3 mouseEndPositionWorld;
    private bool dragging = false;
    public bool hitNewBall = false;

    private float hitDistance;
    private Vector2 hitDirection;

    public List<GameObject> balls;

    // Pool Cue
    public bool poolCueHit = false;
    private float previousCueAngle;

    public static PlayerController playerController;

    // Camera shake
    public Vector3 originalCameraPosition;
    public float cameraShakePower = 0.25f;
    public float cameraShakeRelativePower = 0.25f;
    public bool cameraShake = false;

    void Awake() 
    {
        playerController = this;
        poolCue.GetComponent<SpriteRenderer>().enabled = false;
        cueLine.GetComponent<LineRenderer>().enabled = false;

        powerBarComboColor = UI_powerBarCombo.GetComponent<Text>().color;
        powerBarComboColor.a = 0.0f;
        UI_powerBarCombo.GetComponent<Text>().color = powerBarComboColor;

        originalCameraPosition = Camera.main.transform.position;

        currentScore = 0;
        highscore = PlayerPrefs.GetInt("highscore");

        UI_currentScore.GetComponent<Text>().text = currentScore.ToString();
        UI_highscore.GetComponent<Text>().text = highscore.ToString();
    }

    // Start is called before the first frame update
    void Start()
    {
        dragging = false;

        StartCoroutine(SpawnBall(0.0f));
        StartCoroutine(SpawnBall(0.5f));
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0) && hitNewBall == false && gameOver == false)
        {
            if (EventSystem.current.IsPointerOverGameObject() || 
            EventSystem.current.IsPointerOverGameObject(0) ||
            EventSystem.current.IsPointerOverGameObject(1) ||
            EventSystem.current.IsPointerOverGameObject(2) ||
            EventSystem.current.IsPointerOverGameObject(3))
            {
                return;
            }

            dragging = true;
        } 

        if(Input.GetMouseButtonUp(0) && dragging == true) 
        {
            mouseEndPositionWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            hitDirection = (mouseStartPositionWorld - mouseEndPositionWorld).normalized;

            dragging = false;

            // perform the hit
            //hitPower = UI_powerSlider.GetComponent<Slider>().value;
            hitPower = 0.75f;
            poolCue.GetComponent<SpriteRenderer>().enabled = false;
            cueLine.GetComponent<LineRenderer>().enabled = false;
            poolCueHit = false;
            lastBall.GetComponent<Rigidbody2D>().AddForce(hitDirection * hitPower * hitPowerMultiplier, ForceMode2D.Impulse);
            hitNewBall = true;
            power -= 0.1f * powerUseMultiplier;
            combo = 0;
        }

        if(dragging)
        {
            mouseStartPositionWorld = lastBall.transform.position;
            mouseStartPositionWorld.z = Camera.main.transform.position.z;

            // position the pool cue at the position of the ball
            Vector3 poolCuePosition = mouseStartPositionWorld;

            // make the pool cue face the ball
            poolCue.transform.right = mouseStartPositionWorld - Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // rotate pool cue at a certain distance around the ball
            Vector3 direction = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - mouseStartPositionWorld).normalized;
            poolCuePosition += direction;

            // position the pool cue at the position of the ball
            poolCuePosition.z = 0;
            poolCue.transform.position = poolCuePosition;

            // start drawing the pool cue only when it is set into place
            poolCue.GetComponent<SpriteRenderer>().enabled = true;


            // raycast settings
            Vector3 rayDirection = cueLine.transform.TransformDirection(Vector3.right);

            RaycastHit2D hit = Physics2D.Raycast(cueLine.transform.position - (direction * 2), rayDirection, 20.0f);
            Debug.DrawRay(cueLine.transform.position - (direction * 2), rayDirection, Color.blue);

            if(dragging && hit) {
                cueLine.GetComponent<LineRenderer>().SetPosition(0, cueLine.transform.position - (direction * 2));
                cueLine.GetComponent<LineRenderer>().SetPosition(1, hit.point);

                // Reflection line
                //cueLine.GetComponent<LineRenderer>().SetPosition(2, hit.point + (Vector2.Reflect(rayDirection, hit.normal) * 2));
            }

            cueLine.GetComponent<LineRenderer>().enabled = true;
        }

        // manage new ball spawning
        bool canSpawn = true;
        foreach(GameObject ball in balls) 
        {
            if(ball.GetComponent<Rigidbody2D>().velocity.magnitude > minimumVelocityBeforeNewSpawn) {
                canSpawn = false;
            }
        }

        if(canSpawn && hitNewBall) 
        {
            hitNewBall = false;
            combo = 0;

            if(power <= 0.0f) 
            {
                StartCoroutine(GameOver(2.0f));
            }
            else 
            {
                StartCoroutine(SpawnBall(0.0f));
            }
        }
    }

    void FixedUpdate() 
    {
        UI_powerBar.GetComponent<Image>().fillAmount = Mathf.Lerp(UI_powerBar.GetComponent<Image>().fillAmount, power, Time.deltaTime * 5);

        powerBarComboColor.a = Mathf.MoveTowards(powerBarComboColor.a, 0.0f, Time.deltaTime);
        UI_powerBarCombo.GetComponent<Text>().color = powerBarComboColor;

        // camera shake
        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, originalCameraPosition, Time.deltaTime * 5);

        if(cameraShake) 
        {
            Vector3 cameraShakePosition = originalCameraPosition;
            cameraShakePosition.x += Random.Range(-cameraShakePower * cameraShakeRelativePower, cameraShakePower * cameraShakeRelativePower);
            cameraShakePosition.y += Random.Range(-cameraShakePower * cameraShakeRelativePower, cameraShakePower * cameraShakeRelativePower);

            Camera.main.transform.position = cameraShakePosition;
        }
    }

    private IEnumerator SpawnBall(float delay)
    {
        yield return new WaitForSeconds(delay);

        Vector3 ballPosition = Vector3.zero;

        int passes = 0;
        bool canSpawn = false;
        while(canSpawn == false)
        {
            ballPosition.x = Random.Range(-4.0f, 4.0f);
            ballPosition.y = Random.Range(-4.0f, 4.0f);
            ballPosition.z = 0;

            bool tooClose = false;
            foreach(GameObject ball in balls) {
                if(Vector2.Distance(ballPosition, ball.transform.position) < 2.5f)
                {
                    tooClose = true;
                }
            }

            if(tooClose == false)
            {
                canSpawn = true;
            }

            // prevent infinite loops and crashing
            passes++;
            if(passes == 1000) {
                Debug.Log("found no more free spaces, theoretically Game Over");
                break;
            }
        }

        lastBall = Instantiate(ball, ballPosition, Quaternion.identity);
        balls.Add(lastBall);
    }

    public void ShakeCamera(float time, float power) {
        cameraShake = true;
        cameraShakeRelativePower = 1.0f * (power / 22.0f);
        StartCoroutine(StopShake(time));
    }

    private IEnumerator StopShake(float delay)
    {
        yield return new WaitForSeconds(delay);
        cameraShake = false;
    }

    public void Restart() {
        UI_restart.SetActive(true);
    }

    public void RestartYes() {
        SceneManager.LoadScene(0);
    }

    public void RestartNo() {
        UI_restart.SetActive(false);
    }

    private IEnumerator GameOver(float delay)
    {
        UI_gameOver.SetActive(true);
        gameOver = true;

        GoogleAdMob.googleAdMob.RequestInterstitial();

        Social.ReportScore(highscore, "CgkI1aW6uKgFEAIQAQ", (bool success) => {

        });

        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(0);
    }

    public void OpenLeaderboard() {
        Social.localUser.Authenticate((bool success) => {
            if(success) {
                PlayGamesPlatform.Instance.ShowLeaderboardUI("CgkI1aW6uKgFEAIQAQ");
            }
        });
    }
}
