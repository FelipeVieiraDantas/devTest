using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text movesText;
    [SerializeField] private Button replayButton;
    [SerializeField] private GameObject gameOverPopup;
    
    [Header("Tests")]
    [SerializeField] private Button makeMoveButton;

    private int _currentScore = 0; //Temp variable until we do Game Manager
    private int _currentMoves = 5; //Temp variable until we do Game Manager
    

    public void MakeMove(int cost)
    {
        _currentMoves -= cost;
        UpdateMoves(_currentMoves);
        UpdateScore(1); //Todo: Score upate must come from Game Manager
        if (_currentMoves <= 0)
        {
            GameOver();
        }
    }

    public void UpdateMoves(int amount)
    {
        movesText.text = amount.ToString();
    }

    public void UpdateScore(int score)
    {
        _currentScore += score;
        scoreText.text = _currentScore.ToString();
    }

    public void GameOver()
    {
        gameOverPopup.gameObject.SetActive(true);
    }
    
    private void Start()
    {
        AddListeners();
        
        UpdateMoves(_currentMoves); //Todo this should come when Game Manager is ready
    }

    private void AddListeners()
    {
        replayButton.onClick.AddListener(OnReplayButtonClicked);
        makeMoveButton.onClick.AddListener(OnMakeMoveButtonClicked);
    }

    private void OnReplayButtonClicked()
    {
        gameOverPopup.gameObject.SetActive(false);
        
        //Todo: Restart the actual game
        _currentScore = 0;
        UpdateScore(0);
        _currentMoves = 5;
        UpdateMoves(_currentMoves);
        
    }

    private void OnMakeMoveButtonClicked()
    {
        MakeMove(1);
    }
}
