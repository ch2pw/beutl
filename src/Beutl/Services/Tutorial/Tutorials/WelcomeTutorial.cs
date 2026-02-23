using Beutl.Language;
using Beutl.Services.Tutorials;

namespace Beutl.Services.Tutorials;

public static class WelcomeTutorial
{
    public const string TutorialId = "welcome";

    public static TutorialDefinition Create()
    {
        return new TutorialDefinition
        {
            Id = TutorialId,
            Title = Strings.Tutorial_Welcome_Title,
            Description = Strings.LearnBeutlBasics,
            Trigger = TutorialTrigger.FirstRun,
            Priority = 0,
            Category = "basics",
            Steps =
            [
                new TutorialStep
                {
                    Id = "welcome-intro",
                    Title = Strings.Tutorial_Welcome_Title,
                    Content = Strings.Tutorial_Welcome_Content,
                    PreferredPlacement = TutorialStepPlacement.Center,
                },
                new TutorialStep
                {
                    Id = "welcome-menubar",
                    Title = Strings.Tutorial_Welcome_MenuBar_Title,
                    Content = Strings.Tutorial_Welcome_MenuBar_Content,
                    TargetElementName = "MenuBar",
                    PreferredPlacement = TutorialStepPlacement.Bottom,
                },
                new TutorialStep
                {
                    Id = "welcome-create",
                    Title = Strings.Tutorial_Welcome_Create_Title,
                    Content = Strings.Tutorial_Welcome_Create_Content,
                    TargetElementName = "createNewButton",
                    PreferredPlacement = TutorialStepPlacement.Bottom,
                },
            ]
        };
    }
}
