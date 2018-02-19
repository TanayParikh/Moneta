using MonetaFMS.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonetaFMS.Models
{
    public class BusinessProfile : BindableBase
    {
        string _company;

        [JsonProperty]
        public string Company
        {
            get { return _company; }
            set { SetProperty(ref _company, value); }
        }

        string _address;

        [JsonProperty]
        public string Address
        {
            get { return _address; }
            set { SetProperty(ref _address, value); }
        }

        string _phoneNumber;

        [JsonProperty]
        public string PhoneNumber
        {
            get { return _phoneNumber; }
            set { SetProperty(ref _phoneNumber, value); }
        }

        string _email;

        [JsonProperty]
        public string Email
        {
            get { return _email; }
            set { SetProperty(ref _email, value); }
        }

        public BusinessProfile(string company = "", string address = "", string phoneNumber = "", string email = "")
        {
            Company = company;
            Address = address;
            PhoneNumber = phoneNumber;
            Email = email;
        }

        public BusinessProfile(BusinessProfile businessProfile) : this(businessProfile.Company, businessProfile.Address, businessProfile.PhoneNumber, businessProfile.Email) { }

        // JsonConvert Default Constructor
        public BusinessProfile() { }
    }
}
