using Microsoft.AspNetCore.Mvc.Testing;
using Api;
using Contracts;
using System.Text.Json;
using Newtonsoft.Json;
using System.Text;
using System.Net.Http.Json;
using System.Net;
using FluentAssertions;

namespace ApiTest;

public class UnitTest1 : IClassFixture<WebApplicationFactory<Api.Startup>>
{
    private readonly WebApplicationFactory<Api.Startup> factory;
    public UnitTest1(WebApplicationFactory<Api.Startup> factory) {
        this.factory = factory;
    }
    [Fact]
    public async void Test1()
    {
        var client = factory.CreateClient();

        Args args = new Args ();
        args.Dist = new double[][] {
            new double [] {0, 1, 2},
            new double [] {1 ,0, 5},
            new double [] {2 ,5, 0}
        };
        args.MutationRate = 0.5;
        args.N = 3;
        
        var content = new StringContent(
            JsonConvert.SerializeObject(args), 
            Encoding.UTF8, 
            "application/json");

        var response = await client.PutAsync("experiments", content);
        var id = await response.Content.ReadAsStringAsync();
        
        response.EnsureSuccessStatusCode();

        content = new StringContent(
            JsonConvert.SerializeObject(id), 
            Encoding.UTF8, 
            "application/json");

        response = await client.PostAsync($"experiments/{id}", content);
        var result = await response.Content.ReadFromJsonAsync<Result>();

        response = await client.DeleteAsync($"experiments/{id}");
        response = await client.PostAsync($"experiments/{id}", content);
        response.StatusCode.Should().NotBe(HttpStatusCode.OK);
    }
}