using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Effects;
using System.Windows.Media;
using System.Windows;
using System.Windows.Media.Animation;

namespace ShiningMeeting.Mvvm.Media.Effect
{
    /// <summary>
    /// 图片变暗效果
    /// </summary>
    public class DarkEffect : ShaderEffect
    {
        public static readonly DependencyProperty InputProperty = ShaderEffect.RegisterPixelShaderSamplerProperty("Input", typeof(DarkEffect), 0);
        public static readonly DependencyProperty BloomIntensityProperty = DependencyProperty.Register("BloomIntensity", typeof(double), typeof(DarkEffect), new UIPropertyMetadata(((double)(0D)), PixelShaderConstantCallback(0)));
        public static readonly DependencyProperty BaseIntensityProperty = DependencyProperty.Register("BaseIntensity", typeof(double), typeof(DarkEffect), new UIPropertyMetadata(((double)(0.5D)), PixelShaderConstantCallback(1)));
        public static readonly DependencyProperty BloomSaturationProperty = DependencyProperty.Register("BloomSaturation", typeof(double), typeof(DarkEffect), new UIPropertyMetadata(((double)(1D)), PixelShaderConstantCallback(2)));
        public static readonly DependencyProperty BaseSaturationProperty = DependencyProperty.Register("BaseSaturation", typeof(double), typeof(DarkEffect), new UIPropertyMetadata(((double)(0.5D)), PixelShaderConstantCallback(3)));
        public DarkEffect()
        {
            PixelShader pixelShader = new PixelShader();
            pixelShader.UriSource = new Uri(string.Format("/ShineMeeting;component/Mvvm/Media/Effect/Resource/Bloom.ps"), UriKind.RelativeOrAbsolute);
            this.PixelShader = pixelShader;

            this.UpdateShaderValue(InputProperty);
            this.UpdateShaderValue(BloomIntensityProperty);
            this.UpdateShaderValue(BaseIntensityProperty);
            this.UpdateShaderValue(BloomSaturationProperty);
            this.UpdateShaderValue(BaseSaturationProperty);
        }

        public void BeginAnimation(bool toLight)
        {
            DoubleAnimation animation = new DoubleAnimation();
            animation.Duration = new System.Windows.Duration(new TimeSpan(0, 0, 0, 0, Duration));

            if (toLight)
            {
                animation.From = 0.1D;
                animation.To = 1D;
            }
            else 
            {
                animation.From = 1D;
                animation.To = 0.1D;
            }
            this.BeginAnimation(DarkEffect.BloomIntensityProperty, animation);
        }

        /// <summary>
        /// 是否可用状态
        /// </summary>
        public bool IsLight { get { return BloomIntensity == 1D; } set { if (value) BloomIntensity = 1.0D; else BloomIntensity = 0.2D; } }

        private int m_duration = 500;
        /// <summary>
        /// 动画间隔(毫秒)
        /// </summary>
        public int Duration { get { return m_duration; } set { m_duration = value; } }

        public Brush Input
        {
            get
            {
                return ((Brush)(this.GetValue(InputProperty)));
            }
            set
            {
                this.SetValue(InputProperty, value);
            }
        }
        /// <summary>Intensity of the bloom image.</summary>
        public double BloomIntensity
        {
            get
            {
                return ((double)(this.GetValue(BloomIntensityProperty)));
            }
            set
            {
                this.SetValue(BloomIntensityProperty, value);
            }
        }
        /// <summary>Intensity of the base image.</summary>
        public double BaseIntensity
        {
            get
            {
                return ((double)(this.GetValue(BaseIntensityProperty)));
            }
            set
            {
                this.SetValue(BaseIntensityProperty, value);
            }
        }
        /// <summary>Saturation of the bloom image.</summary>
        public double BloomSaturation
        {
            get
            {
                return ((double)(this.GetValue(BloomSaturationProperty)));
            }
            set
            {
                this.SetValue(BloomSaturationProperty, value);
            }
        }
        /// <summary>Saturation of the base image.</summary>
        public double BaseSaturation
        {
            get
            {
                return ((double)(this.GetValue(BaseSaturationProperty)));
            }
            set
            {
                this.SetValue(BaseSaturationProperty, value);
            }
        }
    }
}
