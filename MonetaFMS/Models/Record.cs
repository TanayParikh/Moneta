using MonetaFMS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonetaFMS.Models
{
    public abstract class Record : BindableBase
    {
        public int Id { get; set; }
        public DateTime CreationDate { get; set; }
        public string Note { get; set; }

        protected Record()
        {
            if (CreationDate.Year == 1)
                CreationDate = DateTime.Now;
        }
    }
}
