using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonetaFMS.Models
{
    public class Expense : Record
    {
        public string Description { get; set; }
        public ExpenseCategory Category { get; set; }
        public DateTime Date { get; set; }
        public decimal TaxComponent { get; set; }
        public decimal TotalCost { get; set; }
        public string ImageReference { get; set; }
        public Invoice Invoice { get; set; }

        [JsonConstructor]
        public Expense(int id, DateTime creation, string note, string description, ExpenseCategory category,
            DateTime date, decimal taxComponent, decimal totalCost, string imageReference, Invoice invoice)
            : this(note, description, category, date, taxComponent, totalCost, imageReference, invoice)
        {
            Id = id;
            CreationDate = creation;
        }

        public Expense(string note, string description, ExpenseCategory category, DateTime date,
            decimal taxComponent, decimal totalCost, string imageReference, Invoice invoice) : base()
        {
            Note = note;
            Description = description;
            Category = category;
            Date = date;
            TaxComponent = taxComponent;
            TotalCost = totalCost;
            ImageReference = imageReference;
            Invoice = invoice;

            if (Date == null || Date.Year == 1)
                Date = DateTime.Now;
        }
    }
}
