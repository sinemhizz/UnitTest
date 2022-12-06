using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitTest.Web.Controllers;
using UnitTest.Web.Models;
using UnitTest.Web.Repository;

namespace UnitTest.Test
{
    public class ProductApiControllerTest
    {
        private readonly Mock<IRepository<Product>> _mockRepo;
        private readonly ProductsApiController _controller;
        private List<Product> products;

        public ProductApiControllerTest()
        {
            _mockRepo = new Mock<IRepository<Product>>();
            _controller = new ProductsApiController(_mockRepo.Object);

            products = new List<Product>()
            {
                new Product
                {
                    Id = 1, Name= "Kalem", Price =15, Stock=1000, Color ="Kırmızı"
                },
                new Product
                {
                    Id = 2, Name= "Kitap", Price =20, Stock=100, Color ="Mavi"
                }
            };
        }

        [Fact]
        public async void GetProducts_ActionExecutes_ReturnProductList()
        {
            _mockRepo.Setup(repo => repo.GetAll()).ReturnsAsync(products);
            var result =await _controller.GetProducts();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnProduct = Assert.IsAssignableFrom<IEnumerable<Product>>(okResult.Value);

            Assert.Equal<int>(2,returnProduct.Count());
        }

        [Theory]
        [InlineData(0)]
        public async void GetProduct_IdInValid_ReturnNotFound(int productId)
        {
            Product product = null;
            _mockRepo.Setup(x => x.GetById(productId)).ReturnsAsync(product);
            var result =await _controller.GetProduct(productId); 
            Assert.IsType<NotFoundResult>(result);

        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public async void GetProduct_IdValid_ReturnGetProduct(int productId)
        {
            var product =products.First(x=>x.Id== productId);
            _mockRepo.Setup(x => x.GetById(productId)).ReturnsAsync(product);
            var result = await _controller.GetProduct(productId); //verilen ıd'ye göre gelen ürün
            var okResult = Assert.IsType<OkObjectResult>(result); //beklediğimiz olumlu değer mi?
            var returnProduct= Assert.IsType<Product>(okResult.Value); //beklediğimiz olumlu değeri alıyoruz ve
                                                                      //dönen değerin tipi Product mı ?

            Assert.Equal(productId, returnProduct.Id); // girilen Id bize dönen Id ile eşleşiyor mu?
            Assert.Equal(product.Name, returnProduct.Name);
        }

        [Theory]
        [InlineData(1)]
        public async void PutProduct_IdIsNotEqualProduct_ReturnBadRequest(int productId)
        {
            var product = products.First(x=>x.Id== productId);
            var result = _controller.PutProduct(2,product);

            Assert.IsType<BadRequestResult>(result);
        }

        [Theory]
        [InlineData(1)]
        public  void PutProduct_ActionExecutest_ReturnNoContent(int productId)
        {
            var product = products.First(x=>x.Id== productId);

            _mockRepo.Setup(x => x.Update(product));

            var result = _controller.PutProduct(productId,product);
            _mockRepo.Verify(x=>x.Update(product), Times.Once);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async void CreateProduct_ActionExecutes_ReturnCreatedAction()
        {
            var product = products.First();
            _mockRepo.Setup(x => x.Create(product)).Returns(Task.CompletedTask);

            var result = await _controller.PostProduct(product);

            var createdAtActionResult= Assert.IsType<CreatedAtActionResult>(result);

            _mockRepo.Verify(x=>x.Create(product), Times.Once);

            Assert.Equal("GetProduct", createdAtActionResult.ActionName);
        }

        [Theory]
        [InlineData(0)]
        public async void DeleteProduct_IdInValid_ReturnNotFound(int productId)
        {
            Product product = null;
            _mockRepo.Setup(x=>x.GetById(productId)).ReturnsAsync(product);
            var resultNotFound = await _controller.DeleteProduct(productId);
            Assert.IsType<NotFoundResult>(resultNotFound);
        }

        [Theory]
        [InlineData(1)]
        public async void DeleteProduct_IdValid_ReturnNoContent(int productId)
        {
            var product = products.First(x=>x.Id==productId);

            _mockRepo.Setup(x => x.GetById(productId)).ReturnsAsync(product);
            _mockRepo.Setup(x => x.Delete(product));

            var result = await _controller.DeleteProduct(productId);

            _mockRepo.Verify(x=>x.Delete(product), Times.Once);
            Assert.IsType<NoContentResult>(result);
        }
    }
}
