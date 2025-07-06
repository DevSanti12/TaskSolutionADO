using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADOTaskLibrary.Interfaces
{
    public interface IOrderOperations
    {
        public void CreateOrder(string status, DateTime createdDate, DateTime updatedDate, int productid);

        public IEnumerable<int> FetchOrdersByStatus(string status);

        public IEnumerable<int> FetchOrderById(int id);

        public IEnumerable<int> GetAllOrders();

        public void UpdateOrder(int id, string status, DateTime createdDate, DateTime updatedDate, int productid);

        public void DeleteOrder(int id);
    }
}
