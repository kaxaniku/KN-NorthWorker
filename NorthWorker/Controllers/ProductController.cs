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
        return View(PopulateDropdowns(new ProductCreationModel()));
    }

    [HttpPost]
    public IActionResult Create(ProductCreationModel product)
    {
        if (!ModelState.IsValid)
        {
            PopulateDropdowns(product);
            return View(product);
        }

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
                cmd.Parameters.AddWithValue("@Cat", product.CategoryId);
                cmd.Parameters.AddWithValue("@Sup", product.SupplierId);
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

    [HttpPost]
    public IActionResult Delete(int id)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string sql = "DELETE FROM Products WHERE ProductID = @id";
            SqlCommand cmd = new SqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@id", id);
            connection.Open();
            cmd.ExecuteNonQuery();
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult Update(Product product)
    {
        if (!ModelState.IsValid)
        {
            return View("Details", product);
        }

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string sql = @"UPDATE Products SET 
                         ProductName = @Name, 
                         UnitPrice = @Price, 
                         UnitsInStock = @Stock 
                         WHERE ProductID = @id";
            using (SqlCommand cmd = new SqlCommand(sql, connection))
            {
                cmd.Parameters.AddWithValue("@Name", product.ProductName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Price", product.UnitPrice);
                cmd.Parameters.AddWithValue("@Stock", product.UnitsInStock);
                cmd.Parameters.AddWithValue("@id", product.ProductId);
                connection.Open();
                cmd.ExecuteNonQuery();
            }
        }
        return RedirectToAction(nameof(Index));
    }

    private ProductCreationModel PopulateDropdowns(ProductCreationModel model)
    {
        List<SelectListItem> categories = new List<SelectListItem>();
        List<SelectListItem> suppliers = new List<SelectListItem>();

        DataReader(categories, "SELECT CategoryID, CategoryName FROM Categories", "CategoryName", "CategoryID");
        DataReader(suppliers, "SELECT SupplierID, CompanyName FROM Suppliers", "CompanyName", "SupplierID");

        model.Categories = categories;
        model.Suppliers = suppliers;

        return model;
    }

    private List<SelectListItem> DataReader(
        List<SelectListItem> selectedList,
        string sql,
        string TextString,
        string ValueString)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            using (SqlCommand cmd = new SqlCommand(sql, connection))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        selectedList.Add(new SelectListItem
                        {
                            Text = reader[TextString].ToString(),
                            Value = reader[ValueString].ToString()
                        });
                    }
                }
            }
        }
        return selectedList;
    }
}
