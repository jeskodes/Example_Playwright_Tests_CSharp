using TestFramework.Core.Tests.Data;
using TestFramework.Core.Tests.Data.Users;
using TestFramework.Core.Tests.Data.Content;
using TestFramework.Core.Tests.Data.Projects;
using TestFramework.Core.Tests.Helper;
using TestFramework.Core.Tests.PageObjects.App;
using TestFramework.Core.Tests.PageObjects.App.Reports;
using TestFramework.Core.Tests.PageObjects.App.Authentication;

namespace TestFramework.Core.Tests.Tests
{
    [Parallelizable(ParallelScope.Self)]
    [TestFixture]
    public class SharedViewsTests : TestBase
    {
        [Test]
        public async Task AddViewWithDefaultLogoAndVerify()
        {
            var toastMessage = new ToastMessage(Page);
            var userRepo = new UserRepository();
            var responseRepo = new ResponseRepository();
            var contentRepo = new ContentRepository();
            var questionRepo = new QuestionRepository();
            var projectRepo = new ProjectRepository();
            var account = new UserRepository().CreateUser(11);

            // Create a test project to pull results from
            var project = projectRepo.CreateProject(account.AccountId);
            var content = contentRepo.CreateContent(project.ProjectId);

            var options = new[] { "Option A", "Option B" };
            var question = questionRepo.CreateSingleChoiceQuestion(project.ProjectId, content, "What's your preferred choice?", options);

            const int responsesCount = 2;

            for (var i = 0; i < responsesCount; i++)
            {
                var user = userRepo.CreateResponse(project.ProjectId);
                var optionId = question.Options[i].OptionId;
                responseRepo.CreateSingleChoiceResponse(user.UserId, project.ProjectId, question.QuestionId, optionId);
            }

            projectRepo.SetProjectResponses(project.ProjectId, responsesCount);

            var loginPage = new LoginPage(Page);
            await loginPage.GotoAsync();
            await loginPage.LoginAsync(account);

            var reportsSummaryPage = new ReportsSummaryPage(Page);

            // Create instances of both page objects
            var createSharedView = new CreateSharedView(Page, reportsSummaryPage);
            var sharedView = new SharedView(Page);

            await reportsSummaryPage.GotoAsync(project.ProjectId);
            await reportsSummaryPage.CloseTourIfVisibleAsync();
            await reportsSummaryPage.ClickAddChart(question.QuestionId);
            await reportsSummaryPage.ClickApplyChanges();

            // Using CreateSharedView POM
            await createSharedView.ClickShare();
            await createSharedView.ClickCreateNewShare();
            await createSharedView.ClickCreateSharingLink();
            await Expect(createSharedView.SharingLinkInput).ToBeVisibleAsync();
            var sharingLink = await createSharedView.CopySharingLinkToClipboard();
            await Expect(toastMessage.GetLastToastMessage()).ToContainTextAsync("Share link copied Successfully");

            // Using SharedView POM
            await sharedView.NavigateToSharingLink(sharingLink);
            await Expect(sharedView.DefaultLogo).ToBeVisibleAsync();
            await sharedView.ReturnToOriginalPage();
            await createSharedView.ClickCloseButton();
        }

