namespace TestFramework.Core.Tests.PageObjects.App.Reports
{
    public class TextAnalysisPage(IPage page)
    {
        // Navigate to the Text Analysis page for a specific project
        public async Task GotoAsync(int projectId)
        {
            await page.GotoAsync($"c/projects/{projectId}/reports/text-analysis/");
        }

        // Locator for welcome video
        public ILocator CloseWelcomeVideoButton => page.GetByRole(AriaRole.Button, new() { Name = "Close" });

        // Locator for Summary Tab
        public ILocator SummaryTab => page.Locator("#textAnalysis-summary");

        public async Task SelectClickQuestion()
        {
            var selectQuestion = page.Locator("[title='Text Analysis']");
            await selectQuestion.ClickAsync();
        }

        // Locators for "Categorise As" functionality
        public ILocator AddNewCategoryInput => page.Locator("input[placeholder='Add New Category']:visible");
        public ILocator ResponseItemDropdown => page.Locator("div:nth-child(2) > .response-item__meta > .response-item__left > .dropdown > .btn");
        public ILocator CategoryTableColumnNames => page.Locator("table.categories-table > tbody > tr > td:nth-child(2)");
        public ILocator PageBody => page.Locator("body");
        public ILocator CategoriseAsButton => page.Locator("button:text('Categorise As')").Nth(0);

        // Press Escape on the new category input
        public async Task PressEscapeOnNewCategoryInput()
        {
            await AddNewCategoryInput.PressAsync("Escape");
        }

        // Methods for "Categorise As" functionality
        public async Task ClickCategoriseAsButtonAsync()
        {
            await CategoriseAsButton.ClickAsync();
            // Wait for modal/input to be visible after click
            await page.WaitForSelectorAsync("input[placeholder='Add New Category']:visible", new() { State = WaitForSelectorState.Visible });
            await AddNewCategoryInput.WaitForAsync();
        }

        public async Task FillNewCategoryAsync(string categoryName)
        {
            await AddNewCategoryInput.ClickAsync();
            await AddNewCategoryInput.FillAsync(categoryName);
        }

        public ILocator AddButton => page.GetByRole(AriaRole.Button, new() { Name = "Add" });

        public async Task ClickAddButtonAsync()
        {
            await page.RunAndWaitForRequestAsync(async () =>
            {
                await AddButton.ClickAsync();
            }, "**/bulk-assign");
        }

        public ILocator GetCategoryByText(string categoryText) =>
           page.Locator($"div:text('{categoryText}')").First;

        public async Task SelectNewCategory(string categoryText)
        {
            var assignNewCategory = GetCategoryByText(categoryText);
            await assignNewCategory.IsVisibleAsync();
            await assignNewCategory.ClickAsync();
        }

        public async Task ManuallySelectCategory(string categoryText)
        {
            var locator = page.Locator("button.category-item.remaining-count");

            await locator.ClickAsync();

            var categoryLocator = page.Locator($"div.truncate.mr-1:has-text('{categoryText}')");
            await page.WaitForSelectorAsync($"div.truncate.mr-1:has-text('{categoryText}')", new() { State = WaitForSelectorState.Visible });
            await categoryLocator.WaitForAsync();
            await categoryLocator.ClickAsync();
        }

        public ILocator NewCategoryAdded => page.Locator($"div.truncate.mr-1");

        public async Task SwitchToCategoriesTab()
        {
            await page.GetByRole(AriaRole.Tab, new() { Name = "Categories" }).ClickAsync();
        }

        public async Task SwitchToSummaryTab()
        {
            await page.GetByRole(AriaRole.Tab, new() { Name = "Summary" }).ClickAsync();
        }

        public async Task SwitchToResponsesTile()
        {
            await page.GetByRole(AriaRole.Heading, new() { Name = "Responses" }).IsVisibleAsync();
        }

        // Verify Category Tracker
        public ILocator CategoryTracker => page.Locator("h4:text('Category Tracker')");
        public ILocator GetCategorySegment(string categoryName) =>
            page.Locator("tspan tspan", new() { HasText = categoryName }).First;

        public async Task SearchResponsesAwaitingHighlight(string term, bool? expectHits = null)
        {
            expectHits ??= !string.IsNullOrEmpty(term);
            var searchBox = page.Locator("#search-responses[placeholder=\"Search Responses\"]");
            await searchBox.FillAsync(term);
            await searchBox.PressAsync("Enter");
            var top1Highlight = page.Locator(".ta-responses__list .response-item:first-of-type .ta-highlighted:first-of-type");
            var awaitState = expectHits.Value ? WaitForSelectorState.Visible : WaitForSelectorState.Hidden;
            await top1Highlight.WaitForAsync(new() { State = awaitState });
        }

        // Delete a Category
        public ILocator GetCategoryByName(string categoryName) =>
                page.Locator($"div.truncate.mr-1:has-text('{categoryName}')").First;

        public ILocator GetDeleteButtonForCategory(string categoryName) =>
            GetCategoryByName(categoryName).Locator("~ button.remove-category");

        public async Task DeleteCategory(string categoryName)
        {
            await GetDeleteButtonForCategory(categoryName).ClickAsync();
        }

        // Verify if a category exists or not
        public ILocator CategoryExists(string categoryName) =>
            page.Locator($"div.truncate.mr-1:has-text('{categoryName}')");

        // Locate the word cloud container
        public ILocator WordCloudContainer => page.Locator("#word-cloud-container");

        // This locator directly targets the first text element within the word cloud container
        public ILocator FirstWordInCloud => WordCloudContainer.Locator("text.highcharts-point").First;

        // Locator for Responses in Response Table
        public ILocator ResponseItems => page.Locator("div.response-item");
    }
}
