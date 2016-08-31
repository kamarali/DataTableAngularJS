using System;
using System.Collections.Generic;
using Iata.IS.Model.Pax.Base;

namespace Iata.IS.Business.Pax
{
  class MemoComparer : IEqualityComparer<MemoBase>
  {
     /// <summary>
    /// Gets the equal coupons.
    /// </summary>
    /// <value>The equal coupons.</value>
    public List<MemoBase>  EqualCoupons 
    { get
      {
        return _equalCoupons;
      }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MemoComparer"/> class.
    /// </summary>
    public MemoComparer()
    {
      _equalCoupons = new List<MemoBase>();
    }

    /// <summary>
    /// 
    /// </summary>
    private readonly List<MemoBase> _equalCoupons;

    /// <summary>
    /// Equals the specified x.
    /// </summary>
    /// <param name="x">The x.</param>
    /// <param name="y">The y.</param>
    /// <returns></returns>
    public bool Equals(MemoBase x, MemoBase y)
    {
      //Check whether the compared objects reference the same data.
      if (Object.ReferenceEquals(x, y)) return true;

      //Check whether any of the compared objects is null.
      if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
        return false;

      //Check whether the products' properties are equal.
      bool isEqual= x.BatchSequenceNumber == y.BatchSequenceNumber && x.RecordSequenceWithinBatch == y.RecordSequenceWithinBatch;

      if(isEqual)
      {
        _equalCoupons.Add(x);
      }
      
      return isEqual;
    }

    /// <summary>
    /// Returns a hash code for this instance.
    /// </summary>
    /// <param name="memoRecordBase">The coupon record.</param>
    /// <returns>
    /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
    /// </returns>
    public int GetHashCode(MemoBase memoRecordBase)
    {
      //Check whether the object is null
      if (ReferenceEquals(memoRecordBase, null)) return 0;

      //Get hash code for the BatchSequenceNumber field if it is not null.
      int hashBatchSequenceNumber = memoRecordBase.BatchSequenceNumber.GetHashCode();

      //Get hash code for the RecordSequenceWithinBatch field.
      int hashRecordSequenceWithinBatch = memoRecordBase.RecordSequenceWithinBatch.GetHashCode();

      //Calculate the hash code for the MemoRecordBase.
      return hashBatchSequenceNumber ^ hashRecordSequenceWithinBatch;
    }
  }
}
