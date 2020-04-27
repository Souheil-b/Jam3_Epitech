using UnityEngine;
using System.Collections;
using System.Collections.Generic;        //Allows us to use Lists. 
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public float levelStartDelay = 2f;
    public float turnDelay = .1f;
    public static GameManager instance = null;                //Static instance of GameManager which allows it to be accessed by any other script.
    private BoardManager boardScript;                        //Store a reference to our BoardManager which will set up the level.
    public int playerFoodPoints = 100;
    [HideInInspector] public bool playersTurn = true;

    private Text levelText;
    private GameObject levelImage;
    private int level = 1;                                  //Current level number, expressed in game as "Day 1".
    private List<Enemy> ennemies;
    private bool ennemiesMoving;
    private bool doingSteup; 

        //Awake is always called before any Start functions
    void Awake()
    {
            //Check if instance already exists
        if (instance == null)

                //if not, set instance to this
            instance = this;

            //If instance already exists and it's not this:
        else if (instance != this)

                //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);    

            //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);
        ennemies = new List<Enemy>();
            //Get a component reference to the attached BoardManager script
        boardScript = GetComponent<BoardManager>();

            //Call the InitGame function to initialize the first level 
        InitGame();
    }

    private void OnLevelWasLoaded (int index)
    {
        level++;
        InitGame();
    }

        //Initializes the game for each level.
    void InitGame()
    {
        doingSteup = true;
        levelImage = GameObject.Find("LevelImage");
        levelText = GameObject.Find("LevelText").GetComponent<Text>();
        levelText.text = "Day " + level;
        levelImage.SetActive(true);
        Invoke("HideLevelImage", levelStartDelay);

        ennemies.Clear();
            //Call the SetupScene function of the BoardManager script, pass it current level number.
        boardScript.SetupScene(level);
    }

    private void HideLevelImage()
    {
        levelImage.SetActive(false);
        doingSteup = false;
    }

    public void GameOver()
    {
        levelText.text = "After" + level + " days, you starved.";
        levelImage.SetActive(true);
        enabled = false;
    }
        //Update is called every frame.
    void Update()
    {
        if (playersTurn || ennemiesMoving || doingSteup)
            return;            
        StartCoroutine(MoveEnnemies());
    }

    public void AddEnemyToList(Enemy script)
    {
        ennemies.Add (script);
    }

    IEnumerator MoveEnnemies()
    {
        ennemiesMoving = true;
        yield return new WaitForSeconds(turnDelay);
        if (ennemies.Count == 0)
        {
            yield return new WaitForSeconds(turnDelay);
        }
        for (int i = 0; i < ennemies.Count; i++)
        {
            ennemies[i].MoveEnemy();
            yield return new WaitForSeconds(ennemies[i].moveTime);
        }
        playersTurn = true;
        ennemiesMoving = false;
    }
}