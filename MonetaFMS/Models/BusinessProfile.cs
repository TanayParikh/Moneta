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
        string _company = "Company Name";

        [JsonProperty]
        public string Company
        {
            get { return _company; }
            set { SetProperty(ref _company, value); }
        }

        string _address = "123 Address St.";

        [JsonProperty]
        public string Address
        {
            get { return _address; }
            set { SetProperty(ref _address, value); }
        }

        string _phoneNumber = "(555) 555-5555";

        [JsonProperty]
        public string PhoneNumber
        {
            get { return _phoneNumber; }
            set { SetProperty(ref _phoneNumber, value); }
        }

        string _email = "email@example.com";

        [JsonProperty]
        public string Email
        {
            get { return _email; }
            set { SetProperty(ref _email, value); }
        }

        public BusinessProfile(string company = "123 Address St.", string address = "123 Address St.", string phoneNumber = "(555) 555-5555", string email = "email@example.com")
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
