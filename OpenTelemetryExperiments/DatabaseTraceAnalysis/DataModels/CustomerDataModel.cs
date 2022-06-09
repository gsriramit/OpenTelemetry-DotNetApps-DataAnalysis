using System;
using System.Collections.Generic;
using System.Text;

namespace DatabaseTraceAnalysis.DataModels
{

    public class CustomerProfile
    {
        public string customerId { get; set; }
        public string name { get; set; }
        public string status { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string contactNumber { get; set; }
        public string emailAddress { get; set; }
        public int createdAt { get; set; }
        public int modifiedAt { get; set; }
        public Address[] addresses { get; set; }
        public Paymentmethod[] paymentMethods { get; set; }
        public bool isFastShippingEnabled { get; set; }
        public string id { get; set; }

    }

    public class Address
    {
        public int addr_id { get; set; }
        public bool addr_isPrimary { get; set; }
        public int addr_created_at { get; set; }
        public int addr_modified_at { get; set; }
        public string addr_line1 { get; set; }
        public string addr_line2 { get; set; }
        public string addr_city { get; set; }
        public string addr_state { get; set; }
        public string addr_country { get; set; }
        public string addr_pincode { get; set; }
    }

    public class Paymentmethod
    {
        public int pymt_id { get; set; }
        public string pymt_mode { get; set; }
        public string pymt_status { get; set; }
        public bool pymt_validated { get; set; }
    }

}
