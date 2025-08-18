using TestFramework.Core.Tests.Data.Users;
using TestFramework.Core.Tests.Helper;
using TestFramework.Core.Tests.PageObjects.App;
using TestFramework.Core.Tests.PageObjects.App.Authentication;
using TestFramework.Core.Tests.PageObjects.App.Users;
using TestFramework.Shared.Test.Classes;

namespace TestFramework.Core.Tests.Tests
{
    [TestFixture]
    public class ProductTours : TestBase
    {
        private LoginAccount loginAccount;

        [OneTimeSetUp]
        public void SetUp()
        {
            loginAccount = new UserRepository().CreateUserWithOrganisation(11);
        }

        [Test]
        public async Task DashboardProductTour()
        {
            // Initialise page objects
            var loginPage = new LoginPage(Page);
            var homePage = new HomePage(Page);

            // Log in
            await loginPage.GotoAsync();
            await loginPage.LoginAsync(loginAccount);

            // Go to the Main Dashboard page and start the tour
            var dashboardPage = await homePage.NavBar.ClickDashboardLink();
            await Expect(dashboardPage.ProductTour.TourWelcome).ToHaveTextAsync("Welcome!");
            await dashboardPage.ProductTour.ClickShowMeButton();

            await Expect(dashboardPage.ProductTour.GetShepherdLocator(1)).ToHaveTextAsync("Browse Items");
            await dashboardPage.ProductTour.ClickNextButton();

            await Expect(dashboardPage.ProductTour.GetShepherdLocator(2)).ToHaveTextAsync("Browse Categories");
            await dashboardPage.ProductTour.ClickNextButton();

            await Expect(dashboardPage.ProductTour.GetShepherdLocator(3)).ToHaveTextAsync("Organize Items");
            await dashboardPage.ProductTour.ClickNextButton();

            await Expect(dashboardPage.ProductTour.GetShepherdLocator(4)).ToHaveTextAsync("Item Options");
            await dashboardPage.ProductTour.ClickNextButton();

            await Expect(dashboardPage.ProductTour.GetShepherdLocator(5)).ToHaveTextAsync("Display Settings");
            await dashboardPage.ProductTour.ClickNextButton();

            await Expect(dashboardPage.ProductTour.GetShepherdLocator(6)).ToHaveTextAsync("Create New Item");
            await dashboardPage.ProductTour.ClickFinishButton();

            await Expect(dashboardPage.ProductTour.ShepherdHeader).ToHaveCountAsync(0);
        }

        [Test]
        public async Task UserManagementProductTour()
        {
            // Initialise page objects
            var loginPage = new LoginPage(Page);
            var userListingPage = new UserListingPage(Page);

            // Log in
            await loginPage.GotoAsync();
            await loginPage.LoginAsync(loginAccount);

            // Go to the User Management page and start the tour
            await userListingPage.GotoAsync(false);
            await Expect(userListingPage.ProductTour.TourWelcome).ToHaveTextAsync("Welcome!");
            await userListingPage.ProductTour.ClickShowMeButton();

            await Expect(userListingPage.ProductTour.GetShepherdLocator(1)).ToHaveTextAsync("Overview");
            await userListingPage.ProductTour.ClickNextButton();

            await Expect(userListingPage.ProductTour.GetShepherdLocator(2)).ToHaveTextAsync("General Settings");
            await userListingPage.ProductTour.ClickNextButton();

            await Expect(userListingPage.ProductTour.GetShepherdLocator(3)).ToHaveTextAsync("User Settings");
            await userListingPage.ProductTour.ClickNextButton();

            await Expect(userListingPage.ProductTour.GetShepherdLocator(4)).ToHaveTextAsync("Login As");
            await userListingPage.ProductTour.ClickFinishButton();

            await Expect(userListingPage.ProductTour.ShepherdHeader).ToHaveCountAsync(0);
        }
    }
}
