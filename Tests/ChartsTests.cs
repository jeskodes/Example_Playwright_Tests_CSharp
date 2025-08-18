using Microsoft.Playwright.NUnit;
using TestFramework.Core.Tests.Data;
using TestFramework.Core.Tests.Data.Users;
using TestFramework.Core.Tests.Data.Content;
using TestFramework.Core.Tests.Data.Projects;
using TestFramework.Core.Tests.Helper;
using TestFramework.Core.Tests.PageObjects.App;
using TestFramework.Core.Tests.PageObjects.App.Reports;
using TestFramework.Core.Tests.PageObjects.App.Authentication;
using static Microsoft.Playwright.Assertions;

namespace TestFramework.Core.Tests.Tests
{
    [Parallelizable(ParallelScope.Self)]
    [TestFixture]
    public class ChartsTests : TestBase
    {
        [Test]
        public async Task MultiChoiceOnlyOneAnswerQuestion()
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

            // Create a test project to pull results from
            var project = projectRepo.CreateProject(account.AccountId);
            var content = contentRepo.CreateContent(project.ProjectId);

            var options = new[] { "Red", "Yellow", "Green", "Purple", "Blue", "Pink" };
            var question = questionRepo.CreateSingleChoiceQuestion(project.ProjectId, content, "What's your favourite colour?", options);

            var responsesCount = 6;

            for (var i = 0; i < responsesCount; i++)
            {
                var user = userRepo.CreateResponse(project.ProjectId);
                var optionId = question.Options[i].OptionId;
                responseRepo.CreateSingleChoiceResponse(user.UserId, project.ProjectId, question.QuestionId, optionId);
            }

            projectRepo.SetProjectResponses(project.ProjectId, responsesCount);

            // Log in
            var loginPage = new LoginPage(Page);
            await loginPage.GotoAsync();
            await loginPage.LoginAsync(account);

            // Go to Reports Summary Page
            var reportsSummaryPage = new ReportsSummaryPage(Page);
            await reportsSummaryPage.GotoAsync(project.ProjectId);

            await reportsSummaryPage.CloseTourIfVisibleAsync();
            await Expect(reportsSummaryPage.AddChartButton).ToBeVisibleAsync();
            await reportsSummaryPage.AddChartButton.ClickAsync();
            await Expect(reportsSummaryPage.SelectPieChart).ToBeVisibleAsync();
            await reportsSummaryPage.SelectPieChart.ClickAsync();
            await reportsSummaryPage.ApplyChangesButton.ClickAsync();
            await Expect(reportsSummaryPage.PieChart).ToBeVisibleAsync();
            await reportsSummaryPage.OpenChartFullScreenAsync();

            // Take baseline screenshot if first run
            await reportsSummaryPage.Charts.TakePieChartScreenshotAsync("results-pie", "ReportsSummary");

            // Compare current chart with baseline
            var currentPieChartLocator = reportsSummaryPage.PieChart;
            var areChartsVisuallySimilar = await reportsSummaryPage.Charts.CompareWithBaseline("results-pie", "ReportsSummary", currentPieChartLocator);

            // Assert chart matches baseline
            Assert.That(areChartsVisuallySimilar, Is.True, "The rendered pie chart does not visually match the baseline image.");
        }
    }
}
