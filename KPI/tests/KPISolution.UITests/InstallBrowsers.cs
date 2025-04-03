using Microsoft.Playwright;

namespace KPISolution.UITests;

public static class BrowserInstaller
{
    public static async Task Install()
    {
        Console.WriteLine("Installing Playwright browsers...");

        var exitCode = Microsoft.Playwright.Program.Main(new[] { "install" });
        if (exitCode != 0)
        {
            Console.WriteLine($"Failed to install browsers. Exit code: {exitCode}");
            return;
        }

        Console.WriteLine("Successfully installed Playwright browsers.");

        // Test that we can create a browser instance
        try
        {
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
                    Console.WriteLine("Successfully launched browser - installation confirmed.");
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
            Console.WriteLine($"Error testing browser: {ex.Message}");
        }
    }
}