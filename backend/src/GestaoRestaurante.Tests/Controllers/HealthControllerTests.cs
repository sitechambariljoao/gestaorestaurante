using System;
using System.Threading.Tasks;
using FluentAssertions;
using GestaoRestaurante.API.Controllers;
using GestaoRestaurante.Infrastructure.Data.Context;
using GestaoRestaurante.Application.Common.Interfaces;
using GestaoRestaurante.Application.Common.Caching;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using System.Reflection;
using System.Collections.Generic;

namespace GestaoRestaurante.Tests.Controllers
{
    public class HealthControllerTests
    {
        private readonly Mock<GestaoRestauranteContext> _contextMock;
        private readonly Mock<DatabaseFacade> _databaseMock;
        private readonly Mock<IApplicationMetrics> _metricsMock;
        private readonly Mock<IPerformanceProfiler> _profilerMock;
        private readonly Mock<ICacheService> _cacheMock;
        private readonly Mock<ILogger<HealthController>> _loggerMock;
        private readonly HealthController _controller;

        public HealthControllerTests()
        {
            var options = new DbContextOptionsBuilder<GestaoRestauranteContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _contextMock = new Mock<GestaoRestauranteContext>(options);
            _databaseMock = new Mock<DatabaseFacade>(_contextMock.Object);
            _metricsMock = new Mock<IApplicationMetrics>();
            _profilerMock = new Mock<IPerformanceProfiler>();
            _cacheMock = new Mock<ICacheService>();
            _loggerMock = new Mock<ILogger<HealthController>>();

            _contextMock.Setup(x => x.Database).Returns(_databaseMock.Object);

            _controller = new HealthController(_contextMock.Object, _metricsMock.Object, _profilerMock.Object, _cacheMock.Object, _loggerMock.Object);
        }

        #region GetHealth Tests

        [Fact]
        public async Task GetHealth_DeveRetornarOk_ComStatusHealthy()
        {
            // Arrange
            _databaseMock.Setup(x => x.CanConnectAsync(default))
                .ReturnsAsync(true);
            _databaseMock.Setup(x => x.GetConnectionString())
                .Returns("Server=test;Database=test;");

            // Act
            var result = await _controller.GetHealth();

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
            
            var healthStatus = okResult.Value;
            healthStatus.Should().NotBeNull();
            
            // Verificar se contém as propriedades esperadas usando reflection
            var statusProperty = healthStatus!.GetType().GetProperty("Status");
            statusProperty.Should().NotBeNull();
            statusProperty!.GetValue(healthStatus).Should().Be("Healthy");

            var timestampProperty = healthStatus.GetType().GetProperty("Timestamp");
            timestampProperty.Should().NotBeNull();
            timestampProperty!.GetValue(healthStatus).Should().BeOfType<DateTime>();

            var versionProperty = healthStatus.GetType().GetProperty("Version");
            versionProperty.Should().NotBeNull();
            versionProperty!.GetValue(healthStatus).Should().Be("1.0.0");

            var databaseProperty = healthStatus.GetType().GetProperty("Database");
            databaseProperty.Should().NotBeNull();
        }

        [Fact]
        public async Task GetHealth_DeveRetornarOk_MesmoComFalhaNoBanco()
        {
            // Arrange
            _databaseMock.Setup(x => x.CanConnectAsync(default))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.GetHealth();

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);

            var healthStatus = okResult.Value;
            healthStatus.Should().NotBeNull();

            var statusProperty = healthStatus!.GetType().GetProperty("Status");
            statusProperty!.GetValue(healthStatus).Should().Be("Healthy");
        }

        [Fact]
        public async Task GetHealth_DeveRetornarOk_QuandoBancoLancaExcecao()
        {
            // Arrange
            _databaseMock.Setup(x => x.CanConnectAsync(default))
                .ThrowsAsync(new Exception("Connection failed"));

            // Act
            var result = await _controller.GetHealth();

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);

            var healthStatus = okResult.Value;
            healthStatus.Should().NotBeNull();

