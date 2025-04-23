using Discount.Grpc.Data;
using Discount.Grpc.Models;
using Grpc.Core;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Discount.Grpc.Services;

public class DiscountService
    (DiscountContext dbcontext, ILogger<DiscountService> logger)
    : DiscountProtoService.DiscountProtoServiceBase
{
    public override async Task<CouponModel> GetDiscount(GetDiscountRequest request, ServerCallContext context)
    {
        var coupon = await dbcontext
            .Coupons
            .FirstOrDefaultAsync(x => x.ProductName == request.ProductName);
        if (coupon is null)
            coupon = new Coupon { ProductName = "No Discount", Amount = 0, Description = "No Discount Desc" };
        logger.LogInformation("Discount is retrieved for ProductName : {productName}, Amount : {amount}", coupon.ProductName, coupon.Amount);

        var couponModel = coupon.Adapt<CouponModel>();
        return couponModel;
    }
    public override async Task<CouponModel> CreateDiscount(CreateDiscountRequest request, ServerCallContext context)
    {
        var coupon = request.Coupon.Adapt<Coupon>();
        if (coupon is null)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid request object."));

        dbcontext.Coupons.Add(coupon);
        await dbcontext.SaveChangesAsync();

        logger.LogInformation("Discount is successfully created. ProductName : {ProductName}", coupon.ProductName);

        var couponModel = coupon.Adapt<CouponModel>();
        return couponModel;
    }
    public override async Task<CouponModel> UpdateDiscount(UpdateDiscountRequest request, ServerCallContext context)
    {
        var coupon = request.Coupon.Adapt<Coupon>();
        if (coupon is null)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid request object."));

        dbcontext.Coupons.Update(coupon);
        await dbcontext.SaveChangesAsync();

        logger.LogInformation("Discount is successfully updated. ProductName : {ProductName}", coupon.ProductName);

        var couponModel = coupon.Adapt<CouponModel>();
        return couponModel;
    }
    public override async Task<DeleteDiscountResponse> DeleteDiscount(DeleteDiscountRequest request, ServerCallContext context)
    {
        var coupon = await dbcontext
            .Coupons
            .FirstOrDefaultAsync(x => x.ProductName == request.ProductName);
        if (coupon is null)
            throw new RpcException(new Status(StatusCode.NotFound, $"Distcount with ProductName = {request.ProductName} is not found."));
        dbcontext.Coupons .Remove(coupon);
        await dbcontext.SaveChangesAsync();
        logger.LogInformation("Discount is successfully deleted. ProductName : {ProductName}", coupon.ProductName);
        return new DeleteDiscountResponse { Success = true };

    }
}
