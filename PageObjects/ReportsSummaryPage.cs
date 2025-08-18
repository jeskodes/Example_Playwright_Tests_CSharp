namespace TestFramework.Core.Tests.PageObjects.App.Reports
{
    public class ReportsSummaryPage(IPage page)
    {
        public async Task GotoAsync(int projectId)
        {
            await page.GotoAsync($"c/projects/reports/{projectId}");
        }

        // Used locator instead of handler because popup is predictable.
        // Can change to handler.
        public async Task CloseTourIfVisibleAsync()
        {
            var closeButton = page.GetByTestId("close-tour");
            if (await closeButton.IsVisibleAsync())
            {
                await closeButton.ClickAsync();
            }
        }

        public async Task ClickAddChart(int questionId)
        {
            await page.ClickAsync($"[data-testid='addChartBtn{questionId}']");
        }

        public async Task ClickApplyChanges()
        {
            var applyChanges = await page.QuerySelectorAsync("button:has-text('Apply Changes')");
            await applyChanges.ClickAsync();
        }

        public async Task ResponseOptionsTab()
        {
            var responseOptions = await page.QuerySelectorAsync("a:has-text('Response Options')");
            await responseOptions.ClickAsync();
        }

        public async Task ClickHideItemMenu(int index)
        {
            var items = await page.QuerySelectorAllAsync(".sd-response-option__icon-button.hide-show-icon.hastooltip");
            await items[index].ClickAsync();
        }

        public async Task ClickSaveView()
        {
            var saveView = await page.QuerySelectorAsync("button[type='button'][data-button-saveas='']");
            await saveView.ClickAsync();
        }

        public async Task FillViewName(string view)
        {
            var input = await page.QuerySelectorAsync(".form-control.addViewName[data-validate-message='View name must not be blank.']");
            await input.FillAsync(view);
        }

        public async Task ClickSaveButton()
        {
            await page.ClickAsync(".ZebraDialog_Button_0.btn.btn-gradient-green");
        }

        public ILocator GetResponseOptionsFromQuestionTable(int questionId)
        {
            var responseOptions = page.Locator($"#question-wrapper-{questionId}").First;
            return responseOptions;
        }

        // Locators to pin Response Tile to Dashboard
        public ILocator PinFirstSetOfResponsesToDashboardButton => page.Locator("button.btn.btn-popper.add-to-dashboard").Nth(0);
        public ILocator PinResponsesButton => page.GetByRole(AriaRole.Button, new() { Name = "Pin Responses" });
        public ILocator CreateDashboardButton => page.GetByRole(AriaRole.Button, new() { Name = "Create a Dashboard" });
        public ILocator DashboardNameInput => page.GetByPlaceholder("Name your Dashboard");
        public ILocator AddDashboardName => page.Locator(".dashboard_box").GetByRole(AriaRole.Button, new() { Name = "Add" });
        public ILocator NewDashboardCheckFirstCheckBox => page.Locator("#dashboard-item-0");
        public ILocator ButtonAddToDashboardFooter => page.Locator("div.dashboard_box__footer").Locator("button#btnAdd");
        public ILocator SeeResponsesOnDashboardButton => page.GetByRole(AriaRole.Link, new() { Name = "See Responses" });
    }
}
