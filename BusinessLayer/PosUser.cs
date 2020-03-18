using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class PosUser
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsGood { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public PosUserTypes UserType { get; set; }

        public int UserTypeId
        {
            get { return (int) UserType; }
            set { UserType = (PosUserTypes) value; }
        }

        //public bool IsBlocked { get; set; }

        public string SalesPersonCode { get; set; }

        public string FullName
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }

        public string UserTypeName
        {
            get { return UserType.ToString(); }
        }
    }

    public enum PosUserTypes
    {
        PreSaler = 1,
        Manager = 2,
        Distributor = 3,
        Shop = 4
    }
}
