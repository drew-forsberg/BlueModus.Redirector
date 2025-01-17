using BlueModus.Redirector.Middleware.Models;

namespace BlueModus.Redirector.Middleware.Tests.Unit.Models
{
    public class RedirectItemComparerTests
    {
        [Fact]
        public void GetMatchingRedirectItem_RequestUrl_Null_Should_Return_Null()
        {
            // Arrange
            const string? requestUrl = null;
            var redirectItemComparer = new RedirectItemComparer();

            // Act
            var actual = redirectItemComparer.GetMatchingRedirectItem(requestUrl, []);

            // Assert
            Assert.Null(actual);
        }

        [Fact]
        public void GetMatchingRedirectItem_RequestUrl_Empty_Should_Return_Null()
        {
            // Arrange
            const string? requestUrl = "";
            var redirectItemComparer = new RedirectItemComparer();

            // Act
            var actual = redirectItemComparer.GetMatchingRedirectItem(requestUrl, []);

            // Assert
            Assert.Null(actual);
        }

        [Fact]
        public void GetMatchingRedirectItem_RequestUrl_Root_Should_Return_Null()
        {
            // Arrange
            const string? requestUrl = "/";
            var redirectItemComparer = new RedirectItemComparer();

            // Act
            var actual = redirectItemComparer.GetMatchingRedirectItem(requestUrl, []);

            // Assert
            Assert.Null(actual);
        }

        [Fact]
        public void GetMatchingRedirectItem_ExactMatch_Should_Return_Expected_Redirect()
        {
            // Arrange
            const string? requestUrl = "/campaignA";
            var redirectItemComparer = new RedirectItemComparer();

            var redirectItem = new RedirectItem
            {
                TargetUrl = "/campaigns/targetcampaign",
                RedirectUrl = "/campaignA",
                RedirectType = 302,
                UseRelative = false
            };

            var redirectItems = new List<RedirectItem>
            {
                redirectItem
            };

            var expected = new RedirectResult
            {
                TargetUrl = redirectItem.TargetUrl,
                IsPermanent = redirectItem.RedirectType == 301
            };

            // Act
            var actual = redirectItemComparer.GetMatchingRedirectItem(requestUrl, redirectItems);

            // Assert
            Assert.Equivalent(expected, actual);
        }

        [Theory]
        [InlineData("/product-directory", "/products")]
        [InlineData("/product-directory/bits", "/products/bits")]
        [InlineData("/product-directory/bits/masonry", "/products/bits/masonry")]
        [InlineData("/product-directory/bits/masonry/diamond-tip", "/products/bits/masonry/diamond-tip")]
        public void GetMatchingRedirectItem_RelativeMatch_Should_Return_Expected_Redirect(string requestUrl, string targetUrl)
        {
            // Arrange
            var redirectItemComparer = new RedirectItemComparer();

            var redirectItem = new RedirectItem
            {
                RedirectUrl = "/product-directory",
                TargetUrl = "/products",
                RedirectType = 302,
                UseRelative = true
            };

            var redirectItems = new List<RedirectItem>
            {
                redirectItem
            };

            var expected = new RedirectResult
            {
                TargetUrl = targetUrl,
                IsPermanent = redirectItem.RedirectType == 301
            };

            // Act
            var actual = redirectItemComparer.GetMatchingRedirectItem(requestUrl, redirectItems);

            // Assert
            Assert.Equivalent(expected, actual);
        }
    }
}
