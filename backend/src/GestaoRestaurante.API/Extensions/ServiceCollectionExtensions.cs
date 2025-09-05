using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using GestaoRestaurante.Application.Interfaces;
using GestaoRestaurante.Application.Services;
using GestaoRestaurante.Application.Mappings;
using GestaoRestaurante.Application.Validators;
using FluentValidation;
using GestaoRestaurante.Domain.Entities;
using GestaoRestaurante.Domain.Repositories;
using GestaoRestaurante.API.Authorization;
// using GestaoRestaurante.Infrastructure.Data.Context;
// using GestaoRestaurante.Infrastructure.Data.Repositories;

namespace GestaoRestaurante.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddActionFilters(this IServiceCollection services)
    {
        // Action Filters
        services.AddScoped<GestaoRestaurante.API.Filters.ValidationActionFilter>();
        services.AddScoped<GestaoRestaurante.API.Filters.LoggingActionFilter>();
        services.AddScoped<GestaoRestaurante.API.Filters.PerformanceActionFilter>();
        services.AddScoped<GestaoRestaurante.API.Filters.ResponseWrapperFilter>();

        return services;
    }


}