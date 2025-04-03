using Microsoft.Playwright;
using System;
using System.Threading.Tasks;

namespace KPISolution.UITests;

public static class CheckPlaywright
{
    public static async Task Check()
    {
        try
        {
            Console.WriteLine("Checking Playwright installation...");

            var playwright = await Playwright.CreateAsync();
            try
            {
                Console.WriteLine("Playwright initialized successfully.");

                // Try to launch a browser to verify installation
                var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
                {
                    Headless = true
                });

                try
                {
                    Console.WriteLine("Successfully launched Chromium browser!");

                    // Create a page
                    var page = await browser.NewPageAsync();
                    try
                    {
                        await page.GotoAsync("https://playwright.dev");
                        Console.WriteLine("Successfully loaded a test page.");
                        Console.WriteLine("Playwright is working correctly!");
                    }
                    finally
                    {
                        await page.CloseAsync();
                    }
                }
                finally
                {
                    await browser.CloseAsync();
                }
            }
            finally
            {
                playwright.Dispose();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error testing Playwright: {ex.Message}");
            Console.WriteLine("You may need to install browsers manually with 'playwright install'");
        }
    }
}