        [Test]
        public async Task ShareViewPasswordProtected()
        {
            var userRepo = new UserRepository();
            var responseRepo = new ResponseRepository();
            var contentRepo = new ContentRepository();
            var questionRepo = new QuestionRepository();
            var projectRepo = new ProjectRepository();
            var account = new UserRepository().CreateUser(11);

            // Create a test project to pull results from
            var project = projectRepo.CreateProject(account.AccountId);
            var content = contentRepo.CreateContent(project.ProjectId);

            var options = new[] { "Red", "Blue" };
            var question = questionRepo.CreateSingleChoiceQuestion(project.ProjectId, content, "What's your favourite color?", options);

            const int responsesCount = 2;

            for (var i = 0; i < responsesCount; i++)
            {
                var user = userRepo.CreateResponse(project.ProjectId);
                var optionId = question.Options[i].OptionId;
                responseRepo.CreateSingleChoiceResponse(user.UserId, project.ProjectId, question.QuestionId, optionId);
            }

            projectRepo.SetProjectResponses(project.ProjectId, responsesCount);

            var loginPage = new LoginPage(Page);
            await loginPage.GotoAsync();
            await loginPage.LoginAsync(account);

            var reportsSummaryPage = new ReportsSummaryPage(Page);
            var createSharedView = new CreateSharedView(Page, reportsSummaryPage);
            var sharedView = new SharedView(Page);

            await reportsSummaryPage.GotoAsync(project.ProjectId);
            await reportsSummaryPage.CloseTourIfVisibleAsync();
            await reportsSummaryPage.ClickAddChart(question.QuestionId);
            await reportsSummaryPage.ClickApplyChanges();

            // Create Password Protected Shared View
            await createSharedView.ClickShare();
            await createSharedView.ClickCreateNewShare();
            await createSharedView.SetPasswordProtectedShare("TestPassword");
            await createSharedView.ClickCreateSharingLink();
            await Expect(createSharedView.SharingLinkInput).ToBeVisibleAsync();
            var sharingLink = await createSharedView.CopySharingLinkToClipboard();

            // Navigate to sharing link
            await sharedView.NavigateToSharingLink(sharingLink);

            // Wait for login modal and password input
            await Expect(sharedView.LoginModal).ToBeVisibleAsync();
            await Expect(sharedView.PasswordInput).ToBeVisibleAsync();

            // Add a delay to allow reCAPTCHA to load completely
            await Page.WaitForTimeoutAsync(3000); // 3 seconds pause

            // Enter Password
            await sharedView.PasswordInput.ClickAsync();
            await sharedView.EnterPassword("TestPassword");
            await Expect(sharedView.SharedViewTitle).ToBeVisibleAsync();
            await sharedView.ReturnToOriginalPage();
            await createSharedView.ClickCloseButton();
        }

        [Test]
        public async Task ShareCustomLogo()
        {
            var (account, project, question) = CreateTestData();

            var loginPage = new LoginPage(Page);
            await loginPage.GotoAsync();
            await loginPage.LoginAsync(account);

            var reportsSummaryPage = new ReportsSummaryPage(Page);
            var createSharedView = new CreateSharedView(Page, reportsSummaryPage);
            var sharedView = new SharedView(Page);

            await reportsSummaryPage.GotoAsync(project.ProjectId);
            await reportsSummaryPage.CloseTourIfVisibleAsync();
            await reportsSummaryPage.ClickAddChart(question.QuestionId);
            await reportsSummaryPage.ClickApplyChanges();

            // Sharing functionality starts here
            await createSharedView.ClickShare();
            await createSharedView.ClickCreateNewShare();
            await Expect(createSharedView.BrandingTab).ToBeVisibleAsync();
            await createSharedView.BrandingTab.ClickAsync();
            await Expect(createSharedView.CustomLogo).ToBeVisibleAsync();
            await createSharedView.CustomLogo.ClickAsync();
            await Expect(createSharedView.LogoUrlInput).ToBeVisibleAsync();
            const string imageUrl = "https://via.placeholder.com/150x50/blue/white?text=Company+Logo";
            await createSharedView.LogoUrlInput.FillAsync(imageUrl);
            await createSharedView.ClickCreateSharingLink();
            await Expect(createSharedView.SharingLinkInput).ToBeVisibleAsync();
            var sharingLink = await createSharedView.CopySharingLinkToClipboard();
            await sharedView.NavigateToSharingLink(sharingLink);
            await Expect(sharedView.CustomLogo).ToBeVisibleAsync();
            await Expect(sharedView.CustomLogo).ToHaveAttributeAsync("src", imageUrl);
            await sharedView.ReturnToOriginalPage();
            await createSharedView.ClickCloseButton();
        }

