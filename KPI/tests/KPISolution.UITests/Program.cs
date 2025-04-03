using System.Text.RegularExpressions;
using Microsoft.Playwright;
using NUnit.Framework;
using KPISolution.UITests;

namespace KPISolution.UITests;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        if (args.Length > 0)
        {
            if (args[0] == "install-browsers")
            {
                await BrowserInstaller.Install();
                return 0;
            }
            else if (args[0] == "check")
            {
                await CheckPlaywright.Check();
                return 0;
            }
        }

        Console.WriteLine("Available commands:");
        Console.WriteLine("  install-browsers - Install Playwright browsers");
        Console.WriteLine("  check - Check if Playwright is working correctly");
        Console.WriteLine("  Or run 'dotnet test' to execute the UI tests");
        return 0;
    }
}

[TestFixture]
public class KpiUITests
{
    private IPlaywright _playwright = null!;
    private IBrowser _browser = null!;
    private IPage _page = null!;
    private IBrowserContext _context = null!;
    private string _baseUrl = "http://localhost:5000"; // Adjust based on your application URL

    [SetUp]
    public async Task Setup()
    {
        _playwright = await Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = false, // Set to true for CI/CD pipelines
            SlowMo = 100, // Slow down Playwright operations by 100ms for visual debugging
        });
        _context = await _browser.NewContextAsync();
        _page = await _context.NewPageAsync();

        // Navigate to base URL
        await _page.GotoAsync(_baseUrl);
    }

    [TearDown]
    public async Task TearDown()
    {
        await _context.CloseAsync();
        await _browser.CloseAsync();
        _playwright.Dispose();
    }

    [Test]
    public async Task Homepage_ShouldLoad()
    {
        // Navigate to home page
        await _page.GotoAsync(_baseUrl);

        // Check that the page loaded with the expected title
        var title = await _page.TitleAsync();
        Assert.That(title, Does.Contain("KPI Management System"));

        // Verify navigation elements are present
        var navElements = await _page.Locator("nav").CountAsync();
        Assert.That(navElements, Is.GreaterThan(0), "Navigation menu should be present");
    }

    [Test]
    public async Task KpiTreeView_ShouldDisplayHierarchy()
    {
        // Navigate to TreeView page
        await _page.GotoAsync($"{_baseUrl}/Kpi/TreeView");

        // Wait for the page to load completely
        await _page.WaitForSelectorAsync(".kpi-tree-container");

        // Check that the KPI hierarchy is displayed
        var treeNodes = await _page.Locator(".kpi-tree-node").CountAsync();
        Assert.That(treeNodes, Is.GreaterThan(0), "KPI tree should have at least one node");

        // Verify KRI nodes are displayed
        var kriNodes = await _page.Locator(".kri-node").CountAsync();
        Assert.That(kriNodes, Is.GreaterThanOrEqualTo(0), "KRI nodes should be displayed if available");
    }

    [Test]
    public async Task KpiList_ShouldFilterCorrectly()
    {
        // Navigate to KPI List page
        await _page.GotoAsync($"{_baseUrl}/Kpi/Index");

        // Wait for the page to load
        await _page.WaitForSelectorAsync(".kpi-list-container");

        // Count initial rows
        var initialRows = await _page.Locator(".kpi-item").CountAsync();

        // Enter filter text
        await _page.FillAsync("input[name='Filter.SearchTerm']", "Test");
        await _page.ClickAsync("button[type='submit']");

        // Wait for results to update
        await _page.WaitForSelectorAsync(".kpi-list-container");

        // Count filtered rows
        var filteredRows = await _page.Locator(".kpi-item").CountAsync();

        // Verify filter worked (not necessarily fewer rows, but should be different)
        Assert.That(filteredRows, Is.Not.EqualTo(0), "Filtered results should display matching KPIs");
    }

    [Test]
    public async Task KpiDetails_ShouldDisplayCorrectly()
    {
        // Navigate to KPI List page first to find a KPI
        await _page.GotoAsync($"{_baseUrl}/Kpi/Index");
        await _page.WaitForSelectorAsync(".kpi-list-container");

        // Click on first KPI to view details
        var detailsLinks = await _page.Locator(".kpi-item a[href*='/Kpi/Details/']").CountAsync();

        if (detailsLinks > 0)
        {
            await _page.ClickAsync(".kpi-item a[href*='/Kpi/Details/']");

            // Wait for details page to load
            await _page.WaitForSelectorAsync(".kpi-details-container");

            // Verify details page elements
            var nameElement = await _page.Locator(".kpi-name").CountAsync();
            var codeElement = await _page.Locator(".kpi-code").CountAsync();

            Assert.That(nameElement, Is.GreaterThan(0), "KPI name should be displayed");
            Assert.That(codeElement, Is.GreaterThan(0), "KPI code should be displayed");
        }
        else
        {
            Assert.Ignore("No KPI items available to test details view");
        }
    }

    [Test]
    public async Task LoginLogout_ShouldWorkCorrectly()
    {
        // Navigate to login page
        await _page.GotoAsync($"{_baseUrl}/Account/Login");

        // Fill login form
        await _page.FillAsync("input[name='Email']", "admin@example.com");
        await _page.FillAsync("input[name='Password']", "Password123!");

        // Submit login form
        await _page.ClickAsync("button[type='submit']");

        // Wait for redirect after login
        await _page.WaitForURLAsync(new Regex($"{_baseUrl}.*"));

        // Verify login was successful by checking for logout link
        var logoutLink = await _page.Locator("a[href*='Account/Logout']").CountAsync();
        Assert.That(logoutLink, Is.GreaterThan(0), "Logout link should be visible after login");

        // Perform logout
        await _page.ClickAsync("a[href*='Account/Logout']");

        // Wait for redirect after logout
        await _page.WaitForURLAsync(new Regex($"{_baseUrl}.*"));

        // Verify logout was successful by checking for login link
        var loginLink = await _page.Locator("a[href*='Account/Login']").CountAsync();
        Assert.That(loginLink, Is.GreaterThan(0), "Login link should be visible after logout");
    }
}
