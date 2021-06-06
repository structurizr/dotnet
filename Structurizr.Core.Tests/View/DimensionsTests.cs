using System;
using Xunit;

namespace Structurizr.Core.Tests.View
{
    public class DimensionsTests
    {
        [Fact]
        public void Test_Construction()
        {
            Dimensions dimensions = new Dimensions(123, 456);

            Assert.Equal(123, dimensions.Width);
            Assert.Equal(456, dimensions.Height);
        }

        [Fact]
        public void Test_Width_ThrowsAnException_WhenANegativeIntegerIsSpecified()
        {
            try
            {
                Dimensions dimensions = new Dimensions();
                dimensions.Width = -100;
                throw new TestFailedException();
            }
            catch (ArgumentException iae)
            {
                Assert.Equal("The width must be a positive integer.", iae.Message);
            }
        }

        [Fact]
        public void Test_Height_ThrowsAnException_WhenANegativeIntegerIsSpecified()
        {
            try
            {
                Dimensions dimensions = new Dimensions();
                dimensions.Height = -100;
                throw new TestFailedException();
            }
            catch (ArgumentException iae)
            {
                Assert.Equal("The height must be a positive integer.", iae.Message);
            }
        }
        
    }
}