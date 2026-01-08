using MediatR;
using TheGourmet.Application.DTOs.Cart;
using TheGourmet.Application.Exceptions;
using TheGourmet.Application.Interfaces.Repositories;

namespace TheGourmet.Application.Features.Carts.Commands.AddItemToCart;

public class AddItemToCartHandler : IRequestHandler<AddItemToCartCommand, CartDto>
{
    private readonly ICartRepository _cartRepository;
    private readonly IProductRepository _productRepository;
    public AddItemToCartHandler(ICartRepository cartRepository, IProductRepository productRepository)
    {
        _cartRepository = cartRepository;
        _productRepository = productRepository;
    }

    public async Task<CartDto> Handle(AddItemToCartCommand request, CancellationToken cancellationToken)
    {
        // ensure product exists
        var product = await _productRepository.GetProductByIdAsync(request.ProductId);
        if (product == null)
        {
            throw new NotFoundException("Product not found");
        }
        
        if (!product.IsActive) 
            throw new BadRequestException("This product is currently unavailable.");
        
        if (product.StockQuantity < request.Quantity) 
            throw new BadRequestException("Insufficient stock for the requested quantity.");
        
        // get current cart of user
        var cart = await _cartRepository.GetCartAsync(request.UserId);
        
        // check if product already in cart
        var existingItem = cart.Items.FirstOrDefault(x => x.ProductId == request.ProductId);

        if (existingItem != null)
        {
            // if exists, update quantity
            existingItem.Quantity += request.Quantity;
            existingItem.Price = product.Price;
        }
        else
        {
            // if not, add new item
            var newItem = new CartItemDto
            {
                ProductId = product.Id,
                ProductName = product.Name,
                ProductImage = product.ImageUrl ?? string.Empty,
                Price = product.Price,
                Quantity = request.Quantity,
                StockRemaining = product.StockQuantity,
            };
            cart.Items.Add(newItem);
        }
        
        // save updated cart
        await _cartRepository.UpdateCartAsync(request.UserId, cart);
        return cart;
    }
}