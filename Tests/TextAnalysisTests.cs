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
    [TestFixture]
    [Parallelizable(ParallelScope.Self)]
    public class TextAnalysisPageTests : TestBase
    {
        [Test]
        public async Task TextAnalysis()
        {
            // Initialize repositories and helpers
            var toastMessage = new ToastMessage(Page);
            var userRepo = new UserRepository();
            var responseRepo = new ResponseRepository();
            var contentRepo = new ContentRepository();
            var questionRepo = new QuestionRepository();
            var projectRepo = new ProjectRepository();

            var memberRepo = new UserRepository();
            var account = memberRepo.CreateUser(11);
            memberRepo.AddUserInteractionLogs(account.AccountId, "text-analysis-intro-popup");

            // Create a project and a question
            var project = projectRepo.CreateProject(account.AccountId);
            var projectContent = contentRepo.CreateContent(project.ProjectId);

            var textAnalysisAnswers = new[]
            {
                "Your application is intuitive and saves me a lot of time every day",
                "It's reliable and meets all my needs, but the user interface could be better",
                "It works fine, but it crashes occasionally when handling large files.",
                "The application frequently crashes and disrupts my workflow",
                "It's too basic and doesn't cater to more advanced users.",
                "The application is decent, but it could be faster and more intuitive."
            };

            var question = questionRepo.CreateSingleTextQuestion(project.ProjectId, projectContent, "How would you describe our application in one sentence?");

            // Create responses
            foreach (var t in textAnalysisAnswers)
            {
                var user = userRepo.CreateResponse(project.ProjectId);
                var optionId = question.Options[0].OptionId;
                var textValue = t.ToLower();
                responseRepo.CreateSingleTextResponse(user.UserId, project.ProjectId, question.QuestionId, optionId, textValue);
            }

            projectRepo.SetProjectResponses(project.ProjectId, textAnalysisAnswers.Length);

            // Log in
            var loginPage = new LoginPage(Page);
            await loginPage.GotoAsync();
            await loginPage.LoginAsync(account);

            // Go to Reports Summary Page
            var reportsSummaryPage = new ReportsSummaryPage(Page);
            await reportsSummaryPage.GotoAsync(project.ProjectId);

            // Go to Text Analysis Page
            var textAnalysisPage = new TextAnalysisPage(Page);
            await textAnalysisPage.GotoAsync(project.ProjectId);

            // Verify there are 6 responses to the question
            await Expect(textAnalysisPage.SummaryTab).ToContainTextAsync("6 responses");

            // Search responses
            await textAnalysisPage.SearchResponsesAwaitingHighlight("application");
            await textAnalysisPage.SearchResponsesAwaitingHighlight(string.Empty);

            // Create an array of category names
            string[] categoryNames =
            [
                "Positive Feedback",
                "UI Feedback",
                "Performance Issues",
                "Critical Issues",
                "Feature Requests",
                "General Feedback"
            ];

            // Click the categorize button
            await textAnalysisPage.ClickCategoriseAsButtonAsync();
            foreach (var t in categoryNames)
            {
                // Fill in the new category name
                await textAnalysisPage.FillNewCategoryAsync(t);

                // Click the add button
                await textAnalysisPage.ClickAddButtonAsync();
            }

            await textAnalysisPage.PressEscapeOnNewCategoryInput();

            await textAnalysisPage.ResponseItemDropdown.ClickAsync();

            // Loop to add categories
            foreach (var category in categoryNames)
            {
                await Expect(textAnalysisPage.PageBody).ToContainTextAsync(category);
            }

            await textAnalysisPage.SwitchToCategoriesTab();
            await Expect(textAnalysisPage.CategoryTableColumnNames).ToContainTextAsync(categoryNames.Order());
            await textAnalysisPage.SwitchToSummaryTab();
            await Expect(textAnalysisPage.CategoryTracker).ToBeVisibleAsync();

            // Verify category segment - names and percentage
            foreach (var categorySegment in categoryNames)
            {
                var findCategorySegment = textAnalysisPage.GetCategorySegment(categorySegment);

                // Assert that this category exists
                await Expect(findCategorySegment).ToBeVisibleAsync();
            }

            // Switch to Responses Tile
            await textAnalysisPage.SwitchToResponsesTile();
            var firstCategoryName = categoryNames[0];
            // Verify the category is visible
            await Expect(textAnalysisPage.GetCategoryByName(firstCategoryName)).ToBeVisibleAsync();

            // Delete the category
            await textAnalysisPage.DeleteCategory(firstCategoryName);

            // Verify the category has been removed
            await Expect(textAnalysisPage.CategoryExists(firstCategoryName)).Not.ToBeVisibleAsync();

            // Reload page
            await Page.ReloadAsync();

            // Verify the category is still not visible after page reload
            await Expect(textAnalysisPage.CategoryExists(firstCategoryName)).Not.ToBeVisibleAsync();
        }
    }
}
