using UnityEngine;
using UnityEngine.UIElements;

public class RuntimeUI : MonoBehaviour
{
    public static RuntimeUI Instance;

    private Button newGameBtn;
    private Button quitGameBtn;

    private Label scoreLabel;
    private Label deathsLabel;
    private Label GameOverText;

    private VisualElement overlayContainer;

    private int score = 0;
    private int deaths = 0;

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
        deathsLabel = uiDocument.rootVisualElement.Q("DeathsLabel") as Label;
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
        deaths = 0;
        scoreLabel.text = $"Ball caught: 0/3";
        deathsLabel.text = $"Deaths: 0/3";
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

    public void UpdateDeathsLabel()
    {
        deaths++;
        deathsLabel.text = $"Deaths: {deaths}/3";
        UpdateIsWinner();
    }

    void UpdateIsWinner()
    {
        if (deaths == 3)
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
