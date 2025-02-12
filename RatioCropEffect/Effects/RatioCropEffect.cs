using System.ComponentModel.DataAnnotations;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Exo;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Plugin.Effects;

namespace RatioCropEffect.Effects
{
    [VideoEffect("クリッピング（比率）", ["合成"], ["clipping", "crop", "クロップ", "プラグイン", "plugin"], isAviUtlSupported:false, isEffectItemSupported:false)]
    internal class RatioCropEffect : VideoEffectBase
    {
        public override string Label => $"クリッピング 上{Top.GetValue(0, 1, 30):F0}%, 下{Bottom.GetValue(0, 1, 30):F0}%, 左{Left.GetValue(0, 1, 30):F0}%, 右{Right.GetValue(0, 1, 30):F0}%";

        [Display(GroupName = "クリッピング（比率）", Name = "上", Description = "上")]
        [AnimationSlider("F2", "%", 0, 100)]
        public Animation Top { get; } = new Animation(0, 0, 100);

        [Display(GroupName = "クリッピング（比率）", Name = "下", Description = "下")]
        [AnimationSlider("F2", "%", 0, 100)]
        public Animation Bottom { get; } = new Animation(0, 0, 100);

        [Display(GroupName = "クリッピング（比率）", Name = "左", Description = "左")]
        [AnimationSlider("F2", "%", 0, 100)]
        public Animation Left { get; } = new Animation(0, 0, 100);

        [Display(GroupName = "クリッピング（比率）", Name = "右", Description = "右")]
        [AnimationSlider("F2", "%", 0, 100)]
        public Animation Right { get; } = new Animation(0, 0, 100);

        [Display(GroupName = "クリッピング（比率）", Name = "中央寄せ", Description = "中央寄せ")]
        [ToggleSlider]
        public bool Centering { get => centering; set => Set(ref centering, value); }
        bool centering = true;
        
        public override IEnumerable<string> CreateExoVideoFilters(int keyFrameIndex, ExoOutputDescription exoOutputDescription)
        {
            return　[];
        }

        public override IVideoEffectProcessor CreateVideoEffect(IGraphicsDevicesAndContext devices)
        {
            return new RatioCropEffectProcessor(devices, this);
        }

        protected override IEnumerable<IAnimatable> GetAnimatables() => [Top, Bottom, Left, Right];
    }
}
