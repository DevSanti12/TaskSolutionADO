using Microsoft.Data.SqlClient;
using ADOTaskLibrary.Interfaces;
using System.Data;
using ADOTaskLibrary;

namespace ADOTaskLibrary;

public class ProductService : IProductOperations
{
    private readonly DbHelper _dbHelper;

    public ProductService(string connectionString)
    {
        _dbHelper = new DbHelper(connectionString);
    }

    //Disconnected model
    public void CreateProduct(string name, string description, float weight, float height, float width, float length)
    {
        var dataSet = new DataSet();

        using (var connection = _dbHelper.GetConnection())
        {
            using (var adapter = new SqlDataAdapter("SELECT * FROM dbo.Product", connection))
            {
                var commandBuilder = new SqlCommandBuilder(adapter);

                // Load data into DataSet
                adapter.Fill(dataSet, "dbo.Product");

                // Create a new DataRow
                var dataTable = dataSet.Tables["dbo.Product"];
                var newRow = dataTable.NewRow();
                newRow["Name"] = name;
                newRow["Description"] = description;
                newRow["Weight"] = weight;
                newRow["Height"] = height;
                newRow["Width"] = width;
                newRow["Length"] = length;

                // Add the row to the table
                dataTable.Rows.Add(newRow);

                // Apply changes to the database
                adapter.Update(dataSet, "dbo.Product");
                Console.WriteLine("Product created successfully.");
            }
        }
    }

    //Disconnected model
    public IEnumerable<string> FetchProduct(string name)
    {
        var products = new List<string>();
        var dataSet = new DataSet();

        using (var connection = _dbHelper.GetConnection())
        {
            using (var adapter = new SqlDataAdapter("SELECT * FROM dbo.Product WHERE Name = @Name", connection))
            {
                adapter.SelectCommand.Parameters.AddWithValue("@Name", name);

                // Fill DataSet
                adapter.Fill(dataSet, "dbo.Product");

                // Read data from the DataSet
                foreach (DataRow row in dataSet.Tables["dbo.Product"].Rows)
                {
                    products.Add(row["Name"].ToString());
                }
            }
        }

        return products;
    }

    //Disconnected model
    public IEnumerable<string> GetAllProducts()
    {
        var products = new List<string>();
        var dataSet = new DataSet();

        using (var connection = _dbHelper.GetConnection())
        {
            using (var adapter = new SqlDataAdapter("SELECT Name FROM Product", connection))
            {
                // Fill DataSet
                adapter.Fill(dataSet, "Product");

                // Read data from the DataSet
                foreach (DataRow row in dataSet.Tables["Product"].Rows)
                {
                    products.Add(row["Name"].ToString());
                }
            }
        }

        return products;
    }

    //Disconnected Model
    public void UpdateProduct(int id, string name, string description, float weight, float height, float width, float length)
    {
        var dataSet = new DataSet();

        using (var connection = _dbHelper.GetConnection())
        {
            using (var adapter = new SqlDataAdapter("SELECT * FROM Product WHERE Id = @Id", connection))
            {
                adapter.SelectCommand.Parameters.AddWithValue("@Id", id);

                var commandBuilder = new SqlCommandBuilder(adapter);

                // Load data into DataSet
                adapter.Fill(dataSet, "Product");

                if (dataSet.Tables["Product"].Rows.Count == 0)
                {
                    Console.WriteLine($"No product with ID {id} was found.");
                    return;
                }

                // Update the in-memory row
                var row = dataSet.Tables["Product"].Rows[0];
                row["Name"] = name;
                row["Description"] = description;
                row["Weight"] = weight;
                row["Height"] = height;
                row["Width"] = width;
                row["Length"] = length;

                // Apply changes to the database
                adapter.Update(dataSet, "Product");
                Console.WriteLine($"Product with ID {id} was successfully updated.");
            }
        }
    }

    public void DeleteProduct(int id)
    {
        var dataSet = new DataSet();

        using (var connection = _dbHelper.GetConnection())
        {
            using (var adapter = new SqlDataAdapter("SELECT * FROM Product WHERE Id = @Id", connection))
            {
                adapter.SelectCommand.Parameters.AddWithValue("@Id", id);

                var commandBuilder = new SqlCommandBuilder(adapter);

                // Load data into DataSet
                adapter.Fill(dataSet, "Product");

                if (dataSet.Tables["Product"].Rows.Count == 0)
                {
                    Console.WriteLine($"No product with ID {id} was found.");
                    return;
                }

                // Mark the row as deleted
                var row = dataSet.Tables["Product"].Rows[0];
                row.Delete();

                // Apply changes to the database
                adapter.Update(dataSet, "Product");
                Console.WriteLine($"Product with ID {id} was successfully deleted.");
            }
        }
    }

}
