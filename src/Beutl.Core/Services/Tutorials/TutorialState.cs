namespace Beutl.Services.Tutorials;

public sealed class TutorialState
{
    public TutorialState(TutorialDefinition definition, int currentStepIndex)
    {
        Definition = definition;
        CurrentStepIndex = currentStepIndex;
    }

    public TutorialDefinition Definition { get; }

    public int CurrentStepIndex { get; }

    public int TotalSteps => Definition.Steps.Count;

    public TutorialStep CurrentStep => Definition.Steps[CurrentStepIndex];

    public bool IsFirstStep => CurrentStepIndex == 0;

    public bool IsLastStep => CurrentStepIndex == TotalSteps - 1;
}
