using UnityEngine;
using UnityEngine.UIElements;

public class RuntimeUI : MonoBehaviour
{
    public static RuntimeUI Instance;

    private Button newGameBtn;
    private Button quitGameBtn;

    private Label scoreLabel;
    private Label livesLabel;
    private Label GameOverText;

    private VisualElement overlayContainer;

    private int score = 0;
    private int lives = 3;

    bool isWinner;

    private void Start()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        // The UXML is already instantiated by the UIDocument component
        var uiDocument = GetComponent<UIDocument>();

        newGameBtn = uiDocument.rootVisualElement.Q("NewGameBtn") as Button;
        quitGameBtn = uiDocument.rootVisualElement.Q("QuitGameBtn") as Button;

        scoreLabel = uiDocument.rootVisualElement.Q("ScoreLabel") as Label;
        livesLabel = uiDocument.rootVisualElement.Q("LivesLabel") as Label;
        GameOverText = uiDocument.rootVisualElement.Q("GameFinishedLabel") as Label;

        overlayContainer = uiDocument.rootVisualElement.Q("OverlayContainer") as VisualElement;

        newGameBtn.RegisterCallback<ClickEvent>(StartNewGame);
        quitGameBtn.RegisterCallback<ClickEvent>(QuitGame);
        }

    private void OnDisable()
    {
        newGameBtn.UnregisterCallback<ClickEvent>(StartNewGame);
        quitGameBtn.UnregisterCallback<ClickEvent>(StartNewGame);
    }

    private void StartNewGame(ClickEvent evt)
    {
        score = 0;
        lives = 3;
        scoreLabel.text = $"Ball caught: 0/3";
        livesLabel.text = $"Lives: 3/3";
        overlayContainer.style.display = DisplayStyle.None;
        Time.timeScale = 1f;

    }

    public void QuitGame(ClickEvent evt)
    {
        QuitGame();
    }

    public void UpdateScoreLabel()
    {
        score++;
        scoreLabel.text = $"Ball caught: {score}/3";
        UpdateIsWinner();
    }

    public void UpdateLivesLabel()
    {
        lives--;
        livesLabel.text = $"Lives: {lives}/3";
        UpdateIsWinner();
    }

    void UpdateIsWinner()
    {
        if (lives == 0)
        {
            isWinner = false;
            ShowOverlay();
        }
        if (score == 3)
        {
            isWinner = true;
            ShowOverlay();
        }
    }

    private void ShowOverlay()
    {
        if (isWinner)
            GameOverText.text = "You won!";
        else
            GameOverText.text = "You lose!";

            bool isVisible = overlayContainer.style.display == DisplayStyle.Flex ? true : false;

        if (isVisible)
            overlayContainer.style.display = DisplayStyle.None;
        else
            overlayContainer.style.display = DisplayStyle.Flex;

        Time.timeScale = 0f;
    }

    private void QuitGame()
    {
        Application.Quit();
    }

}
