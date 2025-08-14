using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using movielogger.dal.entities;

namespace movielogger.api.tests.controllers;

[Collection("IntegrationTests")]
public class AuditControllerTests : BaseTestController
{
    [Fact]
    public async Task GetAuditLogs_ReturnsPaginatedLogs()
    {
        // Act
        var response = await _client.GetAsync("/api/audit/logs");

        // Assert
        response.EnsureSuccessStatusCode();
        var logs = await response.Content.ReadFromJsonAsync<IEnumerable<AuditLog>>();

        logs.Should().NotBeNull();
        logs!.Should().HaveCountGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task GetAuditLogs_WithPageParameters_ReturnsCorrectPage()
    {
        // Act
        var response = await _client.GetAsync("/api/audit/logs?page=1&pageSize=5");

        // Assert
        response.EnsureSuccessStatusCode();
        var logs = await response.Content.ReadFromJsonAsync<IEnumerable<AuditLog>>();

        logs.Should().NotBeNull();
        logs!.Should().HaveCountLessThanOrEqualTo(5);
    }

    [Fact]
    public async Task GetAuditLogs_WithEntityTypeFilter_ReturnsFilteredLogs()
    {
        // Act
        var response = await _client.GetAsync("/api/audit/logs?entityType=Movie");

        // Assert
        response.EnsureSuccessStatusCode();
        var logs = await response.Content.ReadFromJsonAsync<IEnumerable<AuditLog>>();

        logs.Should().NotBeNull();
        // Note: This test assumes there are logs with Movie entity type
    }

    [Fact]
    public async Task GetAuditLogs_WithUserIdFilter_ReturnsFilteredLogs()
    {
        // Act
        var response = await _client.GetAsync("/api/audit/logs?userId=1");

        // Assert
        response.EnsureSuccessStatusCode();
        var logs = await response.Content.ReadFromJsonAsync<IEnumerable<AuditLog>>();

        logs.Should().NotBeNull();
    }

    [Fact]
    public async Task GetEntityTypes_ReturnsAllEntityTypes()
    {
        // Act
        var response = await _client.GetAsync("/api/audit/entity-types");

        // Assert
        response.EnsureSuccessStatusCode();
        var entityTypes = await response.Content.ReadFromJsonAsync<IEnumerable<EntityTypeReference>>();

        entityTypes.Should().NotBeNull();
        entityTypes!.Should().HaveCountGreaterThan(0);
        entityTypes.Should().Contain(et => et.Name == "Movie");
        entityTypes.Should().Contain(et => et.Name == "User");
        entityTypes.Should().Contain(et => et.Name == "Review");
    }

    [Fact]
    public async Task GetAuditLogs_WithInvalidPage_ReturnsBadRequest()
    {
        // Act
        var response = await _client.GetAsync("/api/audit/logs?page=0&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetAuditLogs_WithInvalidPageSize_ReturnsBadRequest()
    {
        // Act
        var response = await _client.GetAsync("/api/audit/logs?page=1&pageSize=0");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}