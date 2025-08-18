using TestFramework.Core.Tests.PageObjects.App.Reports;

namespace TestFramework.Core.Tests.PageObjects.App.Reports
{
    public class SharedView
    {
        private IPage page;
        private IPage originalPage; // Store reference to the original page

        public SharedView(IPage page)
        {
            this.page = page;
            this.originalPage = page; // Initially, both reference the same page
        }

        // Locators
        public ILocator DefaultLogo => page.Locator("[data-testid='viewLogo']");
        public ILocator PasswordInput => page.Locator("#password");
        public ILocator LoginModal => page.Locator("h1.h5").Filter(new LocatorFilterOptions { HasTextString = "Login" });
        public ILocator LoginButton => page.Locator("button:has-text('Login')");
        public ILocator SharedViewTitle => page.Locator("div.view-report-header div.container h1");
        public ILocator CustomLogo => page.GetByTestId("viewLogo");
        public ILocator CustomiseButton => page.Locator("button.customise-chart-button");
        public ILocator SharedViewHeaderBackgroundColor => page.Locator("div.view-report-header");
        public ILocator SharedViewHeaderTitleColour => page.Locator("div.view-report-header h1");

        // Navigate to sharing link
        public async Task NavigateToSharingLink(string sharingLink)
        {
            // Store current page as original before creating new one
            this.originalPage = this.page;

            // Create a new page and navigate to the share link
            var newPage = await page.Context.NewPageAsync();
            await newPage.GotoAsync(sharingLink);

            // Update current page reference to the new page
            this.page = newPage;
        }

        public async Task ReturnToOriginalPage()
        {
            // Close the shared view page if it's not the original
            if (page != originalPage)
            {
                await page.CloseAsync();
            }

            // Switch back to the original page
            await originalPage.BringToFrontAsync();

            // Reset the current page reference back to original
            this.page = originalPage;
        }

        // Enter Shared View Password
        public async Task EnterPassword(string password)
        {
            await PasswordInput.WaitForAsync();
            await PasswordInput.FillAsync(password);
            await LoginButton.ClickAsync();
        }

        public async Task VerifyLoginModalVisible()
        {
            await LoginModal.IsVisibleAsync();
        }

        public async Task VerifySharedViewLoaded()
        {
            await SharedViewTitle.IsVisibleAsync();
        }

        public async Task VerifyCustomLogoDisplayed()
        {
            await CustomLogo.IsVisibleAsync();
        }
    }
}
