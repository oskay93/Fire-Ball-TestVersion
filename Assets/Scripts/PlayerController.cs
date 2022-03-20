using UnityEngine;

public class PlayerController : MonoBehaviour {

    #region Variables
    [SerializeField]
    private GameObject[] blockPrefabs;
    [SerializeField]
    private HingeJoint hJoint;
    [SerializeField]
    private LineRenderer lRenderer;
    [SerializeField]
    private Rigidbody playerRigidbody;
    [SerializeField]
    private GameObject pointPrefab;
    [SerializeField]
    private GUIController guiController;

    private float score;
    float _tempScore;

    private bool gameOver = false;

    public static PlayerController Instance;

    bool _isPlayerReleased;
    public bool IsPlayerReleased { get { return _isPlayerReleased; } }

    bool _isPlaying;
    public bool IsPlaying { get { return _isPlaying; } }

    [SerializeField] float _playerSpeed = 50;
    #endregion

    void Start()
    {
        Instance = this;
        BlockCreator.GetSingleton().Initialize(30, blockPrefabs, pointPrefab);
        FindRelativePosForHingeJoint(new Vector3(0,10,0));
	}
	
    public void FindRelativePosForHingeJoint(Vector3 blockPosition)
    {
        transform.rotation = new Quaternion(0, 0, 0, 0);
        blockPosition = new Vector3(blockPosition.x, blockPosition.y - 5, blockPosition.z); // 5 is coming from half of the blocks height which is 10
        hJoint.anchor = (blockPosition - transform.position);
        lRenderer.SetPosition(1, hJoint.anchor);
        lRenderer.enabled = true;
    }

    public void PointerDown()
    {
        Debug.Log("Pointer Down");
        if (!_isPlaying)
        {
            _isPlaying = true;
            playerRigidbody.isKinematic = false;
            guiController.ShowHoldStartText(false);
        }

        _isPlayerReleased = false;

        if (hJoint == null)
        {
            hJoint = gameObject.AddComponent<HingeJoint>();
            FindRelativePosForHingeJoint(BlockCreator.GetSingleton().GetRelativeBlock(transform.position.z).position);
        }

        #region Joint Motor Settings
        JointMotor motor = hJoint.motor;
        motor.targetVelocity = -(_playerSpeed * 2);
        motor.force = _playerSpeed;
        hJoint.motor = motor;
        hJoint.useMotor = true;
        #endregion
    }

    public void PointerUp()
    {
        Debug.Log("Pointer Up");
        _isPlayerReleased = true;

        Destroy(GetComponent<HingeJoint>());
        lRenderer.enabled = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Prevents After GameOver Collisions
        if(collision.gameObject.tag.Equals("Block") && !gameOver)
        {
            PointerUp(); //Finishes the game here to stoping holding behaviour
            gameOver = true; // Sets Game Over Flag To Tell The Game It Is Game Over
            guiController.scoreText.text = WriteScoreInFormat(score); // Write Score To Game Over Screen

            #region New HighScore System
            float _highScore = PlayerPrefs.GetFloat("HighScore"); // Get HighScore From Prefab,If No HighScore Exist Then It Is Equal To Zero
            if (score > _highScore) _highScore = score; // Compare Score And HighScore
            PlayerPrefs.SetFloat("HighScore", _highScore); // Set Prefab 'HighScore'
            guiController.highscoreText.text = "HighestScore: " + WriteScoreInFormat(_highScore); // Write HighScore Text
            guiController.ShowGameOverPanel(true); // Activate Game Over Panel
            #endregion
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag.Equals("Point"))
        {
            if(Vector3.Distance(transform.position, other.gameObject.transform.position) < .5f)
            {
                score += 10f;
            }
            else
            {
                score += 5f;
            }
            other.gameObject.SetActive(false);
        }
    }
    private void FixedUpdate()
    {
       SetScore();
    }
    public void SetScore()
    {
        if (_tempScore < score) _tempScore = score;
        score += playerRigidbody.velocity.z * Time.fixedDeltaTime * 0.1f;
        guiController.realtimeScoreText.text = WriteScoreInFormat(_tempScore);
    }

    public string WriteScoreInFormat(float newScore)
    {
        return newScore.ToString("0.00");
    }
}