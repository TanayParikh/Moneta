using LottieUWP;
using MonetaFMS.Models;
using MonetaFMS.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace MonetaFMS.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class InvoicesPage : Page
    {
        public InvoicePageViewModel ViewModel { get; set; } = new InvoicePageViewModel();


        public InvoicesPage()
        {
            this.InitializeComponent();
            DataContext = ViewModel;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.OnPageLoaded();
        }

        // https://canbilgin.wordpress.com/2017/05/09/add-list-view-drop-shadow-on-uwp/
        private void ListView_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            var presenter = VisualTreeHelper.GetChild(args.ItemContainer, 0) as ListViewItemPresenter;
            var contentHost = VisualTreeHelper.GetChild(presenter, 0) as Grid;
            var shadowHost = VisualTreeHelper.GetChild(contentHost, 0) as Canvas;
            var content = VisualTreeHelper.GetChild(contentHost, 1) as Grid;

            var contentVisual = Windows.UI.Xaml.Hosting.ElementCompositionPreview.GetElementVisual(content);

            var _compositor = contentVisual.Compositor;

            var sprite = _compositor.CreateSpriteVisual();
            sprite.Size = contentVisual.Size;
            sprite.CenterPoint = contentVisual.CenterPoint;

            var shadow = _compositor.CreateDropShadow();
            sprite.Shadow = shadow;

            shadow.BlurRadius = 2.5f;
            shadow.Offset = new Vector3(-6, -6, 0);
            shadow.Color = Colors.DarkGray;
            shadow.Opacity = 0.10f;

            Windows.UI.Xaml.Hosting.ElementCompositionPreview.SetElementChildVisual(shadowHost, sprite);

            var bindSizeAnimation = _compositor.CreateExpressionAnimation("hostVisual.Size");
            bindSizeAnimation.SetReferenceParameter("hostVisual", contentVisual);

            sprite.StartAnimation("Size", bindSizeAnimation);
        }

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            ViewModel.InvoiceSelected((Invoice)e.ClickedItem);
        }
    }
}
