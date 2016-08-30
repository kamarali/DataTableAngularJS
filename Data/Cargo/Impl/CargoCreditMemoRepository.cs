using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Text;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Cargo;
using Microsoft.Data.Extensions;

namespace Iata.IS.Data.Cargo.Impl
{
  public class CargoCreditMemoRepository
  {
    /// <summary>
    /// This will load list of CargoCreditMemo objects
    /// </summary>
    /// <param name="objectSet"></param>
    /// <param name="loadStrategyResult"></param>
    /// <param name="link"></param>
    /// <returns></returns>
    public static List<CargoCreditMemo> LoadEntities(ObjectSet<CargoCreditMemo> objectSet, LoadStrategyResult loadStrategyResult, Action<CargoCreditMemo> link)
    {
      if (link == null)
        link = new Action<CargoCreditMemo>(c => { });

      var cgoCreditMemo = new List<CargoCreditMemo>();
      var cargoMaterializers = new CargoMaterializers();
      using (var reader = loadStrategyResult.GetReader(LoadStrategy.CargoEntities.CreditMemo))
      {
        // first result set includes the category
          cgoCreditMemo.AddRange(cargoMaterializers.CargoCreditMemoMaterializer.Materialize(reader).Bind(objectSet).ForEach<CargoCreditMemo>(link));
        reader.Close();
      }

      if (cgoCreditMemo.Count > 0)
      {
        if (loadStrategyResult.IsLoaded(LoadStrategy.CargoEntities.CreditMemoVat))
        {
          CargoCreditMemoVatRepository.LoadEntities(objectSet.Context.CreateObjectSet<CargoCreditMemoVat>(), loadStrategyResult, null);
        }

        if (loadStrategyResult.IsLoaded(LoadStrategy.CargoEntities.CreditMemoAttachments))
        {
          CargoCreditMemoAttachmentRepository.LoadEntities(objectSet.Context.CreateObjectSet<CargoCreditMemoAttachment>(), loadStrategyResult, null);
        }
        if (loadStrategyResult.IsLoaded(LoadStrategy.CargoEntities.CmAwb))
        {
          CargoCreditMemoAwbRepository.LoadEntities(objectSet.Context.CreateObjectSet<CMAirWayBill>(),
                                          loadStrategyResult,
                                          null);
        }
      }

      return cgoCreditMemo;
    } 
  } 
}
