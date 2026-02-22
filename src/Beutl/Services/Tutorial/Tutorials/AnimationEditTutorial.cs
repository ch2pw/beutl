using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Beutl.Animation;
using Beutl.Controls.PropertyEditors;
using Beutl.Editor.Components.GraphEditorTab;
using Beutl.Editor.Components.LibraryTab;
using Beutl.Editor.Components.LibraryTab.ViewModels;
using Beutl.Editor.Components.LibraryTab.Views;
using Beutl.Engine;
using Beutl.Graphics;
using Beutl.Graphics.Transformation;
using Beutl.Operation;
using Beutl.Operators.Source;
using Beutl.ProjectSystem;
using Beutl.Services.PrimitiveImpls;
using Beutl.ViewModels;
using Beutl.ViewModels.Editors;

namespace Beutl.Services.Tutorials;

public static class AnimationEditTutorial
{
    public const string TutorialId = "animation-edit";

    private static EditViewModel? GetEditViewModel()
    {
        return EditorService.Current.SelectedTabItem.Value?.Context.Value as EditViewModel;
    }

    private static TopLevel? GetTopLevel()
    {
        if (Application.Current?.ApplicationLifetime
            is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop)
        {
            return desktop.MainWindow;
        }

        return null;
    }

    public static TutorialDefinition Create()
    {
        IDisposable? step1Subscription = null;
        IDisposable? step2Subscription = null;
        IDisposable? step3Subscription = null;
        IDisposable? step4Subscription = null;
        IDisposable? step5Subscription = null;
        IDisposable? step10Subscription = null;
        IDisposable? step11Subscription = null;

        return new TutorialDefinition
        {
            Id = TutorialId,
            Title = TutorialStrings.Tutorial_AnimationEdit_Title,
            Description = TutorialStrings.Tutorial_AnimationEdit_Description,
            Trigger = TutorialTrigger.Manual,
            Priority = 20,
            Category = "advanced",
            CanStart = () =>
            {
                var editVm = GetEditViewModel();
                if (editVm == null) return false;

                var tab = editVm.FindToolTab<LibraryTabViewModel>() ?? new LibraryTabViewModel(editVm);
                editVm.OpenToolTab(tab);
                return true;
            },
            FulfillPrerequisites = () =>
            {
                // プロジェクトが開いていない場合、新規プロジェクトを作成
                if (ProjectService.Current.CurrentProject.Value == null)
                {
                    string tutorialDir = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                        ".beutl", "tmp", "tutorials");
                    Directory.CreateDirectory(tutorialDir);

                    string projectName = $"AnimationTutorial_{DateTime.Now:yyyyMMddHHmmss}";

                    Project? project = ProjectService.Current.CreateProject(
                        width: 1920,
                        height: 1080,
                        framerate: 30,
                        samplerate: 44100,
                        name: projectName,
                        location: tutorialDir,
                        disableTutorial: true);

                    if (project == null)
                    {
                        return Task.FromResult(false);
                    }
                }

                // シーンを開く
                Project? currentProject = ProjectService.Current.CurrentProject.Value;
                if (currentProject == null) return Task.FromResult(false);

                Scene? scene = currentProject.Items.OfType<Scene>().FirstOrDefault();
                if (scene != null)
                {
                    EditorService.Current.ActivateTabItem(scene);
                }

                EditViewModel? editVm = GetEditViewModel();
                if (editVm == null) return Task.FromResult(false);

                editVm.AddElement(new ElementDescription(
                    Start: TimeSpan.Zero,
                    Length: TimeSpan.FromSeconds(5),
                    Layer: 0,
                    InitialOperator: typeof(EllipseOperator)));

                return Task.FromResult(true);
            },
            Steps =
            [
                // Step 1: Select the element in the timeline
                new TutorialStep
                {
                    Id = "animation-select-element",
                    Title = TutorialStrings.Tutorial_AnimationEdit_Step1_Title,
                    Content = TutorialStrings.Tutorial_AnimationEdit_Step1_Content,
                    TargetElements =
                        [new TargetElementDefinition { ToolTabType = typeof(TimelineTabExtension), IsPrimary = true }],
                    PreferredPlacement = TutorialStepPlacement.Top,
                    IsActionRequired = true,
                    OnShown = () =>
                    {
                        EditViewModel? editVm = GetEditViewModel();
                        if (editVm == null) return;

                        // Already selected?
                        if (editVm.SelectedObject.Value != null)
                        {
                            Dispatcher.UIThread.Post(() => TutorialService.Current.AdvanceStep());
                            return;
                        }

                        step1Subscription = editVm.SelectedObject.Subscribe(obj =>
                        {
                            if (obj != null)
                            {
                                Dispatcher.UIThread.Post(() => TutorialService.Current.AdvanceStep());
                            }
                        });
                    },
                    OnDismissed = () =>
                    {
                        step1Subscription?.Dispose();
                        step1Subscription = null;
                    },
                },

                // Step 2: Add Transform via + button
                new TutorialStep
                {
                    Id = "animation-add-translate",
                    Title = TutorialStrings.Tutorial_AnimationEdit_Step2_Title,
                    Content = TutorialStrings.Tutorial_AnimationEdit_Step2_Content,
                    TargetElements =
                        [new TargetElementDefinition { ElementResolver = FindTransformEditor, IsPrimary = true }],
                    PreferredPlacement = TutorialStepPlacement.Left,
                    IsActionRequired = true,
                    OnShown = () =>
                    {
                        EditViewModel? editVm = GetEditViewModel();
                        if (editVm == null) return;

                        Drawable? drawable = GetDrawable(editVm);
                        if (drawable == null) return;

                        // Already has TranslateTransform?
                        if (GetTranslateTransform(drawable) != null)
                        {
                            Dispatcher.UIThread.Post(() => TutorialService.Current.AdvanceStep());
                            return;
                        }

                        // Transformプロパティの変更を監視
                        if (drawable.Transform.CurrentValue is TransformGroup group)
                        {
                            void Handler(Transform t)
                            {
                                if (t is TranslateTransform)
                                {
                                    Dispatcher.UIThread.Post(() => TutorialService.Current.AdvanceStep());
                                }
                            }

                            group.Children.Attached += Handler;
                            step2Subscription = Disposable.Create(() => group.Children.Attached -= Handler);
                        }
                    },
                    OnDismissed = () =>
                    {
                        step2Subscription?.Dispose();
                        step2Subscription = null;
                    },
                },

                // Step 3: Enable animation for X property
                new TutorialStep
                {
                    Id = "animation-enable-x",
                    Title = TutorialStrings.Tutorial_AnimationEdit_Step3_Title,
                    Content = TutorialStrings.Tutorial_AnimationEdit_Step3_Content,
                    TargetElements =
                    [
                        new TargetElementDefinition
                        {
                            ElementResolver = FindTranslateTransformXPropertyEditor, IsPrimary = true
                        }
                    ],
                    PreferredPlacement = TutorialStepPlacement.Bottom,
                    IsActionRequired = true,
                    OnShown = () =>
                    {
                        EditViewModel? editVm = GetEditViewModel();
                        if (editVm == null) return;

                        TranslateTransform? translateTransform = GetTranslateTransform(GetDrawable(editVm));
                        if (translateTransform == null) return;

                        if (translateTransform.X.Animation != null)
                        {
                            Dispatcher.UIThread.Post(() => TutorialService.Current.AdvanceStep());
                        }
                        else if (translateTransform.X is AnimatableProperty<float> animatableProp)
                        {
                            void Handler(IAnimation<float>? anm)
                            {
                                if (anm != null)
                                {
                                    Dispatcher.UIThread.Post(() => TutorialService.Current.AdvanceStep());
                                }
                            }

                            animatableProp.AnimationChanged += Handler;
                            step3Subscription = Disposable.Create(() => animatableProp.AnimationChanged -= Handler);
                        }
                    },
                    OnDismissed = () =>
                    {
                        step3Subscription?.Dispose();
                        step3Subscription = null;
                    },
                },

                // Step 4: Add keyframe
                new TutorialStep
                {
                    Id = "animation-add-keyframe",
                    Title = TutorialStrings.Tutorial_AnimationEdit_Step4_Title,
                    Content = TutorialStrings.Tutorial_AnimationEdit_Step4_Content,
                    TargetElements =
                    [
                        new TargetElementDefinition
                        {
                            ElementResolver = FindTranslateTransformXPropertyEditor, IsPrimary = true
                        }
                    ],
                    PreferredPlacement = TutorialStepPlacement.Bottom,
                    IsActionRequired = true,
                    OnShown = () =>
                    {
                        EditViewModel? editVm = GetEditViewModel();
                        if (editVm == null) return;

                        Element? element = editVm.Scene.Children
                            .FirstOrDefault(e => e.Operation.Children.OfType<EllipseOperator>().Any());
                        if (element == null) return;

                        TranslateTransform? translateTransform = GetTranslateTransform(GetDrawable(editVm));
                        if (translateTransform == null) return;

                        if (translateTransform.X.Animation is not KeyFrameAnimation<float> animation) return;

                        // Move playhead forward
                        if (animation.KeyFrames.Count >= 2)
                        {
                            editVm.CurrentTime.Value = animation.KeyFrames[1].KeyTime + element.Start;
                            Dispatcher.UIThread.Post(() => TutorialService.Current.AdvanceStep());
                        }
                        else
                        {
                            editVm.CurrentTime.Value = element.Start + TimeSpan.FromSeconds(2);

                            void Handler(IKeyFrame _)
                            {
                                if (animation.KeyFrames.Count >= 2)
                                {
                                    Dispatcher.UIThread.Post(() => TutorialService.Current.AdvanceStep());
                                }
                            }

                            animation.KeyFrames.Attached += Handler;
                            step4Subscription = Disposable.Create(() => animation.KeyFrames.Attached -= Handler);
                        }
                    },
                    OnDismissed = () =>
                    {
                        step4Subscription?.Dispose();
                        step4Subscription = null;
                    },
                },

                // Step 5: Drag and drop easing
                new TutorialStep
                {
                    Id = "animation-drag-easing",
                    Title = TutorialStrings.Tutorial_AnimationEdit_Step5_Title,
                    Content = TutorialStrings.Tutorial_AnimationEdit_Step5_Content,
                    TargetElements =
                    [
                        new TargetElementDefinition { ToolTabType = typeof(GraphEditorTabExtension), IsPrimary = true },
                        new TargetElementDefinition { ToolTabType = typeof(LibraryTabExtension) },
                    ],
                    PreferredPlacement = TutorialStepPlacement.Top,
                    IsActionRequired = true,
                    OnShown = () =>
                    {
                        EditViewModel? editVm = GetEditViewModel();
                        if (editVm == null) return;

                        // Set LibraryTab to Easings tab via View's TabStrip
                        Dispatcher.UIThread.Post(() =>
                        {
                            TopLevel? topLevel = GetTopLevel();
                            var tabStrip = topLevel?.GetVisualDescendants()
                                .OfType<LibraryTabView>()
                                .FirstOrDefault()?.tabStrip;

                            tabStrip?.SelectedIndex = 1; // 0:Search, 1:Easings, 2:Library, 3:Nodes
                        }, DispatcherPriority.Loaded);

                        // Monitor easing change
                        TranslateTransform? translateTransform = GetTranslateTransform(GetDrawable(editVm));
                        if (translateTransform?.X.Animation is not KeyFrameAnimation<float> animation) return;

                        var disposables = new CompositeDisposable();

                        void Handler(IKeyFrame _)
                        {
                            Dispatcher.UIThread.Post(() => TutorialService.Current.AdvanceStep());
                        }

                        void EasingChangedHandler()
                        {
                            Dispatcher.UIThread.Post(() => TutorialService.Current.AdvanceStep());
                        }

                        foreach (var item in animation.KeyFrames)
                        {
                            item.GetPropertyChangedObservable(KeyFrame.EasingProperty)
                                .Subscribe(_ => EasingChangedHandler())
                                .DisposeWith(disposables);
                        }

                        animation.KeyFrames.Attached += Handler;
                        Disposable.Create(() => animation.KeyFrames.Attached -= Handler)
                            .DisposeWith(disposables);
                        step5Subscription = disposables;
                    },
                    OnDismissed = () =>
                    {
                        step5Subscription?.Dispose();
                        step5Subscription = null;
                    },
                },

                // Step 6: Graph editor overview
                new TutorialStep
                {
                    Id = "animation-graph-intro",
                    Title = TutorialStrings.Tutorial_AnimationEdit_Step6_Title,
                    Content = TutorialStrings.Tutorial_AnimationEdit_Step6_Content,
                    TargetElements =
                    [
                        new TargetElementDefinition { ToolTabType = typeof(GraphEditorTabExtension), IsPrimary = true }
                    ],
                    PreferredPlacement = TutorialStepPlacement.Top,
                },

                // Step 7: Move keyframe intro
                new TutorialStep
                {
                    Id = "animation-move-keyframe",
                    Title = TutorialStrings.Tutorial_AnimationEdit_Step7_Title,
                    Content = TutorialStrings.Tutorial_AnimationEdit_Step7_Content,
                    TargetElements =
                    [
                        new TargetElementDefinition { ToolTabType = typeof(GraphEditorTabExtension), IsPrimary = true }
                    ],
                    PreferredPlacement = TutorialStepPlacement.Top,
                },

                // Step 8: Preview animation
                new TutorialStep
                {
                    Id = "animation-play-preview",
                    Title = TutorialStrings.Tutorial_AnimationEdit_Step8_Title,
                    Content = TutorialStrings.Tutorial_AnimationEdit_Step8_Content,
                    TargetElements = [new TargetElementDefinition { ElementName = "Player", IsPrimary = true }],
                    PreferredPlacement = TutorialStepPlacement.Bottom,
                    OnShown = () =>
                    {
                        EditViewModel? editVm = GetEditViewModel();
                        if (editVm == null) return;

                        Element? element = editVm.Scene.Children
                            .FirstOrDefault(e => e.Operation.Children.OfType<EllipseOperator>().Any());
                        if (element != null)
                        {
                            editVm.Scene.Duration = element.Range.End + TimeSpan.FromSeconds(1);
                            editVm.CurrentTime.Value = element.Start;
                        }
                    },
                },

                // Step 9: Copy all keyframes intro
                new TutorialStep
                {
                    Id = "animation-copy-all",
                    Title = TutorialStrings.Tutorial_AnimationEdit_Step9_Title,
                    Content = TutorialStrings.Tutorial_AnimationEdit_Step9_Content,
                    TargetElements =
                    [
                        new TargetElementDefinition { ToolTabType = typeof(GraphEditorTabExtension), IsPrimary = true }
                    ],
                    PreferredPlacement = TutorialStepPlacement.Top,
                },

                // Step 10: Enable animation for Y property
                new TutorialStep
                {
                    Id = "animation-enable-y",
                    Title = TutorialStrings.Tutorial_AnimationEdit_Step10_Title,
                    Content = TutorialStrings.Tutorial_AnimationEdit_Step10_Content,
                    TargetElements =
                    [
                        new TargetElementDefinition
                        {
                            ElementResolver = FindTranslateTransformYPropertyEditor, IsPrimary = true
                        }
                    ],
                    PreferredPlacement = TutorialStepPlacement.Bottom,
                    IsActionRequired = true,
                    OnShown = () =>
                    {
                        EditViewModel? editVm = GetEditViewModel();
                        if (editVm == null) return;

                        TranslateTransform? translateTransform = GetTranslateTransform(GetDrawable(editVm));
                        if (translateTransform == null) return;

                        if (translateTransform.Y.Animation != null)
                        {
                            Dispatcher.UIThread.Post(() => TutorialService.Current.AdvanceStep());
                        }
                        else if (translateTransform.Y is AnimatableProperty<float> animatableProp)
                        {
                            void Handler(IAnimation<float>? anm)
                            {
                                if (anm != null)
                                {
                                    Dispatcher.UIThread.Post(() => TutorialService.Current.AdvanceStep());
                                }
                            }

                            animatableProp.AnimationChanged += Handler;
                            step10Subscription = Disposable.Create(() => animatableProp.AnimationChanged -= Handler);
                        }
                    },
                    OnDismissed = () =>
                    {
                        step10Subscription?.Dispose();
                        step10Subscription = null;
                    },
                },

                // Step 11: Paste animation
                new TutorialStep
                {
                    Id = "animation-paste",
                    Title = TutorialStrings.Tutorial_AnimationEdit_Step11_Title,
                    Content = TutorialStrings.Tutorial_AnimationEdit_Step11_Content,
                    TargetElements =
                    [
                        new TargetElementDefinition { ToolTabType = typeof(GraphEditorTabExtension), IsPrimary = true }
                    ],
                    PreferredPlacement = TutorialStepPlacement.Top,
                    IsActionRequired = true,
                    OnShown = () =>
                    {
                        EditViewModel? editVm = GetEditViewModel();
                        if (editVm == null) return;

                        TranslateTransform? translateTransform = GetTranslateTransform(GetDrawable(editVm));
                        if (translateTransform?.Y.Animation is KeyFrameAnimation<float> animation)
                        {
                            // Already has keyframes?
                            if (animation.KeyFrames.Count >= 2)
                            {
                                Dispatcher.UIThread.Post(() => TutorialService.Current.AdvanceStep());
                                return;
                            }

                            void Handler(IKeyFrame _)
                            {
                                if (animation.KeyFrames.Count >= 2)
                                {
                                    Dispatcher.UIThread.Post(() => TutorialService.Current.AdvanceStep());
                                }
                            }

                            animation.KeyFrames.Attached += Handler;
                            step11Subscription = Disposable.Create(() => animation.KeyFrames.Attached -= Handler);
                        }
                    },
                    OnDismissed = () =>
                    {
                        step11Subscription?.Dispose();
                        step11Subscription = null;
                    },
                },

                // Step 12: Final preview
                new TutorialStep
                {
                    Id = "animation-final-play",
                    Title = TutorialStrings.Tutorial_AnimationEdit_Step12_Title,
                    Content = TutorialStrings.Tutorial_AnimationEdit_Step12_Content,
                    TargetElements = [new TargetElementDefinition { ElementName = "Player", IsPrimary = true }],
                    PreferredPlacement = TutorialStepPlacement.Bottom,
                    OnShown = () =>
                    {
                        EditViewModel? editVm = GetEditViewModel();
                        if (editVm == null) return;

                        Element? element = editVm.Scene.Children
                            .FirstOrDefault(e => e.Operation.Children.OfType<EllipseOperator>().Any());
                        if (element != null)
                        {
                            editVm.CurrentTime.Value = element.Start;
                        }
                    },
                },

                // Step 13: Complete
                new TutorialStep
                {
                    Id = "animation-complete",
                    Title = TutorialStrings.Tutorial_AnimationEdit_Step13_Title,
                    Content = TutorialStrings.Tutorial_AnimationEdit_Step13_Content,
                    PreferredPlacement = TutorialStepPlacement.Center,
                },
            ]
        };
    }

    private static Control? FindTransformEditor()
    {
        TopLevel? topLevel = GetTopLevel();
        return topLevel?.GetVisualDescendants()
            .OfType<Views.Editors.TransformEditor>()
            .FirstOrDefault(c =>
                c.DataContext is TransformEditorViewModel vm &&
                (vm.GetService(typeof(Element)) as Element)?.Operation.Children.OfType<EllipseOperator>().Any() ==
                true);
    }

    private static Control? FindTranslateTransformXPropertyEditor()
    {
        TopLevel? topLevel = GetTopLevel();
        return topLevel?.GetVisualDescendants()
            .OfType<NumberEditor<float>>()
            .FirstOrDefault(c =>
                c.DataContext is BaseEditorViewModel vm &&
                vm.PropertyAdapter.GetEngineProperty() is IProperty prop &&
                prop.GetOwnerObject() is TranslateTransform &&
                prop.Name == nameof(TranslateTransform.X));
    }

    private static Control? FindTranslateTransformYPropertyEditor()
    {
        TopLevel? topLevel = GetTopLevel();
        return topLevel?.GetVisualDescendants()
            .OfType<NumberEditor<float>>()
            .FirstOrDefault(c =>
                c.DataContext is BaseEditorViewModel vm &&
                vm.PropertyAdapter.GetEngineProperty() is IProperty prop &&
                prop.GetOwnerObject() is TranslateTransform &&
                prop.Name == nameof(TranslateTransform.Y));
    }

    private static Drawable? GetDrawable(EditViewModel? editVm)
    {
        if (editVm == null) return null;
        Element? element = editVm.SelectedObject.Value as Element;
        element ??= editVm.Scene.Children.FirstOrDefault(e =>
            e.Operation.Children
                .OfType<IPublishOperator>()
                .Any(op => op.Value is Drawable));

        if (element?.Operation.Children.OfType<IPublishOperator>().FirstOrDefault() is { Value: Drawable value })
        {
            return value;
        }

        return null;
    }

    private static TranslateTransform? GetTranslateTransform(Drawable? drawable)
    {
        if (drawable?.Transform.CurrentValue is TransformGroup group)
        {
            return group.Children.OfType<TranslateTransform>().FirstOrDefault();
        }

        return drawable?.Transform.CurrentValue as TranslateTransform;
    }
}
