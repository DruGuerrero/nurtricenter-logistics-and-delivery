namespace Nurtricenter.Test.Infrastructure.Services;

using System.Net;
using FluentAssertions;
using Joseco.DDD.Core.Results;
using Nurtricenter.Core.Interfaces.Services.ClinicService.Dto;
using Nurtricenter.Infrastructure.Services;
using RichardSzalay.MockHttp;

public class ClinicServiceTests
{
    private const string BaseUrl = "https://clinic-service.test";
    private const string ContactInfoEndpoint = "/api/v1/patients/contact-information";

    /// <summary>
    /// Builds a <see cref="ClinicService"/> whose HttpClient is backed by the given MockHttpMessageHandler.
    /// </summary>
    private static ClinicService BuildSut(MockHttpMessageHandler mockHttp)
    {
        var httpClient = mockHttp.ToHttpClient();
        httpClient.BaseAddress = new Uri(BaseUrl);
        return new ClinicService(httpClient);
    }

    [Fact]
    public async Task GetPatientsContactInfoAsync_WithEmptyList_ReturnsEmptyWithoutHttpCall()
    {
        // Arrange
        var mockHttp = new MockHttpMessageHandler();
        var sut = BuildSut(mockHttp);

        // Act
        var result = await sut.GetPatientsContactInfoAsync([], CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
        mockHttp.VerifyNoOutstandingRequest();
    }

    [Fact]
    public async Task GetPatientsContactInfoAsync_WithNullList_ReturnsEmptyWithoutHttpCall()
    {
        // Arrange
        var mockHttp = new MockHttpMessageHandler();
        var sut = BuildSut(mockHttp);

        // Act
        var result = await sut.GetPatientsContactInfoAsync(null!, CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
        mockHttp.VerifyNoOutstandingRequest();
    }

    [Fact]
    public async Task GetPatientsContactInfoAsync_WithValidIds_SendsPostToCorrectEndpoint()
    {
        // Arrange
        var mockHttp = new MockHttpMessageHandler();
        mockHttp
            .Expect(HttpMethod.Post, BaseUrl + ContactInfoEndpoint)
            .Respond("application/json", """
                [{ "patientId": "p-1", "phoneNumber": "555-0001", "fullName": "Alice" }]
                """);

        var sut = BuildSut(mockHttp);

        // Act
        await sut.GetPatientsContactInfoAsync(["p-1"], CancellationToken.None);

        // Assert
        mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task GetPatientsContactInfoAsync_WithValidIds_SendsPatientIdsInRequestBody()
    {
        // Arrange
        var mockHttp = new MockHttpMessageHandler();
        mockHttp
            .Expect(HttpMethod.Post, BaseUrl + ContactInfoEndpoint)
            .WithJsonContent(new[] { "p-1", "p-2" })
            .Respond("application/json", """
                [
                  { "patientId": "p-1", "phoneNumber": "555-0001", "fullName": "Alice" },
                  { "patientId": "p-2", "phoneNumber": "555-0002", "fullName": "Bob" }
                ]
                """);

        var sut = BuildSut(mockHttp);

        // Act
        await sut.GetPatientsContactInfoAsync(["p-1", "p-2"], CancellationToken.None);

        // Assert - expectation includes body match, so this confirms the exact IDs were sent
        mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task GetPatientsContactInfoAsync_WhenServerReturns200_ReturnsMappedPatients()
    {
        // Arrange
        var mockHttp = new MockHttpMessageHandler();
        mockHttp
            .When(HttpMethod.Post, BaseUrl + ContactInfoEndpoint)
            .Respond("application/json", """
                [
                  { "patientId": "p-1", "phoneNumber": "555-0001", "fullName": "Alice Wonder" },
                  { "patientId": "p-2", "phoneNumber": "555-0002", "fullName": "Bob Builder" }
                ]
                """);

        var sut = BuildSut(mockHttp);

        // Act
        var result = await sut.GetPatientsContactInfoAsync(["p-1", "p-2"], CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
        result.Should().ContainEquivalentOf(new PatientContactInfo("p-1", "555-0001", "Alice Wonder"));
        result.Should().ContainEquivalentOf(new PatientContactInfo("p-2", "555-0002", "Bob Builder"));
    }

    [Fact]
    public async Task GetPatientsContactInfoAsync_WhenServerReturns500_ThrowsDomainExceptionWithRequestFailedCode()
    {
        // Arrange
        var mockHttp = new MockHttpMessageHandler();
        mockHttp
            .When(HttpMethod.Post, BaseUrl + ContactInfoEndpoint)
            .Respond(HttpStatusCode.InternalServerError, "text/plain", "Internal server error");

        var sut = BuildSut(mockHttp);
        var act = async () => await sut.GetPatientsContactInfoAsync(["p-1"], CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .Where(ex => ex.Error.Code == "ClinicService.RequestFailed");
    }

    [Fact]
    public async Task GetPatientsContactInfoAsync_WhenServerReturns404_ThrowsDomainExceptionWithRequestFailedCode()
    {
        // Arrange
        var mockHttp = new MockHttpMessageHandler();
        mockHttp
            .When(HttpMethod.Post, BaseUrl + ContactInfoEndpoint)
            .Respond(HttpStatusCode.NotFound, "text/plain", "Not found");

        var sut = BuildSut(mockHttp);
        var act = async () => await sut.GetPatientsContactInfoAsync(["p-1"], CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .Where(ex => ex.Error.Code == "ClinicService.RequestFailed");
    }

    [Fact]
    public async Task GetPatientsContactInfoAsync_WhenServerReturnsNullBody_ThrowsDomainExceptionWithEmptyResponseCode()
    {
        // Arrange - server sends HTTP 200 but the JSON body is the literal "null"
        var mockHttp = new MockHttpMessageHandler();
        mockHttp
            .When(HttpMethod.Post, BaseUrl + ContactInfoEndpoint)
            .Respond("application/json", "null");

        var sut = BuildSut(mockHttp);
        var act = async () => await sut.GetPatientsContactInfoAsync(["p-1"], CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .Where(ex => ex.Error.Code == "ClinicService.EmptyResponse");
    }
}
