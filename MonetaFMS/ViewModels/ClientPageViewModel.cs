using Microsoft.Toolkit.Uwp.UI;
using MonetaFMS.Common;
using MonetaFMS.Interfaces;
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
        private IClientService ClientService { get; set; }
        private ObservableCollection<Client> _allClients { get; set; }
        public AdvancedCollectionView AllClients { get; set; }
        
        private Client ClientBackup { get; set; }

        Client _selectedClient;
        public Client SelectedClient
        {
            get { return _selectedClient; }
            set
            {
                SetProperty(ref _selectedClient, value);
                ClientBackup = Extensions.Clone(SelectedClient);
            }
        }


        public ClientPageViewModel()
        {
            _allClients = new ObservableCollection<Client>(Services.Services.ClientService.AllItems);
            AllClients = new AdvancedCollectionView(_allClients);

            ClientService = Services.Services.ClientService;
        }

        internal void CancelClientEdit()
        {
            SelectedClient = SelectedClient.Id == -1 ? null : ClientBackup;
        }

        internal bool SaveClient()
        {
            return SelectedClient.Id == -1 ?
                ClientService.CreateEntry(SelectedClient).Id > 0 :
                ClientService.UpdateEntry(SelectedClient);

        }

        internal void CreateClient()
        {
            Client newClient = ClientService.NewClient();
            AllClients.Insert(0, newClient);
            SelectedClient = newClient;
        }
    }
}
