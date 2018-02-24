using LottieUWP;
using MonetaFMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace MonetaFMS.Pages
{
    public class PageBase : Page
    {
        private const string LOTTIE_ANIMATIONS_DIRECTORY = "Assets/LottieAnimations/";

        protected LottieAnimationView AnimationView { get; set; }
        protected bool FadesEnabled { get; set; }
        
        protected async Task PlayAnimation(bool success)
        {
            await PlayAnimation(success ? LottieAnimation.Success : LottieAnimation.Error);
        }

        protected async Task PlayAnimation(LottieAnimation animationName)
        {
            if (AnimationView.IsAnimating)
                return;

            await AnimationView.SetAnimationAsync(System.IO.Path.Combine(LOTTIE_ANIMATIONS_DIRECTORY, animationName.ToString() + ".json"), LottieAnimationView.CacheStrategy.Strong);
            
            if (FadesEnabled)
                ((Storyboard) Resources["AnimationFadeIn"])?.Begin();

            AnimationView.Speed = 2f;
            AnimationView.RepeatCount = 0;
            AnimationView.PlayAnimation();

            await Task.Delay(2700);

            if (FadesEnabled)
                ((Storyboard) Resources["AnimationFadeOut"])?.Begin();
        }
    }
}
