using MonetaFMS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonetaFMS.ViewModels
{
    public class MainPageViewModel : BindableBase
    {
        private string pageTitle = "Moneta FMS";
        private bool isPaneOpen = true;

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
    }
}
