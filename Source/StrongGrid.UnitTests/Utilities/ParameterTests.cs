using Shouldly;
using StrongGrid.Utilities;
using Xunit;

namespace StrongGrid.UnitTests.Utilities
{
#pragma warning disable RECS0088 // Comparing equal expression for equality is usually useless
	public class ParameterTests
	{
		[Fact]
		public void HasValue_false()
		{
			// Arrange
			var parameter = new Parameter<string>();

			// Act
			var result = parameter.HasValue;

			// Assert
			result.ShouldBeFalse();
		}

		[Fact]
		public void HasValue_true_when_null_string()
		{
			// Arrange
			var parameter = new Parameter<string>(null);

			// Act
			var result = parameter.HasValue;

			// Assert
			result.ShouldBeTrue();
		}

		[Fact]
		public void HasValue_true_when_string()
		{
			// Arrange
			var parameter = new Parameter<string>("abc123");

			// Act
			var result = parameter.HasValue;

			// Assert
			result.ShouldBeTrue();
		}

		[Fact]
		public void HasValue_true_when_nullable()
		{
			// Arrange
			var parameter = new Parameter<long?>(null);

			// Act
			var result = parameter.HasValue;

			// Assert
			result.ShouldBeTrue();
		}

		[Fact]
		public void Implicit_Instance()
		{
			// Act
			Parameter<string> parameter = "abc123";

			// Assert
			parameter.Value.ShouldBe("abc123");
		}

		[Fact]
		public void Implicit_Convert()
		{
			// Act
			string value = new Parameter<string>("abc123");

			// Assert
			value.ShouldBe("abc123");
		}

		[Fact]
		public void Operator_TRUE()
		{
			(new Parameter<string>() ? true : false).ShouldBeFalse();
			(new Parameter<string>("abc123") ? true : false).ShouldBeTrue();
		}

		[Fact]
		public void Operator_NOT()
		{
			(!new Parameter<string>()).ShouldBeTrue();
			(!new Parameter<string>("abc123")).ShouldBeFalse();
		}

		[Fact]
		public void Operator_EQUAL()
		{
			(new Parameter<string>("abc123") == new Parameter<string>("abc123")).ShouldBeTrue();
			(new Parameter<string>("abc123") == new Parameter<string>("qwerty")).ShouldBeFalse();
		}

		[Fact]
		public void Operator_NOT_EQUAL()
		{
			(new Parameter<string>("abc123") != new Parameter<string>()).ShouldBeTrue();
		}

		[Fact]
		public void Operator_OR()
		{
			(new Parameter<string>() | "abc123").ShouldBe("abc123");
			(new Parameter<string>("qwerty") | "abc123").ShouldBe("qwerty");
		}

		[Fact]
		public void Equals_parameter()
		{
			(new Parameter<string>()).Equals(new Parameter<string>("abc123")).ShouldBeFalse();
			(new Parameter<string>("abc123")).Equals(new Parameter<string>()).ShouldBeFalse();
			(new Parameter<string>()).Equals(new Parameter<string>()).ShouldBeTrue();
			(new Parameter<string>("abc123")).Equals(new Parameter<string>("abc123")).ShouldBeTrue();
			(new Parameter<string>("abc123")).Equals(new Parameter<string>("qwerty")).ShouldBeFalse();
		}

		[Fact]
		public void Equals_object()
		{
			(new Parameter<string>("abc123")).Equals((object)98765).ShouldBeFalse();
			(new Parameter<string>("abc123")).Equals((object)new Parameter<string>("abc123")).ShouldBeTrue();
		}

		[Fact]
		public void HashCode()
		{
			(new Parameter<string>()).GetHashCode().ShouldBe(0);
			(new Parameter<string>("abc123")).GetHashCode().ShouldBe("abc123".GetHashCode());
		}
	}
#pragma warning restore RECS0088 // Comparing equal expression for equality is usually useless
}
