using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using FileHelpers;
using Iata.IS.Core.Configuration;
using Iata.IS.Core.Exceptions;
using Iata.IS.Data;
using Iata.IS.Data.Common;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Data.Impl;
using log4net;

namespace Iata.IS.Business.Common.Impl
{
  public class AirportManager : IAirportManager
  {
    /// <summary>
    /// Gets or sets the airport repository.
    /// </summary>
    /// <value>
    /// The airport repository.
    /// </value>
    public IRepository<Airport> AirportRepository { get; set; }

    /// <summary>
    /// Adds the airport.
    /// </summary>
    /// <param name="airport">The airport.</param>
    /// <returns></returns>
    public Airport AddAirport(Airport airport)
    {
      var AirportData = AirportRepository.Single(type => type.Id.ToLower() == airport.Id.ToLower());

      //If Airport Code already exists, throw exception
      if (AirportData != null)
      {
        throw new ISBusinessException(ErrorCodes.InvalidAirportIcaoCode);
      }
      AirportRepository.Add(airport);
      UnitOfWork.CommitDefault();
      return airport;
    }

    /// <summary>
    /// Updates the airport.
    /// </summary>
    /// <param name="airport">The airport.</param>
    /// <returns></returns>
    public Airport UpdateAirport(Airport airport)
    {
      var AirportData = AirportRepository.Single(type => type.Id.ToLower() == airport.Id.ToLower());
      if (AirportData == null)
      {
        throw new ISBusinessException(ErrorCodes.InvalidAirportIcaoCode);
      }
      var updatedAirport = AirportRepository.Update(airport);
      UnitOfWork.CommitDefault();
      return updatedAirport;
    }

    /// <summary>
    /// Deletes the airport.
    /// </summary>
    /// <param name="airportId">The airport id.</param>
    /// <returns></returns>
    public bool DeleteAirport(string airportId)
    {
      bool delete = false;
      var airportData = AirportRepository.Single(type => type.Id == airportId);
      if (airportData != null)
      {
        airportData.IsActive = !(airportData.IsActive);
        var updatedcountry = AirportRepository.Update(airportData);
        delete = true;
        UnitOfWork.CommitDefault();
      }
      return delete;
    }

    /// <summary>
    /// Gets the airport details.
    /// </summary>
    /// <param name="airportId">The airport id.</param>
    /// <returns></returns>
    public Airport GetAirportDetails(string airportId)
    {
      var Airport = AirportRepository.Single(type => type.Id == airportId);
      return Airport;
    }

    /// <summary>
    /// Gets all airport list.
    /// </summary>
    /// <returns></returns>
    public List<Airport> GetAllAirportList()
    {
      var airportList = AirportRepository.GetAll();

      return airportList.ToList();
    }

    /// <summary>
    /// Gets the airport list.
    /// </summary>
    /// <param name="id">The id.</param>
    /// <param name="name">The name.</param>
    /// <param name="countryCode">The country code.</param>
    /// <returns></returns>
    public List<Airport> GetAirportList(string id, string name, string countryCode)
    {
      var airportList = AirportRepository.GetAll().ToList();

      if ((!string.IsNullOrEmpty(name)))
      {
        airportList = airportList.Where(cl => (cl.Name.ToLower().Contains(name.ToLower()))).ToList();
      }
      if (!string.IsNullOrEmpty(id))
      {
        airportList = airportList.Where(cl => (cl.Id.ToLower().Contains(id.ToLower()))).ToList();
      }
      if (!string.IsNullOrEmpty(countryCode))
      {
        airportList = airportList.Where(cl => (cl.CountryCode.ToLower().Contains(countryCode.ToLower()))).ToList();
      }
      return airportList.ToList();
    }
  }
}
