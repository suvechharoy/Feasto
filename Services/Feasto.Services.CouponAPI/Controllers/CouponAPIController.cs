using AutoMapper;
using Feasto.Services.CouponAPI.Data;
using Feasto.Services.CouponAPI.Models;
using Feasto.Services.CouponAPI.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace Feasto.Services.CouponAPI.Controllers;

[Route("api/coupon")]
[ApiController]
public class CouponAPIController : ControllerBase
{
    private readonly AppDbContext _db;
    private ResponseDTO _response;
    private IMapper _mapper;
    public CouponAPIController(AppDbContext db, IMapper mapper)
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
            IEnumerable<Coupon> coupons = _db.Coupons.ToList();
            _response.Result = _mapper.Map<IEnumerable<CouponDTO>>(coupons);
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
            Coupon coupon = _db.Coupons.First(u => u.CouponId == id);
            _response.Result = _mapper.Map<CouponDTO>(coupon);
        }
        catch (Exception e)
        {
            _response.IsSuccess = false;
            _response.Message = e.Message;
        }
        return _response;
    }
    [HttpGet]
    [Route("GetByCode/{code}")]
    public ResponseDTO Get(string code)
    {
        try
        {
            Coupon coupon = _db.Coupons.FirstOrDefault(u => u.CouponCode.ToLower() == code.ToLower());
            if (coupon == null)
            {
                _response.IsSuccess = false;
            }
            _response.Result = _mapper.Map<CouponDTO>(coupon);
        }
        catch (Exception e)
        {
            _response.IsSuccess = false;
            _response.Message = e.Message;
        }
        return _response;
    }
    [HttpPost]
    public ResponseDTO Post([FromBody] CouponDTO couponDTO)
    {
        try
        {
            Coupon coupon = _mapper.Map<Coupon>(couponDTO);
            _db.Coupons.Add(coupon);
            _db.SaveChanges();
            _response.Result = _mapper.Map<CouponDTO>(coupon);
        }
        catch (Exception e)
        {
            _response.IsSuccess = false;
            _response.Message = e.Message;
        }
        return _response;
    }
    [HttpPut]
    public ResponseDTO Put([FromBody] CouponDTO couponDTO)
    {
        try
        {
            Coupon coupon = _mapper.Map<Coupon>(couponDTO);
            _db.Coupons.Update(coupon);
            _db.SaveChanges();
            _response.Result = _mapper.Map<CouponDTO>(coupon);
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
    public ResponseDTO Delete(int id)
    {
        try
        {
            Coupon coupon = _db.Coupons.First(u => u.CouponId == id);
            _db.Coupons.Remove(coupon);
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