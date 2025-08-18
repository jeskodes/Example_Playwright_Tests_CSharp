using TestFramework.Core.Tests.PageObjects.App.Reports;

namespace TestFramework.Core.Tests.PageObjects.App.Reports
{
    // Page Object to Create the Shared View

    public class CreateSharedView
    {
        private readonly IPage page;
        private readonly ReportsSummaryPage reportsSummaryPage;

        public CreateSharedView(IPage page, ReportsSummaryPage reportsSummaryPage)
        {
            this.page = page;
            this.reportsSummaryPage = reportsSummaryPage;
        }

        // Locators
        public ILocator ShareButton => page.Locator("#buttonshare-view");
        public ILocator CreateFirstShareButton => page.Locator("#createFirstShare");
        public ILocator ShareModal => page.Locator("#modal_ShareMyView");
        public ILocator ModalTitle => page.Locator("h3:has-text('Create New Share')");
        public ILocator PasswordProtectedButton => page.Locator("#tooltip-createShare-access-password");
        public ILocator PasswordInput => page.Locator("#newSharePassword");
        public ILocator CreateSharingLinkButton => page.Locator("#createSharingLink");
        public ILocator SharingLinkInput => page.Locator("input[readonly][aria-labelledby='shareableLinkLbl'][class*='large form-control']");
        public ILocator CopyButton => page.Locator(".copy-to-clipboard");
        public ILocator CloseButton => page.GetByRole(AriaRole.Button, new() { Name = "Close Window" });
        public ILocator BrandingTab => page.Locator("#creatingShare-branding-tab");
        public ILocator CustomLogo => page.Locator("label:has-text('Custom logo')");
        public ILocator LogoUrlInput => page.Locator("#ReportLogoUrl");
        public ILocator TextColor => page.Locator("#textColour");
        public ILocator HeaderBackgroundColor => page.Locator("#backgroundColour");
        public ILocator TextColorInput => page.Locator("input#textColour");
        public ILocator HeaderBackgroundColorInput => page.Locator("input#backgroundColour");
        public ILocator CreateNewShareButton => page.Locator("#createNewShare");
        public ILocator ShareLimitModal => page.Locator("#SharingViewSaveLimit");
        public ILocator LearnMoreButton => page.GetByRole(AriaRole.Link, new() { Name = "Upgrade ➜" });
        public ILocator SharedViewsUpgradeModal => page.Locator("#ReportViewSharing");

        // Actions
        public async Task ClickShare()
        {
            await ShareButton.WaitForAsync();
            await ShareButton.IsVisibleAsync();
            await ShareButton.ClickAsync();
        }

        public async Task ClickCreateNewShare()
        {
            await CreateFirstShareButton.WaitForAsync();
            await CreateFirstShareButton.IsVisibleAsync();
            await CreateFirstShareButton.HoverAsync();
            await CreateFirstShareButton.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
            await CreateFirstShareButton.ClickAsync(new LocatorClickOptions { Force = true });
        }

        public async Task VerifyShareModalIsVisible()
        {
            await ShareModal.IsVisibleAsync();
            await ModalTitle.IsVisibleAsync();
        }

        public async Task SetPasswordProtectedShare(string password)
        {
            await PasswordProtectedButton.ClickAsync();
            await PasswordInput.WaitForAsync();
            await PasswordInput.FillAsync(password);
        }

        public async Task ClickCreateSharingLink()
        {
            await CreateSharingLinkButton.WaitForAsync();
            await CreateSharingLinkButton.IsVisibleAsync();
            await CreateSharingLinkButton.ClickAsync();
        }

        public async Task<string> CopySharingLinkToClipboard()
        {
            await CopyButton.WaitForAsync();
            var sharingLink = await SharingLinkInput.GetAttributeAsync("value");
            await CopyButton.ClickAsync();
            return sharingLink;
        }

        public async Task ClickCloseButton()
        {
            await CloseButton.WaitForAsync();
            await CloseButton.IsVisibleAsync();
            await CloseButton.ClickAsync(new() { Force = true });
        }

        public async Task ConfigureCustomLogo(string logoUrl)
        {
            await BrandingTab.ClickAsync();
            await CustomLogo.ClickAsync();
            await LogoUrlInput.FillAsync(logoUrl);
        }

        // Using ReportsSummaryPage within SharedViewsCreate
        public async Task GoToReportResults(int projectId)
        {
            await reportsSummaryPage.GotoAsync(projectId);
        }

        // Change the text colour of shared view
        public async Task SetTextColor(string textColor)
        {
            await TextColorInput.FillAsync(textColor);
        }

        // Change the header background colour of shared view
        public async Task SetHeaderBackgroundColor(string color)
        {
            await HeaderBackgroundColorInput.FillAsync(color);
        }
    }
}
