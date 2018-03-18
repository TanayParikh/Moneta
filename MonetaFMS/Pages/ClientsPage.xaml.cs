using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.Toolkit.Uwp;
using MonetaFMS.ViewModels;
using System.Threading.Tasks;
using LottieUWP;
using MonetaFMS.Models;

namespace MonetaFMS.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public partial class ClientsPage : PageBase
    {
        public ClientPageViewModel ViewModel { get; set; } = new ClientPageViewModel();
        
        private double _previousWidth = Window.Current.Bounds.Width;

        public ClientsPage()
        {
            InitializeComponent();
            DataContext = ViewModel;

            AnimationView = LottieAnimationView;
            FadesEnabled = true;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Window.Current.SizeChanged += Current_SizeChanged;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Window.Current.SizeChanged -= Current_SizeChanged;
        }

        // workaround for loaded unloaded getting called in wrong order when shell template gets swapped
        private void Current_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            if ((e.Size.Width < 700 && _previousWidth >= 700) ||
                   (e.Size.Width >= 700 && _previousWidth < 700))
            {
                _previousWidth = e.Size.Width;
                var t = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, async () =>
                {
                    await Task.Delay(500);
                    //await Windows.UI.Shell.Current.RefreshXamlRenderAsync();
                });
            }
            else
            {
                _previousWidth = e.Size.Width;
            }
        }

        private void LottieAnimationWrapper_GettingFocus(UIElement sender, GettingFocusEventArgs args)
        {

        }
    }
}
