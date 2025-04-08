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
    public ResponseDTO Post([FromBody] ProductDTO productDTO)
    {
        try
        {
            Product product = _mapper.Map<Product>(productDTO);
            _db.Products.Add(product);
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
    public ResponseDTO Put([FromBody] ProductDTO productDTO)
    {
        try
        {
            Product product = _mapper.Map<Product>(productDTO);
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