            var statusProperty = healthStatus!.GetType().GetProperty("Status");
            statusProperty!.GetValue(healthStatus).Should().Be("Healthy");
        }

        #endregion

        #region GetDetailedHealth Tests

        [Fact]
        public async Task GetDetailedHealth_DeveRetornarOk_QuandoTudoHealthy()
        {
            // Arrange
            _databaseMock.Setup(x => x.CanConnectAsync(default))
                .ReturnsAsync(true);
            _databaseMock.Setup(x => x.GetConnectionString())
                .Returns("Server=test;Database=test;Password=secret;");

            // Act
            var result = await _controller.GetDetailedHealth();

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);

            var healthStatus = okResult.Value;
            healthStatus.Should().NotBeNull();

            var statusProperty = healthStatus!.GetType().GetProperty("Status");
            statusProperty!.GetValue(healthStatus).Should().Be("Healthy");

            var checksProperty = healthStatus.GetType().GetProperty("Checks");
            checksProperty.Should().NotBeNull();
            var checks = checksProperty!.GetValue(healthStatus).Should().BeOfType<Dictionary<string, object>>().Subject;

            checks.Should().ContainKey("Database");
            checks.Should().ContainKey("Memory");
            checks.Should().ContainKey("Uptime");
        }

        [Fact]
        public async Task GetDetailedHealth_DeveRetornarServiceUnavailable_QuandoBancoUnhealthy()
        {
            // Arrange
            _databaseMock.Setup(x => x.CanConnectAsync(default))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.GetDetailedHealth();

            // Assert
            var statusResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
            statusResult.StatusCode.Should().Be(StatusCodes.Status503ServiceUnavailable);

            var healthStatus = statusResult.Value;
            healthStatus.Should().NotBeNull();

            var statusProperty = healthStatus!.GetType().GetProperty("Status");
            statusProperty!.GetValue(healthStatus).Should().Be("Unhealthy");
        }

        [Fact]
        public async Task GetDetailedHealth_DeveRetornarServiceUnavailable_QuandoBancoLancaExcecao()
        {
            // Arrange
            _databaseMock.Setup(x => x.CanConnectAsync(default))
                .ThrowsAsync(new Exception("Database connection failed"));

            // Act
            var result = await _controller.GetDetailedHealth();

            // Assert
            var statusResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
            statusResult.StatusCode.Should().Be(StatusCodes.Status503ServiceUnavailable);

            var healthStatus = statusResult.Value;
            healthStatus.Should().NotBeNull();

            var statusProperty = healthStatus!.GetType().GetProperty("Status");
            statusProperty!.GetValue(healthStatus).Should().Be("Unhealthy");

            var checksProperty = healthStatus.GetType().GetProperty("Checks");
            var checks = checksProperty!.GetValue(healthStatus).Should().BeOfType<Dictionary<string, object>>().Subject;

            checks.Should().ContainKey("Database");
            var databaseCheck = checks["Database"];
            
            var dbStatusProperty = databaseCheck.GetType().GetProperty("Status");
            dbStatusProperty!.GetValue(databaseCheck).Should().Be("Unhealthy");

            var dbErrorProperty = databaseCheck.GetType().GetProperty("Error");
            dbErrorProperty!.GetValue(databaseCheck).Should().Be("Database connection failed");
        }

        [Fact]
        public async Task GetDetailedHealth_DeveIncluirChecksDeMemoriaEUptime()
        {
            // Arrange
            _databaseMock.Setup(x => x.CanConnectAsync(default))
                .ReturnsAsync(true);
            _databaseMock.Setup(x => x.GetConnectionString())
                .Returns("Server=test;Database=test;");

            // Act
            var result = await _controller.GetDetailedHealth();

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var healthStatus = okResult.Value;

            var checksProperty = healthStatus!.GetType().GetProperty("Checks");
            var checks = checksProperty!.GetValue(healthStatus).Should().BeOfType<Dictionary<string, object>>().Subject;

            // Verificar check de memória
            checks.Should().ContainKey("Memory");
            var memoryCheck = checks["Memory"];
            var memoryStatusProperty = memoryCheck.GetType().GetProperty("Status");
            var memoryUsageProperty = memoryCheck.GetType().GetProperty("UsageBytes");
            var memoryUsageMBProperty = memoryCheck.GetType().GetProperty("UsageMB");

            memoryStatusProperty.Should().NotBeNull();
            memoryUsageProperty.Should().NotBeNull();
            memoryUsageMBProperty.Should().NotBeNull();

            // Verificar check de uptime
            checks.Should().ContainKey("Uptime");
            var uptimeCheck = checks["Uptime"];
            var uptimeStatusProperty = uptimeCheck.GetType().GetProperty("Status");
            var uptimeMsProperty = uptimeCheck.GetType().GetProperty("UptimeMs");
            var uptimeFormattedProperty = uptimeCheck.GetType().GetProperty("UptimeFormatted");

            uptimeStatusProperty!.GetValue(uptimeCheck).Should().Be("Healthy");
            uptimeMsProperty.Should().NotBeNull();
            uptimeFormattedProperty.Should().NotBeNull();
        }

        [Fact]
        public async Task GetDetailedHealth_DeveMascararSenhaNoConnectionString()
        {
            // Arrange
            _databaseMock.Setup(x => x.CanConnectAsync(default))
                .ReturnsAsync(true);
            _databaseMock.Setup(x => x.GetConnectionString())
                .Returns("Server=localhost;Database=TestDB;Password=supersecret;User=admin;");

            // Act
            var result = await _controller.GetDetailedHealth();

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var healthStatus = okResult.Value;

            var checksProperty = healthStatus!.GetType().GetProperty("Checks");
            var checks = checksProperty!.GetValue(healthStatus).Should().BeOfType<Dictionary<string, object>>().Subject;

            var databaseCheck = checks["Database"];
            var connectionStringProperty = databaseCheck.GetType().GetProperty("ConnectionString");
            
            if (connectionStringProperty != null)
            {
                var connectionString = connectionStringProperty.GetValue(databaseCheck) as string;
                connectionString.Should().Contain("Password=***");
                connectionString.Should().NotContain("supersecret");
            }
        }

        #endregion
    }
}