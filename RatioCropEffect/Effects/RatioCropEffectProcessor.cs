using System.Numerics;
using Vortice.Direct2D1;
using Vortice.Direct2D1.Effects;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Player.Video.Effects;

namespace RatioCropEffect.Effects
{
    internal class RatioCropEffectProcessor(IGraphicsDevicesAndContext devices, RatioCropEffect item) : VideoEffectProcessorBase(devices)
    {
        readonly ID2D1DeviceContext deviceContext = devices.DeviceContext;
        
        ID2D1Image input;
        
        bool isFirst = true;

        bool centering;

        Vector4 rect;

        AffineTransform2DInterpolationMode interpolationMode;

        Crop cropEffect;

        AffineTransform2D centeringEffect;

        protected override void setInput(ID2D1Image? input)
        {
            this.input = input;
            cropEffect.SetInput(0, input, true);
        }
        protected override ID2D1Image? CreateEffect(IGraphicsDevicesAndContext devices)
        {
            cropEffect = new Crop(devices.DeviceContext);
            disposer.Collect(cropEffect);
            centeringEffect = new AffineTransform2D(devices.DeviceContext);
            using (var image = cropEffect.Output)
            {
                centeringEffect.SetInput(0, image, true);
            }
            var output = centeringEffect.Output;
            disposer.Collect(output);
            return output;
        }
        public override DrawDescription Update(EffectDescription effectDescription)
        {
            var inputRect = deviceContext.GetImageLocalBounds(input);
            var width = inputRect.Right - inputRect.Left;
            var height = inputRect.Bottom - inputRect.Top;

            var frame = effectDescription.ItemPosition.Frame;
            var length = effectDescription.ItemDuration.Frame;
            var fps = effectDescription.FPS;

            var left = width * (float)item.Left.GetValue(frame, length, fps) / 100f;
            var top = height * (float)item.Top.GetValue(frame, length, fps) / 100f;
            var right = width * (float)item.Right.GetValue(frame, length, fps) / 100f;
            var bottom = height * (float)item.Bottom.GetValue(frame, length, fps) / 100f;
            var centering = item.Centering;
            AffineTransform2DInterpolationMode interpolationMode = effectDescription.DrawDescription.ZoomInterpolationMode.ToTransform2D();
            
            if (left + right > width)
            {
                left = width;
                right = 0;
            }
            if (top + bottom > height)
            {
                top = height;
                bottom = 0;
            }
            var rect = new Vector4(inputRect.Left + left, inputRect.Top + top, inputRect.Right - right, inputRect.Bottom - bottom);

            if (isFirst || this.rect != rect || this.centering != centering)
            {
                cropEffect.Rectangle = rect;
                centeringEffect.TransformMatrix = centering ? Matrix3x2.CreateTranslation(-left / 2f + right / 2f, -top / 2f + bottom / 2f) : Matrix3x2.Identity;
            }
            if (isFirst || this.interpolationMode != interpolationMode)
            {
                this.interpolationMode = interpolationMode;
            }

            isFirst = false;
            this.centering = centering;
            this.rect = rect;
            this.interpolationMode = interpolationMode;

            return effectDescription.DrawDescription;
        }
        protected override void ClearEffectChain()
        {
            cropEffect.SetInput(0, null, true);
            centeringEffect.SetInput(0, null, true);
        }
    }
}