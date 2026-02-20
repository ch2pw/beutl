namespace Beutl.Services.Tutorials;

public interface ITutorialService
{
    IObservable<TutorialState?> CurrentTutorial { get; }

    void Register(TutorialDefinition tutorial);

    Task StartTutorial(string tutorialId);

    void AdvanceStep();

    void PreviousStep();

    void CancelTutorial();

    bool IsTutorialCompleted(string tutorialId);

    IReadOnlyList<TutorialDefinition> GetAvailableTutorials();
}
