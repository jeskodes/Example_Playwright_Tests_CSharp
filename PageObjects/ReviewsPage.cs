using TestFramework.Core.Tests.Data.Users;
using TestFramework.Core.Tests.Data.Reviews;
using TestFramework.Core.Tests.Helper;
using TestFramework.Core.Tests.PageObjects.App.Authentication;
using TestFramework.Shared.DataAccess;
using TestFramework.Shared.Models;
using TestFramework.Shared.Test.Classes;

namespace TestFramework.Core.Tests.PageObjects.App.Reviews
{
    public class ReviewsSetupPage
    {
        private readonly IPage page;

        public ReviewsSetupPage(IPage page)
        {
            this.page = page;
        }

        public async Task GotoReviewsPageAsync()
        {
            await page.GotoAsync("c/reviews/");
            await ReviewsPageHeader.WaitForAsync(); // Ensures page is loaded
        }

        // Locator for the Reviews Page Header
        public ILocator ReviewsPageHeader => page.Locator("h1:has-text('Reviews Management')");

        // Locator for the Add Source Button
        public ILocator AddSourceButton => page.Locator("button:has-text('Add Source')").Nth(0);

        // Click on Source Ellipsis
        public ILocator ClickOnEllipsisToRightOfSource => page.Locator("button[aria-label='Open menu']");

        // Delete Source
        public ILocator DeleteSourceButton => page.Locator("span.p-menuitem-text:has-text('Delete Source')");

        public ILocator DeleteSourceModal => page.Locator("h3:has-text('Delete Source')");

        public ILocator DeleteConfirmInput => page.Locator("#txtDeleteConfirm");

        public ILocator ConfirmDeleteButton => page.Locator("button:has-text('Delete')[aria-label='Confirm delete']");

        // Add More Review Sources Upgrade Modal
        public ILocator AddMoreReviewSourcesUpgradeModal => page.Locator("h1:has-text('Add More Review Sources')");

        // Locator for the credit usage label (e.g. "1/1")
        public ILocator UsageCount => page.Locator(".global-credit-usage__header > div:nth-child(2)");

        // Locator for the "Limit Reached" warning text
        public ILocator LimitReachedText => page.Locator(".global-credit-usage__header--title", new() { HasTextString = "Limit Reached" });

        // Locator for the container that has the is-limit-reached attribute
        public ILocator LimitReachedBanner => page.Locator(".global-credit-usage[is-limit-reached='true']");

        // Returns the text "used/total" e.g. "1/1"
        public async Task<string> GetUsageTextAsync()
        {
            return await UsageCount.InnerTextAsync();
        }

        // Returns whether "Limit Reached" is shown
        public async Task<bool> IsLimitReachedVisibleAsync()
        {
            return await LimitReachedText.IsVisibleAsync();
        }

        // Returns whether the limit banner is visible
        public async Task<bool> IsLimitBannerVisibleAsync()
        {
            return await LimitReachedBanner.IsVisibleAsync();
        }
    }
}
