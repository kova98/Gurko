using Gurko.Api;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Gurko.Test;

public record TestResult(string Name);

public class ResultTests
{
    [Fact]
    public void Ok_ToHttpResult_ShouldMapBody()
    {
        var okResult = Result<TestResult>.Ok(new TestResult("Test"));
        
        var result = okResult.ToHttpResult();
        
        result.Should().BeOfType<Ok<TestResult>>().Which.Value?.Name.Should().Be("Test");
    }
    
    [Fact]
    public void Created_ToHttpResult_ShouldMapLocation()
    {
        var id = Guid.NewGuid();
        var createdResult = Result<Guid>.Created(id);
        
        var result = createdResult.ToHttpResult("subscriber");
        
        result.Should().BeOfType<Created>().Which.Location.Should().Be($"subscriber/{id}");
    }
    
    [Fact]
    public void Fail_ToHttpResult_ShouldMapError()
    {
        var failResult = Result<TestResult>.Fail("Test");
        
        var result = failResult.ToHttpResult();
        
        result.Should().BeOfType<BadRequest<ErrorBody>>().Which!.Value!.Error.Should().Be("Test");
    }
}