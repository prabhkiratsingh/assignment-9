using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApi.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        
        [Required]
        public decimal Amount { get; set; }

        [Required]
        public string From { get; set; }

        [Required]
        public string Description { get; set; }

        // Date & time is that of the transaction.
        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public DateTime Time { get; set; }
        
        // Creating a foreign key linking UserAccount.
        public virtual UserAccount UserAccount { get; set; }

        [Required]
        public int UserAccountId { get; set; }
    }
}