using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using NorthWorker.Models;

namespace NorthWorker.Controllers;
public class ProductController : Controller
{
    private readonly ILogger<ProductController> _logger;
    private readonly string _connectionString;
    public ProductController(ILogger<ProductController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _connectionString = configuration.GetConnectionString("DefaultConnection")!;
    }
    public IActionResult Index()
    {
        List<Product> products = new List<Product>();

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string sql = "SELECT * FROM Products";
            SqlCommand cmd = new SqlCommand(sql, connection);

            connection.Open();
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    products.Add(new Product
                    {
                        ProductId = (int)reader["ProductID"],
                        ProductName = reader["ProductName"].ToString(),
                    });
                }
            }
            connection.Close();
        }
        return View(products);
    }
    public IActionResult Details(int id)
    {
        Product? product = null;

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string sql = "SELECT * FROM Products WHERE ProductID = @id";
            SqlCommand cmd = new SqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@id", id);

            connection.Open();
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    product = new Product
                    {
                        ProductId = (int)reader["ProductID"],
                        ProductName = reader["ProductName"].ToString(),
                        UnitPrice = (decimal)reader["UnitPrice"],
                        UnitsInStock = Convert.ToInt32(reader["UnitsInStock"])
                    };
                }
            }
            connection.Close();
        }

        return View(product);
    }
}
