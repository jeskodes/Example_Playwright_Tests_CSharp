
# Playwright C# Test Automation Examples

Test automation portfolio showing different types of tests for a survey and analytics application using Playwright and C#.

## What These Tests Do

### Visual Regression Testing (`ChartsTests.cs`)
**Type of testing:** Visual regression testing

**What it tests:** A user creates a pie chart from survey data and the chart displays correctly.

**How it works:**
1. Creates fake survey data (6 responses for different colors)
2. User logs in and goes to the reports page
3. Clicks "Add Chart" and selects "Pie Chart"
4. Takes a screenshot of the chart
5. Compares it with a saved "good" version to verify the chart displays as expected.

**Why this is important:** Charts can break when code changes or CSS updates. Visual regression testing catches these bugs automatically before users see them.

### End-to-End Integration Testing (`SharedViewsTests.cs`)
**Type of testing:** End-to-end workflows, multi-user scenarios, and security testing

**What it tests:** Users can share their reports with other people via links.

**How it works:**
1. User creates a report with charts
2. Clicks "Share" and creates a sharing link
3. Test opens the link in a new browser tab (simulating external user access)
4. Checks the shared page shows correctly
5. Tests password protection and custom branding work
6. Validates different user permission levels

**Why this is important:** Sharing involves multiple systems working together - authentication, permissions, URL generation, and cross-page functionality. E2E testing ensures the complete user journey works.

### Functional and Data Integrity Testing (`TextAnalysisPageTests.cs`) 
**Type of testing:** Functional testing, CRUD operations

**What it tests:** Users can categorize and search through text responses.

**How it works:**
1. Creates fake survey with 6 text responses about a product
2. User goes to text analysis page
3. Creates categories like "Positive Feedback" and "Bug Reports"
4. Assigns responses to categories
5. Tests search highlighting works
6. Deletes a category and checks it stays deleted after page refresh

**Why this is important:** Data operations (create, read, update, delete) are critical business functions. This tests data persistence, search functionality, and ensures user changes are properly saved.

### User Experience and Smoke Testing (`ProductTours.cs`)
**Type of testing:** UX validation, onboarding flow testing, and smoke tests

**What it tests:** New users get guided tours showing them how to use the app.

**How it works:**
1. User logs in for first time
2. Product tour popup appears saying "Welcome!"
3. Test clicks through each tour step
4. Checks each step shows the right information
5. Confirms tour closes properly at the end

**Why this is important:** First impressions matter. Broken onboarding loses users immediately. This validates critical user experience flows and serves as smoke testing for core navigation.

### Review Management Testing (`ReviewsPageTests.cs`)
**Type of testing:** Regression testing and data integrity validation with realistic test data

**What it tests:** Business owners managing customer reviews and feedback sources.

**How it works:**
1. Creates fake review data using Bogus library - *this aspect is not my work and I initially had a working test which directly hooked up to the database to add fake data; while this worked it was much less efficient than using the Bogus Library.* 
2. Business owner adds sources to collect customer reviews 
3. Replies to customer reviews, edits replies, and deletes replies
4. Deletes entire review sources with confirmation prompts
5. Tests what happens when users hit their plan limits (upgrade modals)
6. Checks success messages appear when actions work

**Why this is important:** These features were previously tested manually, so automation prevents regression bugs when code changes. Data operations like deleting reviews or sources are high-risk - if they break, businesses could lose critical customer data or be unable to respond to feedback. Automated regression testing catches these breaks before customers are affected.

## Testing Strategy Demonstrated

- **Regression Testing** - Automatically catches when updates break existing features
- **Security Testing** - Validates password protection and access controls work
- **Performance Testing** - Checks page loads and interactions respond quickly
- **Smoke Testing** - Confirms core functionality works before deeper testing

## How The Code Is Organised

- **Tests/** - The actual test files that run the scenarios above
- **PageObjects/** - Separates UI interactions from test logic to improve maintainability and readability.
- **Helpers/** - Shared code for taking screenshots and creating fake test data
- **Data/Images/** - Where chart screenshots are saved for comparison

## What This Shows About My Testing Skills

- I understand **different types of testing** and when to use each approach
- I can automate **complex user journeys** that span multiple pages and systems
- I write tests that provide **business value** by catching real bugs
- I follow **industry best practices** like Page Object Model and data-driven testing
- I implement **modern testing techniques** like visual regression testing
- My code is **maintainable and scalable** for long-term test suite growth

## Tools and Technologies

- **Playwright** - UI automation framework
- **C#** - Strongly-typed programming with async/await patterns
- **ImageSharp** - Visual comparison tool
- **Claude.ai and ChatGPT** - AI-assisted development for faster delivery

## Quality Assurance Approach

These examples demonstrate my understanding that **test automation should:**
- **Catch bugs early** in the development cycle
- **Provide fast feedback** to developers
- **Reduce manual testing effort** on repetitive tasks or tasks previously manually tested that can be fragile
- **Increase confidence** in releases
- **Document expected behavior** through executable specifications

## Important Note

*I used AI tools to continue to develop my knowledge and skills of Playwright and to accelerate development and ensure best practices.*
<br>
*Each pull request was first reviewed by a Senior Dev and changes were made according to their valuable feedback and advice.*
<br>
*These are real tests from production applications, but I've anonymised the business logic to maintain confidentiality.*

