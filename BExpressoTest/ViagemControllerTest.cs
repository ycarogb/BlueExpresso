using BlueExpresso.API;
using BlueExpresso.Controllers;
using BlueExpresso.Models;
using BlueExpresso.Services;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace BExpressoTest
{
    public class ViagemControllerTest
    {
        int viagemQuantity = 10;
        List<Viagem> fakeViagens;

        public ViagemControllerTest()
        {
            fakeViagens = new List<Viagem>();
            for (var i = 1; i < viagemQuantity; i++)
                fakeViagens.Add(new Viagem { Id = i, Cliente = $"Cliente {i}" });
        }

        [Fact]
        public void GetViagens_Returns_The_Correct_Viagens()
        {
            var productService = A.Fake<IViagemService>();
            A.CallTo(() => productService.GetAll()).Returns(fakeViagens); //Quando chamar o All, retorne fake Products
            var controller = new ViagemController(productService);

            OkObjectResult result = controller.Index() as OkObjectResult;

            var values = result.Value as APIResponse<List<Viagem>>;

            Assert.True(
                values.Results == fakeViagens &&
                values.Message == "" &&
                values.Succeed
                );
        }

        [Theory]
        [InlineData(1)]
        [InlineData(8)]
        [InlineData(13, "Viagem não encontrada!", false)]
        [InlineData(0, "Viagem não encontrada!", false)]
        [InlineData(-98, "Viagem não encontrada!", false)]
        public void GetViagem_Returns_Viagem_By_Id(int id, string message = "", bool succeed = true)
        {
            var productService = A.Fake<IViagemService>();
            A.CallTo(() => productService.Get(id)).Returns(fakeViagens.Find(p => p.Id == id));

            var controller = new ViagemController(productService);

            ObjectResult result = controller.Index(id) as ObjectResult;

            var exists = fakeViagens.Find(p => p.Id == id) != null;

            if (exists)
            {
                var values = result.Value as APIResponse<Viagem>;
                Assert.True(
                    values.Succeed == succeed &&
                    values.Message == message &&
                    values.Results == fakeViagens.Find(p => p.Id == id)
                    );
            }
            else
            {
                var values = result.Value as APIResponse<string>;
                Assert.True(
                    values.Succeed == succeed &&
                    values.Message == message &&
                    values.Results == null
                    );
            }
        }
    }
}
