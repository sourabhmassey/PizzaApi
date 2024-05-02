using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using PizzaApi;
using PizzaApi.Tests;
using RestSharp;
using System.Net.Http;
using System.Threading.Tasks;

public abstract class TestBase
{

    private TestServer testServer;

    protected abstract HttpClient Client { get; set; }

    protected abstract PizzaDbContext DbContext { get; set; }

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        SetUpClient();
    }

    [SetUp]
    public async Task Setup()
    {
        await DatabaseHandler.InitDataBaseAsync(testServer.Host);
        this.DbContext = testServer.Services.GetRequiredService<PizzaDbContext>();
    }

    [OneTimeTearDown]
    public void TestDown()
    {
        this.testServer.Dispose();
    }

    protected void SetUpClient()
    {
        testServer = new TestServer(new WebHostBuilder().UseStartup<Startup>());
        Client = testServer.CreateClient();
    }
}