        [Test]
        public async Task VerifySharedViewCustomColours()
        {
            var userRepo = new UserRepository();
            var responseRepo = new ResponseRepository();
            var contentRepo = new ContentRepository();
            var questionRepo = new QuestionRepository();
            var projectRepo = new ProjectRepository();
            var account = new UserRepository().CreateUser(11);

            // Create a test project to pull results from
            var project = projectRepo.CreateProject(account.AccountId);
            var content = contentRepo.CreateContent(project.ProjectId);

            var options = new[] { "Option A", "Option B" };
            var question = questionRepo.CreateSingleChoiceQuestion(project.ProjectId, content, "What's your preference between the two options?", options);

            const int responsesCount = 2;

            for (var i = 0; i < responsesCount; i++)
            {
                var user = userRepo.CreateResponse(project.ProjectId);
                var optionId = question.Options[i].OptionId;
                responseRepo.CreateSingleChoiceResponse(user.UserId, project.ProjectId, question.QuestionId, optionId);
            }

            projectRepo.SetProjectResponses(project.ProjectId, responsesCount);

            var loginPage = new LoginPage(Page);
            await loginPage.GotoAsync();
            await loginPage.LoginAsync(account);

            var reportsSummaryPage = new ReportsSummaryPage(Page);
            var createSharedView = new CreateSharedView(Page, reportsSummaryPage);
            var sharedView = new SharedView(Page);

            await reportsSummaryPage.GotoAsync(project.ProjectId);
            await reportsSummaryPage.CloseTourIfVisibleAsync();
            await reportsSummaryPage.ClickAddChart(question.QuestionId);
            await reportsSummaryPage.ClickApplyChanges();

            // Sharing functionality starts here
            await createSharedView.ClickShare();
            await createSharedView.ClickCreateNewShare();
            await Expect(createSharedView.BrandingTab).ToBeVisibleAsync();
            await createSharedView.BrandingTab.ClickAsync();
            await Expect(createSharedView.TextColor).ToBeVisibleAsync();
            await Expect(createSharedView.TextColorInput).ToHaveValueAsync("#222222");
            await Expect(createSharedView.HeaderBackgroundColor).ToBeVisibleAsync();
            await Expect(createSharedView.HeaderBackgroundColorInput).ToHaveValueAsync("#ffffff");
            await createSharedView.SetTextColor("#800080");
            await createSharedView.SetHeaderBackgroundColor("#FFFFFF");
            await createSharedView.ClickCreateSharingLink();
            await Expect(createSharedView.SharingLinkInput).ToBeVisibleAsync();

            var sharingLink = await createSharedView.CopySharingLinkToClipboard();

            await sharedView.NavigateToSharingLink(sharingLink);
            await Expect(sharedView.SharedViewHeaderBackgroundColor).ToHaveCSSAsync("background-color", "rgb(255, 255, 255)");
            await Expect(sharedView.SharedViewHeaderTitleColour).ToHaveCSSAsync("color", "rgb(128, 0, 128)");
            await sharedView.ReturnToOriginalPage();
            await createSharedView.ClickCloseButton();
        }

        [Test]
        public async Task BusinessUserOnlyOneSharedView()
        {
            const string VIEW_NAME = "Business Share View";
            var userRepo = new UserRepository();
            var responseRepo = new ResponseRepository();
            var contentRepo = new ContentRepository();
            var questionRepo = new QuestionRepository();
            var projectRepo = new ProjectRepository();
            var account = new UserRepository().CreateUser(7);

            // Create a test project to pull results from
            var project = projectRepo.CreateProject(account.AccountId);
            var content = contentRepo.CreateContent(project.ProjectId);

            var options = new[] { "Choice 1", "Choice 2" };
            var question = questionRepo.CreateSingleChoiceQuestion(project.ProjectId, content, "What's your preference between these choices?", options);

            const int responsesCount = 2;

            for (var i = 0; i < responsesCount; i++)
            {
                var user = userRepo.CreateResponse(project.ProjectId);
                var optionId = question.Options[i].OptionId;
                responseRepo.CreateSingleChoiceResponse(user.UserId, project.ProjectId, question.QuestionId, optionId);
            }

            projectRepo.SetProjectResponses(project.ProjectId, responsesCount);

            var loginPage = new LoginPage(Page);
            await loginPage.GotoAsync();
            await loginPage.LoginAsync(account);

            var reportsSummaryPage = new ReportsSummaryPage(Page);
            var createSharedView = new CreateSharedView(Page, reportsSummaryPage);
            var sharedView = new SharedView(Page);

            await reportsSummaryPage.GotoAsync(project.ProjectId);
            await reportsSummaryPage.CloseTourIfVisibleAsync();
            await reportsSummaryPage.ClickAddChart(question.QuestionId);
            await reportsSummaryPage.ClickApplyChanges();
            await reportsSummaryPage.ClickSaveView();
            await reportsSummaryPage.FillViewName(VIEW_NAME);
            await reportsSummaryPage.ClickSaveButton();

            await createSharedView.ClickShare();
            await createSharedView.ClickCreateNewShare();
            await createSharedView.ClickCreateSharingLink();
            await Expect(createSharedView.SharingLinkInput).ToBeVisibleAsync();
            var sharingLink = await createSharedView.CopySharingLinkToClipboard();
            await createSharedView.ClickCloseButton();

            await createSharedView.ClickShare();
            await Expect(createSharedView.CreateNewShareButton).ToContainTextAsync("Create New Share");
            await createSharedView.CreateNewShareButton.ClickAsync(new LocatorClickOptions { Force = true });
            await Expect(createSharedView.ShareLimitModal).ToBeVisibleAsync();
            await Expect(createSharedView.LearnMoreButton).ToBeVisibleAsync();
        }

