using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Effects;
using System.Windows;

namespace ShiningMeeting.Mvvm.Media.Effect
{
    /// <summary>
    /// 图片灰度效果
    /// </summary>
    public class GrayEffect : ShaderEffect
    {

        public static DependencyProperty InputProperty = ShaderEffect.RegisterPixelShaderSamplerProperty("Input", typeof(GrayEffect), 0);

        public static DependencyProperty FactorProperty = DependencyProperty.Register(
"Factor", typeof(double), typeof(GrayEffect), new PropertyMetadata(new double(), PixelShaderConstantCallback(0)));

        GrayEffect()
        {
            PixelShader = new PixelShader()
            {
                UriSource = new Uri(string.Format("/ShineMeeting;component/Mvvm/Media/Effect/Resource/gray.ps"), UriKind.RelativeOrAbsolute)
            };

            this.UpdateShaderValue(InputProperty);
            this.UpdateShaderValue(FactorProperty);
        }

        private static GrayEffect _instance;
        public static GrayEffect Instance 
        {
            get 
            {
                if (_instance == null)
                    _instance = new GrayEffect();
                return _instance;
            }
        }

        public virtual System.Windows.Media.Brush Input
        {
            get
            {
                return ((System.Windows.Media.Brush)(GetValue(InputProperty)));
            }
            set
            {
                SetValue(InputProperty, value);
            }
        }

        public virtual double Factor
        {
            get
            {
                return ((double)(GetValue(FactorProperty)));
            }
            set
            {
                SetValue(FactorProperty, value);
            }
        }

    }
}
