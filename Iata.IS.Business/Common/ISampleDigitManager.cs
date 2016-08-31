using System;
using System.Collections.Generic;
using Iata.IS.Model.Common;
using Iata.IS.Model.Pax.Common;

namespace Iata.IS.Business.Common
{
    public interface ISampleDigitManager
    {
        /// <summary>
        /// Adds the sample digit.
        /// </summary>
        /// <param name="sampleDigit">The sample digit.</param>
        /// <returns></returns>
        SampleDigit AddSampleDigit(SampleDigit sampleDigit);

        /// <summary>
        /// Updates the sample digit.
        /// </summary>
        /// <param name="sampleDigit">The sample digit.</param>
        /// <returns></returns>
        SampleDigit UpdateSampleDigit(SampleDigit sampleDigit);

        /// <summary>
        /// Deletes the sample digit.
        /// </summary>
        /// <param name="sampleDigitId">The sample digit id.</param>
        /// <returns></returns>
        bool DeleteSampleDigit(int sampleDigitId);

        /// <summary>
        /// Gets the sample digit details.
        /// </summary>
        /// <param name="sampleDigitId">The sample digit id.</param>
        /// <returns></returns>
        SampleDigit GetSampleDigitDetails(int sampleDigitId);

        /// <summary>
        /// Gets all sample digit list.
        /// </summary>
        /// <returns></returns>
        List<SampleDigit> GetAllSampleDigitList();

        /// <summary>
        /// Gets the sample digit list.
        /// </summary>
        /// <param name="billingMonth">The billing month.</param>
        /// <returns></returns>
        List<SampleDigit> GetSampleDigitList(string billingMonth);
    }
}
