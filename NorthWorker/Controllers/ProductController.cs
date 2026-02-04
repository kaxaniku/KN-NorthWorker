using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        }
        return View(products);
    }
    [Route("Product/Details/{id:int}")]
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
        }
        return View(product);
    }
    public IActionResult Create()
    {
        PopulateDropdowns();
        return View();
    }

    [HttpPost]
    public IActionResult Create(ProductCreationModel product)
    {
        if (ModelState.IsValid)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string sql = @"INSERT INTO Products 
                         (ProductName, QuantityPerUnit, CategoryID, SupplierID, UnitPrice, UnitsInStock, UnitsOnOrder, ReorderLevel) 
                         VALUES 
                         (@Name, @Qty, @Cat, @Sup, @Price, @Stock, @Order, @Level)";

                using (SqlCommand cmd = new SqlCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@Name", product.ProductName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Qty", product.QuantityPerUnit ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Cat", product.CategoryName);
                    cmd.Parameters.AddWithValue("@Sup", product.SupplierName);
                    cmd.Parameters.AddWithValue("@Price", product.UnitPrice);
                    cmd.Parameters.AddWithValue("@Stock", product.UnitsInStock);
                    cmd.Parameters.AddWithValue("@Order", product.UnitsOnOrder);
                    cmd.Parameters.AddWithValue("@Level", product.ReorderLevel);

                    connection.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            return RedirectToAction(nameof(Index));
        }

        PopulateDropdowns();
        return View(product);
    }

    private void PopulateDropdowns()
    {
        List<SelectListItem> categories = new List<SelectListItem>();
        List<SelectListItem> suppliers = new List<SelectListItem>();

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();

            string catSql = "SELECT CategoryID, CategoryName FROM Categories";
            using (SqlCommand cmd = new SqlCommand(catSql, connection))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        categories.Add(new SelectListItem
                        {
                            Text = reader["CategoryName"].ToString(),
                            Value = reader["CategoryID"].ToString()
                        });
                    }
                }
            }

            string supSql = "SELECT SupplierID, CompanyName FROM Suppliers";
            using (SqlCommand cmd = new SqlCommand(supSql, connection))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        suppliers.Add(new SelectListItem
                        {
                            Text = reader["CompanyName"].ToString(),
                            Value = reader["SupplierID"].ToString()
                        });
                    }
                }
            }
        }

        ViewBag.CategoryList = categories;
        ViewBag.SupplierList = suppliers;
    }
}
