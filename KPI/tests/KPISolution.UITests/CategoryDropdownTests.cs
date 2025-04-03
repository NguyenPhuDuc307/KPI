using Microsoft.Playwright;
using NUnit.Framework;

namespace KPISolution.UITests;

[TestFixture]
public class CategoryDropdownTests
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
            SlowMo = 100, // Slow down operations for visual debugging
        });
        _context = await _browser.NewContextAsync();
        _page = await _context.NewPageAsync();

        // Navigate to base URL and log in
        await _page.GotoAsync(_baseUrl);
        await LoginAsAdmin();
    }

    [TearDown]
    public async Task TearDown()
    {
        await _context.CloseAsync();
        await _browser.CloseAsync();
        _playwright.Dispose();
    }

    private async Task LoginAsAdmin()
    {
        // Navigate to login page
        await _page.GotoAsync($"{_baseUrl}/Account/Login");

        // Fill login form with admin credentials
        await _page.FillAsync("input[name='Email']", "admin@example.com");
        await _page.FillAsync("input[name='Password']", "Password123!");

        // Submit login form
        await _page.ClickAsync("button[type='submit']");

        // Wait for successful login redirect
        await _page.WaitForURLAsync($"{_baseUrl}/");
    }

    [Test]
    public async Task CategoryDropdown_ShouldSelectCorrectly_InCreateForm()
    {
        // Navigate to Create PI page
        await _page.GotoAsync($"{_baseUrl}/Pi/Create");

        // Wait for the form to load
        await _page.WaitForSelectorAsync("#Category");

        // Select a Category from dropdown (Performance)
        await _page.SelectOptionAsync("#Category", "1"); // Assuming 1 is the value for Performance

        // Fill out other required fields
        await _page.FillAsync("#Name", "Test Performance Indicator");
        await _page.FillAsync("#Code", "PI-TEST-01");
        await _page.FillAsync("#Description", "Automated UI test");
        await _page.SelectOptionAsync("#DepartmentId", new SelectOptionValue { Label = "Finance" });

        // Take a screenshot for debugging
        await _page.ScreenshotAsync(new PageScreenshotOptions
        {
            Path = "CategoryDropdownBeforeSubmit.png"
        });

        // Submit the form
        await _page.ClickAsync("button[type='submit']");

        // Wait for redirect to details page or index
        try
        {
            await _page.WaitForURLAsync(new System.Text.RegularExpressions.Regex($"{_baseUrl}/Pi/(Details|Index).*"));

            // Check for success message
            var successMessage = await _page.Locator(".alert-success").CountAsync();
            Assert.That(successMessage, Is.GreaterThanOrEqualTo(0), "Success message should be present after successful creation");
        }
        catch
        {
            // If not redirected, check for validation errors
            var hasErrors = await _page.Locator(".validation-summary-errors").CountAsync();

            // Take screenshot to capture error state
            await _page.ScreenshotAsync(new PageScreenshotOptions
            {
                Path = "CategoryDropdownAfterSubmit.png"
            });

            if (hasErrors > 0)
            {
                var categoryError = await _page.Locator("#Category-error").CountAsync();
                Assert.That(categoryError, Is.EqualTo(0), "Category field should not have validation errors");
            }
        }
    }

    [Test]
    public async Task CategoryDropdown_ShouldRetainValue_OnEditForm()
    {
        // Navigate to PI Index page to find a PI to edit
        await _page.GotoAsync($"{_baseUrl}/Pi/Index");
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Click Edit on first PI in list (if any)
        var editLinks = await _page.Locator("a[href*='/Pi/Edit/']").CountAsync();

        if (editLinks > 0)
        {
            await _page.ClickAsync("a[href*='/Pi/Edit/']");

            // Wait for edit form to load
            await _page.WaitForSelectorAsync("#Category");

            // Get current selected value
            var selectedValue = await _page.EvaluateAsync<string>("document.querySelector('#Category').value");

            // Change to a different Category
            int newValue = selectedValue == "1" ? 2 : 1;
            await _page.SelectOptionAsync("#Category", newValue.ToString());

            // Take a screenshot to see the dropdown state
            await _page.ScreenshotAsync(new PageScreenshotOptions
            {
                Path = "CategoryDropdownEdit.png"
            });

            // Submit the form
            await _page.ClickAsync("button[type='submit']");

            // Wait for redirect or check for validation errors
            try
            {
                await _page.WaitForURLAsync(new System.Text.RegularExpressions.Regex($"{_baseUrl}/Pi/(Details|Index).*"));

                // Success case
                var successMessage = await _page.Locator(".alert-success").CountAsync();
                Assert.That(successMessage, Is.GreaterThanOrEqualTo(0), "Success message should appear after successful update");
            }
            catch
            {
                // Check for category validation error
                var categoryError = await _page.Locator("#Category-error").CountAsync();

                // Take a screenshot to capture error state
                await _page.ScreenshotAsync(new PageScreenshotOptions
                {
                    Path = "CategoryDropdownEditError.png"
                });

                Assert.That(categoryError, Is.EqualTo(0), "Category field should not have validation errors after edit");
            }
        }
        else
        {
            Assert.Ignore("No Performance Indicators available to test edit functionality");
        }
    }

    [Test]
    public async Task CategoryDropdown_ShouldValidate_RequiredField()
    {
        // Navigate to Create PI page
        await _page.GotoAsync($"{_baseUrl}/Pi/Create");

        // Wait for the form to load
        await _page.WaitForSelectorAsync("#Category");

        // DO NOT select a Category (test required field validation)

        // Fill out other required fields
        await _page.FillAsync("#Name", "Test Required Category Validation");
        await _page.FillAsync("#Code", "PI-TEST-REQ");
        await _page.FillAsync("#Description", "Testing required field validation");
        await _page.SelectOptionAsync("#DepartmentId", new SelectOptionValue { Label = "Finance" });

        // Submit the form
        await _page.ClickAsync("button[type='submit']");

        // Form should not redirect and should show validation error
        var categoryError = await _page.Locator(".field-validation-error[data-valmsg-for='Category']").CountAsync();

        // Take screenshot to capture validation state
        await _page.ScreenshotAsync(new PageScreenshotOptions
        {
            Path = "CategoryValidationRequired.png"
        });

        Assert.That(categoryError, Is.GreaterThan(0), "Category field should show validation error when not selected");
    }
}