        [Test]
        public async Task SharedViewsDisabledForLowerPlans()
        {
            var userRepo = new UserRepository();
            var responseRepo = new ResponseRepository();
            var contentRepo = new ContentRepository();
            var questionRepo = new QuestionRepository();
            var projectRepo = new ProjectRepository();
            var account = new UserRepository().CreateUser(1);
            // Create a test project to pull results from
            var project = projectRepo.CreateProject(account.AccountId);
            var content = contentRepo.CreateContent(project.ProjectId);
            var options = new[] { "Option A", "Option B" };
            var question = questionRepo.CreateSingleChoiceQuestion(project.ProjectId, content, "What's your preference between these options?", options);
            const int responsesCount = 2;
            for (var i = 0; i < responsesCount; i++)
            {
                var user = userRepo.CreateResponse(project.ProjectId);
                var optionId = question.Options[i].OptionId;
                responseRepo.CreateSingleChoiceResponse(user.UserId, project.ProjectId, question.QuestionId, optionId);
            }

            projectRepo.SetProjectResponses(project.ProjectId, responsesCount);
            var loginPage = new LoginPage(Page);
            await loginPage.GotoAsync();
            await loginPage.LoginAsync(account);
            var reportsSummaryPage = new ReportsSummaryPage(Page);
            var createSharedView = new CreateSharedView(Page, reportsSummaryPage);
            await reportsSummaryPage.GotoAsync(project.ProjectId);
            await reportsSummaryPage.CloseTourIfVisibleAsync();
            await createSharedView.ClickShare();
            await createSharedView.ClickCreateNewShare();
            await Expect(createSharedView.SharedViewsUpgradeModal).ToBeVisibleAsync();
        }

        private (LoginAccount account, Project project, Question question) CreateTestData()
        {
            var userRepo = new UserRepository();
            var responseRepo = new ResponseRepository();
            var contentRepo = new ContentRepository();
            var questionRepo = new QuestionRepository();
            var projectRepo = new ProjectRepository();
            var account = new UserRepository().CreateUser(11);

            var project = projectRepo.CreateProject(account.AccountId);
            var content = contentRepo.CreateContent(project.ProjectId);

            var options = new[] { "Alpha", "Beta" };
            var question = questionRepo.CreateSingleChoiceQuestion(project.ProjectId, content, "Select your preferred option", options);

            const int responsesCount = 2;
            for (var i = 0; i < responsesCount; i++)
            {
                var user = userRepo.CreateResponse(project.ProjectId);
                var optionId = question.Options[i].OptionId;
                responseRepo.CreateSingleChoiceResponse(user.UserId, project.ProjectId, question.QuestionId, optionId);
            }

            projectRepo.SetProjectResponses(project.ProjectId, responsesCount);
            return (account, project, question);
        }
    }
}
