using System.Collections.Generic;
using Iata.IS.Model.Cargo;

namespace Iata.IS.Business.Cargo
{
  public class AwbRecordComparer : IEqualityComparer<AwbRecord>
  {

    private readonly List<AwbRecord> _equalCoupons;

    public AwbRecordComparer()
    {
      _equalCoupons = new List<AwbRecord>();
    }

    /// <summary>
    /// Gets the equal coupons.
    /// </summary>
    /// <value>The equal coupons.</value>
    public List<AwbRecord> EqualCoupons
    {
      get
      {
        return _equalCoupons;
      }
    }

    public bool Equals(AwbRecord x, AwbRecord y)
    {
      //Check whether the compared objects reference the same data.
      if (ReferenceEquals(x, y)) return true;

      //Check whether any of the compared objects is null.
      if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
        return false;

      //Check whether the products' properties are equal.
      bool isEqual = x.BatchSequenceNumber == y.BatchSequenceNumber && x.RecordSequenceWithinBatch == y.RecordSequenceWithinBatch;

      if (isEqual)
      {
        _equalCoupons.Add(x);
      }

      return isEqual;
    }

    public int GetHashCode(AwbRecord awbRecord)
    {
      //Check whether the object is null
      if (ReferenceEquals(awbRecord, null)) return 0;

      //Get hash code for the BatchSequenceNumber field if it is not null.
      int hashBatchSequenceNumber = awbRecord.BatchSequenceNumber.GetHashCode();

      //Get hash code for the RecordSequenceWithinBatch field.
      int hashRecordSequenceWithinBatch = awbRecord.RecordSequenceWithinBatch.GetHashCode();

      //Calculate the hash code for the CouponRecord.
      return hashBatchSequenceNumber ^ hashRecordSequenceWithinBatch;
    }
  }
}
