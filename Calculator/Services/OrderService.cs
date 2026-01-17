using Backend.Interfaces;
using Backend.Models;

namespace Backend.Services;

public class OrderService
{
    private readonly IInventoryRepository _inventoryRepository;

    // Dependency Injection (Constructor Injection)
    public OrderService(IInventoryRepository inventoryRepository)
    {
        _inventoryRepository = inventoryRepository;
    }

    public decimal PlaceOrder(int productId, int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Order quantity must be greater than zero.");

        if (quantity > 50)
            throw new ArgumentException("Cannot order more than 50 items at once.");

        // Bagimlilik uzerinden urunu getir
        var product = _inventoryRepository.GetProductById(productId);

        if (product == null)
            throw new KeyNotFoundException($"Product with ID {productId} not found.");

        if (product.StockQuantity < quantity)
            throw new InvalidOperationException($"Insufficient stock for product '{product.Name}'. Available: {product.StockQuantity}, Requested: {quantity}");

        // Stogu guncelle
        product.StockQuantity -= quantity;
        bool updateResult = _inventoryRepository.UpdateStock(productId, product.StockQuantity);

        if (!updateResult)
            throw new Exception("Failed to update stock in database.");

        // Calculate and return total price
        return product.Price * quantity;
        
    }
}
