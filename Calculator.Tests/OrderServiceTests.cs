using Xunit;
using Moq;
using Backend.Services;
using Backend.Interfaces;
using Backend.Models;
using System;

namespace Backend.Tests;

public class OrderServiceTests
{
    private readonly Mock<IInventoryRepository> _mockRepo;
    private readonly OrderService _orderService;

    public OrderServiceTests()
    {
        // Her testten once calisir
        _mockRepo = new Mock<IInventoryRepository>();
        _orderService = new OrderService(_mockRepo.Object);
    }

    [Fact]
    public void PlaceOrder_ShouldReturnTotalPrice_WhenStockIsSufficient()
    {
        // Arrange (Hazirlik)
        int productId = 1;
        int orderQuantity = 2;
        var product = new Product { Id = productId, Name = "Laptop", Price = 1000, StockQuantity = 10 };

        // Mock ayarlari: GetProductById cagirildiginda bizim urunu don
        _mockRepo.Setup(repo => repo.GetProductById(productId)).Returns(product);
        // Mock ayarlari: UpdateStock cagirildiginda true don (basarili say)
        _mockRepo.Setup(repo => repo.UpdateStock(productId, It.IsAny<int>())).Returns(true);

        // Act (Eylem)
        decimal totalPrice = _orderService.PlaceOrder(productId, orderQuantity);

        // Assert (Dogrulama)
        Assert.Equal(2000, totalPrice); // 2 * 1000 = 2000 olmali
        
        // Stok guncelleme metodunun cagirildigini dogrula (yeni stok 8 olmali)
        _mockRepo.Verify(repo => repo.UpdateStock(productId, 8), Times.Once);
    }

    [Fact]
    public void PlaceOrder_ShouldThrowException_WhenStockIsInsufficient()
    {
        // Arrange
        int productId = 1;
        int orderQuantity = 11; // Stoktan fazla
        var product = new Product { Id = productId, Name = "Laptop", Price = 1000, StockQuantity = 10 };

        _mockRepo.Setup(repo => repo.GetProductById(productId)).Returns(product);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => _orderService.PlaceOrder(productId, orderQuantity));
        
        Assert.Contains("Insufficient stock", exception.Message);
        
        // Stok guncellemenin HIC cagirilmamasini bekliyoruz
        _mockRepo.Verify(repo => repo.UpdateStock(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public void PlaceOrder_ShouldThrowException_WhenProductNotFound()
    {
        // Arrange
        int productId = 999; // Olmayan urun
        
        _mockRepo.Setup(repo => repo.GetProductById(productId)).Returns((Product?)null);

        // Act & Assert
        Assert.Throws<KeyNotFoundException>(() => _orderService.PlaceOrder(productId, 1));
    }

    [Fact]
    public void PlaceOrder_ShouldThrowException_WhenQuantityIsZeroOrNegative()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => _orderService.PlaceOrder(1, 0));
        Assert.Throws<ArgumentException>(() => _orderService.PlaceOrder(1, -5));
    }

    [Fact]
    public void PlaceOrder_ShouldThrowException_WhenQuantityExceedsLimit()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _orderService.PlaceOrder(1, 51));
        Assert.Contains("Cannot order more than 50 items", exception.Message);
    }
}
