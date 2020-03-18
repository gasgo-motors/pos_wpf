using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer;

namespace BusinessLayer
{
    public class CustomersManager : PosManager<CustomersManager>
    {
        public void SaveCustomer(Customer customer)
        {
            DaoController.Current.SaveCustomer(customer);
        }

        public void UpdateCustomer(Customer customer)
        {
            DaoController.Current.UpdateCustomer(customer);
        }
    }
}
