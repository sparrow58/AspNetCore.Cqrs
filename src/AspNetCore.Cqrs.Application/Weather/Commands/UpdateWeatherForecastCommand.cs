﻿using AspNetCore.Cqrs.Application.Abstractions.Commands;
using AspNetCore.Cqrs.Application.Abstractions.Repositories;
using AspNetCore.Cqrs.Core.Weather.Entities;
using AspNetCore.Cqrs.Core.Abstractions.Guards;

namespace AspNetCore.Cqrs.Application.Weather.Commands
{
    public sealed record UpdateWeatherForecastCommand(Guid Id, DateTime Date) : Command;

    public sealed class UpdateWeatherForecastCommandHandler : CommandHandler<UpdateWeatherForecastCommand>
    {
        private readonly IRepository<WeatherForecast> _repository;

        public UpdateWeatherForecastCommandHandler(IRepository<WeatherForecast> repository,
            IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _repository = repository;
        }

        protected override async Task HandleAsync(UpdateWeatherForecastCommand request)
        {
            var forecast = await _repository.GetByIdAsync(request.Id);
            forecast = Guard.Against.NotFound(forecast);
            forecast.UpdateDate(request.Date);
            await UnitOfWork.CommitAsync();
        }
    }
}
