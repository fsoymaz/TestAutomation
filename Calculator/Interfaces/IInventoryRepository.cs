using Backend.Models;

namespace Backend.Interfaces;

public interface IInventoryRepository
{
    Product? GetProductById(int id);
    bool UpdateStock(int productId, int newQuantity);
}