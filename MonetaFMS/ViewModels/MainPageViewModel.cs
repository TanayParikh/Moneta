using MonetaFMS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace MonetaFMS.ViewModels
{
    public class MainPageViewModel : BindableBase
    {
        private string pageTitle = "Moneta FMS";
        private bool isPaneOpen = true;
        private Visibility backButtonVisibility = Visibility.Collapsed;

        public MainPageViewModel()
        {
        }

        public string PageTitle
        {
            get { return pageTitle; }
            set { SetProperty(ref pageTitle, value); }
        }

        public bool IsPaneOpen
        {
            get { return isPaneOpen; }
            set { SetProperty(ref isPaneOpen, value); }
        }

        public Visibility BackButtonVisibility
        {
            get { return backButtonVisibility; }
            set { SetProperty(ref backButtonVisibility, value); }
        }
    }
}
