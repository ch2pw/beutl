using System.ComponentModel.DataAnnotations;
using Beutl.Audio.Graph;
using Beutl.Engine;
using Beutl.Language;
using Beutl.Media.Source;

namespace Beutl.Audio;

[Display(Name = nameof(Strings.SoundGroup), ResourceType = typeof(Strings))]
public sealed partial class SoundGroup : Sound
{
    public SoundGroup()
    {
        ScanProperties<SoundGroup>();
    }

    public IListProperty<Sound> Children { get; } = Property.CreateList<Sound>();

    protected override SoundSource? GetSoundSource() => null;

    public override void Compose(AudioContext context)
    {
        if (Children.Count == 0)
        {
            context.Clear();
            return;
        }

        // このSoundGroupが1-5秒で、処理範囲が0-2秒の場合、0-1秒はそのまま通して、1-2秒はSoundGroupの処理を加える必要がある
        // そのまま通す
        foreach (var child in Children)
        {
            if (child.TimeRange.Start < TimeRange.Start)
            {
                var internalContext = new AudioContext(context.SampleRate, context.ChannelCount);
                child.Compose(internalContext);
                foreach (AudioNode node in internalContext.Nodes)
                {
                    context.AddNode(node);
                }
                foreach (var outputNode in internalContext.GetOutputNodes())
                {
                    var shiftNode = context.CreateShiftNode(child.TimeRange.Start);
                    var clipNode2 = context.CreateClipNode(
                        child.TimeRange.Start, TimeRange.Start - child.TimeRange.Start);
                    context.Connect(outputNode, shiftNode);
                    context.Connect(shiftNode, clipNode2);
                    context.MarkAsOutput(clipNode2);
                }
            }

            if (child.TimeRange.End > TimeRange.End)
            {
                var internalContext = new AudioContext(context.SampleRate, context.ChannelCount);
                child.Compose(internalContext);
                foreach (AudioNode node in internalContext.Nodes)
                {
                    context.AddNode(node);
                }
                foreach (var outputNode in internalContext.GetOutputNodes())
                {
                    var shiftNode = context.CreateShiftNode(TimeRange.End);
                    var clipNode2 = context.CreateClipNode(
                        TimeRange.End, child.TimeRange.End - TimeRange.End);
                    context.Connect(outputNode, shiftNode);
                    context.Connect(shiftNode, clipNode2);
                    context.MarkAsOutput(clipNode2);
                }
            }
        }

        // SoundGroupの処理を加える
        var mixerNode = context.CreateMixerNode();

        foreach (var child in Children)
        {
            var internalContext = new AudioContext(context.SampleRate, context.ChannelCount);
            child.Compose(internalContext);
            foreach (AudioNode node in internalContext.Nodes)
            {
                context.AddNode(node);
            }
            // 各子要素の出力ノードにShiftNodeを挿入してMixerに接続
            foreach (var outputNode in internalContext.GetOutputNodes())
            {
                // ShiftNodeでSoundGroupのStartを加算して打ち消す
                var shiftNode = context.CreateShiftNode(TimeRange.Start);
                context.Connect(outputNode, shiftNode);
                context.Connect(shiftNode, mixerNode);
            }
        }

        AudioNode currentNode = mixerNode;

        // SoundGroup全体のGainを適用
        var gainNode = context.CreateGainNode(Gain);
        context.Connect(currentNode, gainNode);
        currentNode = gainNode;

        // SoundGroup全体のEffectを適用
        if (Effect.CurrentValue != null && Effect.CurrentValue.IsEnabled)
        {
            currentNode = Effect.CurrentValue.CreateNode(context, currentNode);
        }

        // ClipNodeを作成（EffectがローカルTimeRangeを前提としているため）
        var clipNode = context.CreateClipNode(TimeRange.Start, TimeRange.Duration);
        context.Connect(currentNode, clipNode);
        context.MarkAsOutput(clipNode);
    }
}
