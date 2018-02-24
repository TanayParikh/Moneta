using Microsoft.Toolkit.Uwp.UI;
using MonetaFMS.Common;
using MonetaFMS.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonetaFMS.ViewModels
{
    public class ClientPageViewModel : BindableBase
    {
        private ObservableCollection<Client> _allClients { get; set; }
        public AdvancedCollectionView AllClients { get; set; }

        public ClientPageViewModel()
        {
            _allClients = new ObservableCollection<Client>(Services.Services.ClientService.AllItems);
            AllClients = new AdvancedCollectionView(_allClients);
        }
        

    }
}
