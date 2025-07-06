using Microsoft.Data.SqlClient;
using ADOTaskLibrary.Interfaces;
using ADOTaskLibrary;

namespace ADOTaskLibrary;

public class OrderService : IOrderOperations
{
    public readonly DbHelper _dbHelper;
    public OrderService( string connectionString)
    {
        _dbHelper = new DbHelper(connectionString);
    }

    public void CreateOrder(string status, DateTime createdDate, DateTime updatedDate, int productid)
    {
        using (var connection = _dbHelper.GetConnection())
        {
            connection.Open();
            using (var command = new SqlCommand("INSERT INTO \"Order\" (Status, CreatedDate, UpdatedDate, ProductId) VALUES (@Status, @CreatedDate, @UpdatedDate, @ProductId)", connection))
            {
                command.Parameters.AddWithValue("@Status", status);
                command.Parameters.AddWithValue("@CreatedDate", createdDate);
                command.Parameters.AddWithValue("@UpdatedDate", updatedDate);
                command.Parameters.AddWithValue("@ProductId", productid);
                command.ExecuteNonQuery();
            }
        }
    }

    public void DeleteOrder(int id)
    {
        using (var connection = _dbHelper.GetConnection())
        {
            connection.Open();
            using (var command = new SqlCommand("DELETE FROM \"Order\" WHERE Id = @Id", connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                int rowAffected = command.ExecuteNonQuery();
                if (rowAffected == 0)
                {
                    Console.WriteLine($"No order found with ID {id} to delete.");
                }
                else
                {
                    Console.WriteLine($"Order with ID {id} deleted successfully.");
                }
            }
        }
    }

    public IEnumerable<int> FetchOrderById(int id)
    {
        using(var connection = _dbHelper.GetConnection())
        {
            connection.Open();
            using (var command = new SqlCommand("SELECT Id FROM \"Order\" WHERE Id = @Id", connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                using (var reader = command.ExecuteReader())
                {
                    var orderIds = new List<int>();
                    while (reader.Read())
                    {
                        orderIds.Add(reader.GetInt32(0));
                    }
                    return orderIds;
                }
            }
        }
    }

    public IEnumerable<int> FetchOrdersByStatus(string status)
    {
        using (var connection = _dbHelper.GetConnection())
        {
            connection.Open();
            using (var command = new SqlCommand("SELECT Id FROM \"Order\" WHERE Status = @Status", connection))
            {
                command.Parameters.AddWithValue("@Status", status);
                using (var reader = command.ExecuteReader())
                {
                    var orderStatus = new List<int>();
                    while (reader.Read())
                    {
                        orderStatus.Add(reader.GetInt32(0));
                    }
                    return orderStatus;
                }
            }
        }
    }

    public IEnumerable<int> GetAllOrders()
    {
        using(var connection = _dbHelper.GetConnection())
        {
            connection.Open();
            using (var command = new SqlCommand("SELECT Id FROM \"Order\"", connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    var orders = new List<int>();
                    while (reader.Read())
                    {
                        orders.Add(reader.GetInt32(0));
                    }
                    return orders;
                }
            }
        }
    }

    public void UpdateOrder(int id, string status, DateTime createdDate, DateTime updatedDate, int productid)
    {
        using (var connection = _dbHelper.GetConnection())
        {
            connection.Open();
            using (var command = new SqlCommand("UPDATE \"Order\" SET Status = @Status, CreatedDate = @CreatedDate, UpdatedDate = @UpdatedDate, ProductId = @ProductId WHERE Id = @Id", connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                command.Parameters.AddWithValue("@Status", status);
                command.Parameters.AddWithValue("@CreatedDate", createdDate);
                command.Parameters.AddWithValue("@UpdatedDate", updatedDate);
                command.Parameters.AddWithValue("@ProductId", productid);
                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected == 0)
                {
                    Console.WriteLine($"No order found with ID {id} to update.");
                }
                else
                {
                    Console.WriteLine($"Order with ID {id} updated successfully.");
                }
            }
        }
    }

    public IEnumerable<dynamic> FetchFilteredOrders(int? year = null, int? month = null, string? status = null, int? productId = null)
    {
        using (var connection = _dbHelper.GetConnection())
        {
            connection.Open();
            using (var command = new SqlCommand("GetFilteredOrders", connection))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;

                // Add parameters, allowing NULL values for optional filters
                command.Parameters.AddWithValue("@Year", (object)year ?? DBNull.Value);
                command.Parameters.AddWithValue("@Month", (object)month ?? DBNull.Value);
                command.Parameters.AddWithValue("@Status", (object)status ?? DBNull.Value);
                command.Parameters.AddWithValue("@ProductId", (object)productId ?? DBNull.Value);

                using (var reader = command.ExecuteReader())
                {
                    var orders = new List<dynamic>();
                    while (reader.Read())
                    {
                        orders.Add(new
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Status = reader.GetString(reader.GetOrdinal("Status")),
                            CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                            UpdatedDate = reader.GetDateTime(reader.GetOrdinal("UpdatedDate")),
                            ProductId = reader.GetInt32(reader.GetOrdinal("ProductId"))
                        });
                    }
                    return orders;
                }
            }
        }
    }

    public void DeleteOrdersInBulk(int? year = null, int? month = null, string? status = null, int? productId = null)
    {
        using (var connection = _dbHelper.GetConnection())
        {
            connection.Open();

            // Start a transaction to ensure atomicity and avoid partial deletions
            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    using (var command = new SqlCommand("BulkDeleteOrders", connection, transaction))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        // Add parameters for filters
                        command.Parameters.AddWithValue("@Year", (object)year ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Month", (object)month ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Status", (object)status ?? DBNull.Value);
                        command.Parameters.AddWithValue("@ProductId", (object)productId ?? DBNull.Value);

                        int rowsAffected = command.ExecuteNonQuery();

                        // Commit transaction if deletion is successful
                        transaction.Commit();
                        Console.WriteLine($"{rowsAffected} orders deleted successfully.");
                    }
                }
                catch (Exception ex)
                {
                    // Rollback transaction on error
                    transaction.Rollback();
                    Console.WriteLine($"Error occurred: {ex.Message}");
                    throw;
                }
            }
        }
    }
}
