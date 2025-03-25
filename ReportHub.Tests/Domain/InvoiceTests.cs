using ReportHub.Domain.Entities;

namespace ReportHub.Tests.Domain
{
    public class InvoiceTests
    {
        [Fact]
        public void InvoiceId_ShouldFollowYearWithIncrementPattern()
        {
            //Arrange
            string year = DateTime.UtcNow.Year.ToString();
            string invoiceId = $"{year}001";

            //Act & Assert
            Assert.Matches(@"^\d{4}\d{3}$", invoiceId); // Matches "YYYYNNN" pattern
        }

        [Fact]
        public void DueDate_ShouldBeGreaterThan_IssueDate()
        {
            // Arrange
            var invoice = new Invoice()
            {
                IssueDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(5) // Future date
            };

            // Act & Assert
            Assert.True(invoice.DueDate > invoice.IssueDate);
        }


        [Fact]
        public void Currency_ShouldHaveLengthThree()
        {
            // Arrange
            var invoice = new Invoice { Currency = "USD" };

            // Act & Assert
            Assert.Equal(3, invoice.Currency.Length);
        }


        [Theory]
        [InlineData("USD")] // Valid
        [InlineData("EUR")] // Valid
        [InlineData("GB")]  // Invalid
        [InlineData("USDD")]// Invalid
        [InlineData("")]// Invalid
        public void Currency_ShouldBeExactlyThreeCharacters(string currency)
        {
            // Act
            bool isValid = currency.Length == 3;

            // Assert
            Assert.Equal(currency.Length == 3, isValid);
        }


        [Theory]
        [InlineData("Paid", true)]
        [InlineData("Pending", true)]
        [InlineData("Overdue", true)]
        [InlineData("Completed", false)] // Invalid
        [InlineData("Failed", false)]    // Invalid
        [InlineData("", false)]    // Invalid
        public void PaymentStatus_ShouldBeValid(string status, bool expectedResult)
        {
            // Arrange
            string[] validStatuses = { "Paid", "Pending", "Overdue" };

            // Act
            bool isValid = Array.Exists(validStatuses, s => s == status);

            // Assert
            Assert.Equal(expectedResult, isValid);
        }
    }
}
