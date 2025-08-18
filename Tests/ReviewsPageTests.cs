using TestFramework.Core.Tests.Data;
using TestFramework.Core.Tests.Data.Users;
using TestFramework.Core.Tests.Data.Reviews;
using TestFramework.Core.Tests.Helper;
using TestFramework.Core.Tests.PageObjects.App;
using TestFramework.Core.Tests.PageObjects.App.Reviews;
using TestFramework.Core.Tests.PageObjects.App.Authentication;

namespace TestFramework.Core.Tests.Tests.ReviewsManagement
{
    public class ReviewsPageTests : TestBase
    {
        [Test]
        public async Task DeleteSourceAddSourceButtonReappears()
        {
            // Create a test user first
            var toastMessage = new ToastMessage(Page);
            var masterAccount = new UserRepository().CreateUserWithOrganisation();
            var organisationRepository = new OrganisationRepository();
            var organisation = organisationRepository.GetOrganisationByOwnerId(masterAccount.AccountId);
            var reviewSource = new ReviewSourceRepository().CreateReviewSource(organisation.OrganisationId);
            new ReviewRepository().CreateReviews(reviewSource.ReviewSourceId);

            var reviewsSetup = new ReviewsPage(Page);

            // Login and setup test data in one step
            var loginPage = new LoginPage(Page);
            await loginPage.GotoAsync();
            await loginPage.LoginAsync(masterAccount);

            await reviewsSetup.GotoReviewsPageAsync();

            // Delete source and verify toast message and Add Source button reappears
            await reviewsSetup.ClickEllipsisToRightOfSource();
            await Expect(reviewsSetup.GetDeleteSourceButton()).ToBeVisibleAsync();
            await reviewsSetup.ClickDeleteSourceButton();
            await reviewsSetup.ClickDeleteConfirmInput();
            await reviewsSetup.EnterDeleteConfirmInput("delete");
            await reviewsSetup.ClickConfirmDeleteButton();
            await Expect(toastMessage.GetLastToastMessage()).ToContainTextAsync("Source deleted successfully.");
            await Expect(reviewsSetup.GetAddSourceButton()).ToBeVisibleAsync(new() { Timeout = 10000 });
        }

        [Test]
        public async Task ReviewsBusinessUserAddMoreSourcesUpgradeModal()
        {
            // Create a test user first
            var masterAccount = new UserRepository().CreateUserWithOrganisation(7);
            var organisationRepository = new OrganisationRepository();
            var organisation = organisationRepository.GetOrganisationByOwnerId(masterAccount.AccountId);
            var reviewSource = new ReviewSourceRepository().CreateReviewSource(organisation.OrganisationId);
            new ReviewRepository().CreateReviews(reviewSource.ReviewSourceId);

            var reviewsSetup = new ReviewsPage(Page);

            // Login and setup test data in one step
            var loginPage = new LoginPage(Page);
            await loginPage.GotoAsync();
            await loginPage.LoginAsync(masterAccount);

            await reviewsSetup.GotoReviewsPageAsync();

            // Verify the usage count is 1/1
            await Expect(reviewsSetup.GetUsageCount()).ToContainTextAsync("1/1");

            // Verify Limit warning is visible
            await Expect(reviewsSetup.GetLimitReachedText()).ToBeVisibleAsync();

            // Verify Limit banner is present
            await Expect(reviewsSetup.GetLimitReachedBanner()).ToBeVisibleAsync();

            // Click the Add Source button
            await reviewsSetup.ClickAddSourceButton();

            // Verify the modal opens and is the correct one
            await Expect(reviewsSetup.GetAddMoreReviewSourcesUpgradeModal()).ToBeVisibleAsync();
        }

        [Test]
        public async Task Reviews_ReplyThenUpdateItThenDeleteIt()
        {
            const string reply = "Thanks for your feedback!";
            const string editedReply = "Thank you for bringing this to our attention. We've looked into this matter and have taken appropriate action.";

            // Create a test user first
            var masterAccount = new UserRepository().CreateUserWithOrganisation(7);
            var organisationRepository = new OrganisationRepository();
            var organisation = organisationRepository.GetOrganisationByOwnerId(masterAccount.AccountId);
            var reviewSource = new ReviewSourceRepository().CreateReviewSource(organisation.OrganisationId);
            new ReviewRepository().CreateReviews(reviewSource.ReviewSourceId);

            var reviewsSetup = new ReviewsPage(Page);

            // Login and setup test data in one step
            var loginPage = new LoginPage(Page);
            await loginPage.GotoAsync();
            await loginPage.LoginAsync(masterAccount);
            await reviewsSetup.GotoReviewsPageAsync();

            await reviewsSetup.ClickReplyToReviewButton(0);

            // Reply
            await reviewsSetup.FillReplyText(reply);
            await reviewsSetup.ClickSendReply();
            await reviewsSetup.WaitForReplyConfirmation();

            // Edit the reply
            await reviewsSetup.ClickReplyOption(ReviewsPage.ReplyOption.Edit);
            await reviewsSetup.FillReplyText(editedReply);
            await reviewsSetup.ClickSendReply();
            await reviewsSetup.WaitForReplyConfirmation();

            // Delete the reply
            await reviewsSetup.ClickReplyOption(ReviewsPage.ReplyOption.Delete);
            await reviewsSetup.ClickConfirmDelete();
            await reviewsSetup.WaitForReplyDeleteConfirmation();
        }
    }
}
