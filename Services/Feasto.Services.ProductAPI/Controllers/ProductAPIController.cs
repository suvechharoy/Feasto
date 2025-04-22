using AutoMapper;
using Feasto.Services.ProductAPI.Data;
using Feasto.Services.ProductAPI.Models;
using Feasto.Services.ProductAPI.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Feasto.Services.ProductAPI.Controllers;

[Route("api/product")]
[ApiController]
public class ProductAPIController : ControllerBase
{
    private readonly AppDbContext _db;
    private ResponseDTO _response;
    private IMapper _mapper;
    public ProductAPIController(AppDbContext db, IMapper mapper)
    {
        _db = db;
        _response = new ResponseDTO();
        _mapper = mapper;
    }

    [HttpGet]
    public ResponseDTO Get()
    {
        try
        {
            IEnumerable<Product> products = _db.Products.ToList();
            _response.Result = _mapper.Map<IEnumerable<ProductDTO>>(products);
        }
        catch (Exception e)
        {
            _response.IsSuccess = false;
            _response.Message = e.Message;
        }
        return _response;
    }
    [HttpGet]
    [Route("{id:int}")]
    public ResponseDTO Get(int id)
    {
        try
        {
            Product product = _db.Products.First(u => u.ProductId == id);
            _response.Result = _mapper.Map<ProductDTO>(product);
        }
        catch (Exception e)
        {
            _response.IsSuccess = false;
            _response.Message = e.Message;
        }
        return _response;
    }
    [HttpPost]
    [Authorize(Roles = "ADMIN")]
    public ResponseDTO Post(ProductDTO productDTO)
    {
        try
        {
            Product product = _mapper.Map<Product>(productDTO);
            _db.Products.Add(product);
            _db.SaveChanges();

            if (productDTO.Image != null)
            {
                string fileName = product.ProductId + Path.GetExtension(productDTO.Image.FileName);
                string filePath = @"ProductImages\" + fileName;

                // Construct the absolute path within the Azure App Service wwwroot for saving
                string uploadPath = Path.Combine(@"C:\home\site\wwwroot\", "ProductImages");
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }
                string filePathDirectory = Path.Combine(uploadPath, fileName);
                
                // Delete any existing image with the same name (using the Azure path)
                FileInfo file = new FileInfo(filePathDirectory);
                if (file.Exists)
                {
                    file.Delete();
                }
                
                using (var fileStream = new FileStream(filePathDirectory, FileMode.Create))
                {
                    productDTO.Image.CopyTo(fileStream);
                }
                var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";
                product.ImageUrl = baseUrl+ "/ProductImages/"+ fileName;
                product.ImageLocalPath = filePath;
            }
            else
            {
                product.ImageUrl = "https://placehold.co/600x400"; //default placeholder
            }
            _db.Products.Update(product);
            _db.SaveChanges();
            _response.Result = _mapper.Map<ProductDTO>(product);
        }
        catch (Exception e)
        {
            _response.IsSuccess = false;
            _response.Message = e.Message;
        }
        return _response;
    }
    [HttpPut]
    [Authorize(Roles = "ADMIN")]
    public ResponseDTO Put(ProductDTO productDTO)
    {
        try
        {
            Product product = _mapper.Map<Product>(productDTO);
            
            if (productDTO.Image != null) //which means a new image has been uploaded for the product
            {
                //delete the existing image
                if (!string.IsNullOrEmpty(product.ImageLocalPath))
                {
                    var oldFilePathDirectory = Path.Combine(Directory.GetCurrentDirectory(), product.ImageLocalPath);
                    FileInfo file = new FileInfo(oldFilePathDirectory);
                    if (file.Exists)
                    {
                        file.Delete();
                    }
                }
                
                //add new image
                string fileName = product.ProductId + Path.GetExtension(productDTO.Image.FileName); //generate a new filename based on productID and preserve the extension. So its extracting the extension and appending it to productID
                //string filePath = @"wwwroot\ProductImages\" + fileName; 
                string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ProductImages"); //complete file path 
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }
                string filePathDirectory = Path.Combine(uploadPath, fileName); //get the file path directory by combining the file path with current directory. This will give complete location to wwwroot folder
                using (var fileStream = new FileStream(filePathDirectory, FileMode.Create)) //copy the image to the folder fetched above i.e., filePathDirectory
                {
                    productDTO.Image.CopyTo(fileStream);
                }

                var baseUrl =
                    $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}"; 
                product.ImageUrl = baseUrl+ "/ProductImages/"+ fileName;
                product.ImageLocalPath = filePathDirectory;
            }
            _db.Products.Update(product);
            _db.SaveChanges();
            _response.Result = _mapper.Map<ProductDTO>(product);
        }
        catch (Exception e)
        {
            _response.IsSuccess = false;
            _response.Message = e.Message;
        }
        return _response;
    }
    [HttpDelete]
    [Route("{id:int}")]
    [Authorize(Roles = "ADMIN")]
    public ResponseDTO Delete(int id)
    {
        try
        {
            Product product = _db.Products.First(u => u.ProductId == id);
            if (!string.IsNullOrEmpty(product.ImageLocalPath))
            {
                var oldFilePathDirectory = Path.Combine(Directory.GetCurrentDirectory(), product.ImageLocalPath);
                FileInfo file = new FileInfo(oldFilePathDirectory);
                if (file.Exists)
                {
                    file.Delete();
                }
            }
            _db.Products.Remove(product);
            _db.SaveChanges();
        }
        catch (Exception e)
        {
            _response.IsSuccess = false;
            _response.Message = e.Message;
        }
        return _response;
    }
}